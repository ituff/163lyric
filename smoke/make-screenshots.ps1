# 功能截图生成：反射驱动真实 UI + 真实数据（真搜索/真扫描），离屏渲染三张 PNG 到 assets/
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = 'Stop'
$bin = 'D:\Project\163lyrics\163lyric\163lyric\bin\Release\net48'
$icoPath = 'D:\Project\163lyrics\163lyric\163lyric\app.ico'
$assets = 'D:\Project\163lyrics\assets'
$asm = [System.Reflection.Assembly]::LoadFrom((Join-Path $bin 'OneLyric.exe'))
[System.Reflection.Assembly]::LoadFrom((Join-Path $bin 'TagLibSharp.dll')) | Out-Null

function Unwrap($x) { if ($x -is [PSObject]) { $x.psobject.BaseObject } else { $x } }

# ============ 生成 3 个带标签的样例音乐文件（真实扫描用） ============
$sampleDir = 'D:\Project\163lyrics\smoke\sample-music'
if (Test-Path $sampleDir) { Remove-Item -Recurse -Force $sampleDir }
[IO.Directory]::CreateDirectory($sampleDir) | Out-Null

function New-SampleWav([string]$title, [string]$album) {
    $path = Join-Path $sampleDir ("周杰伦 - " + $title + '.wav')
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter($ms)
    foreach ($ch in 'RIFF'.ToCharArray()) { $bw.Write([byte][int]$ch) }
    $bw.Write([int](36 + 8000))
    foreach ($ch in 'WAVE'.ToCharArray()) { $bw.Write([byte][int]$ch) }
    foreach ($ch in 'fmt '.ToCharArray()) { $bw.Write([byte][int]$ch) }
    $bw.Write([int]16); $bw.Write([int16]1); $bw.Write([int16]1)
    $bw.Write([int]8000); $bw.Write([int]16000); $bw.Write([int16]2); $bw.Write([int16]16)
    foreach ($ch in 'data'.ToCharArray()) { $bw.Write([byte][int]$ch) }
    $bw.Write([int]8000)
    for ($i = 0; $i -lt 4000; $i++) { $bw.Write([int16]0) }
    $bw.Flush(); [IO.File]::WriteAllBytes($path, $ms.ToArray()); $bw.Close()

    $tf = [TagLib.File]::Create($path)
    $tf.Tag.Title = $title
    $tf.Tag.Performers = [string[]]@('周杰伦')
    $tf.Tag.Album = $album
    $tf.Save(); $tf.Dispose()
    $path
}

$f1 = New-SampleWav '晴天' '叶惠美'
$f2 = New-SampleWav '七里香' '七里香'
$f3 = New-SampleWav '简单爱' '范特西'

$form = $asm.CreateInstance('_163lyric.Form1')
$form.Icon = New-Object System.Drawing.Icon($icoPath)
$form.StartPosition = 'Manual'
$form.Show()
[System.Windows.Forms.Application]::DoEvents()
Start-Sleep -Milliseconds 600

$tForm = $asm.GetType('_163lyric.Form1')
$BF = [System.Reflection.BindingFlags]'NonPublic,Public,Instance'

# ============ 截图1：搜索歌词（真实网易搜索 + 真实歌词） ============
$src = $asm.CreateInstance('_163lyric.NetEaseSource')
$results = $src.Search('晴天 周杰伦')

# 直接按 ShowSearchResults 同样逻辑填充 ListView（PS 会把返回的 List 展开成 object[]，反射传参太绕）
$list = $tForm.GetField('ResultList', $BF).GetValue($form)
$list.BeginUpdate()
$list.Items.Clear()
foreach ($s in $results) {
    $dur = ''
    if ($s.Duration -gt 0) { $dur = '{0}:{1:D2}' -f [int][math]::Floor($s.Duration / 60), ($s.Duration % 60) }
    $item = New-Object System.Windows.Forms.ListViewItem (,[string[]]@($s.Title, $s.Artist, $s.Album, $dur))
    $item.Tag = (Unwrap $s)
    [void]$list.Items.Add($item)
}
$list.EndUpdate()
Write-Host ("  rows=" + $list.Items.Count)
if ($list.Items.Count -gt 0) {
    $list.Items[0].Selected = $true
    $song = $list.Items[0].Tag
    $lyr = $src.GetLyric($song)
    $tForm.GetField('SearchLyrictb', $BF).GetValue($form).Text = $lyr.ToString()
}
[System.Windows.Forms.Application]::DoEvents()
Start-Sleep -Milliseconds 500
$bmp = New-Object System.Drawing.Bitmap($form.Width, $form.Height)
$form.DrawToBitmap($bmp, (New-Object System.Drawing.Rectangle(0, 0, $form.Width, $form.Height)))
$bmp.Save((Join-Path $assets 'screenshot-search.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host 'shot 1 (search) ok, rows=' $list.Items.Count

# ============ 截图2：批量扫描（真实扫描 3 个样例文件） ============
$tabs = $tForm.GetField('MainTabs', $BF).GetValue($form)
$tabs.SelectedIndex = 1
$tForm.GetField('Dirtb', $BF).GetValue($form).Text = 'D:\Music\周杰伦'

$listType = [System.Collections.Generic.List`1].MakeGenericType($asm.GetType('_163lyric.ILyricSource'))
$sources = [Activator]::CreateInstance($listType)
$listType.GetMethod('Add').Invoke($sources, @($asm.CreateInstance('_163lyric.NetEaseSource')))
$mScan = $tForm.GetMethod('ScanOneFile', $BF)

$log = $tForm.GetField('ScanLogtb', $BF).GetValue($form)
[void]$log.AppendText("共 3 个音乐文件，开始扫描...`r`n")
$done = 0
foreach ($f in @($f1, $f2, $f3)) {
    $line = $mScan.Invoke($form, @((Unwrap $f), $sources, $true))
    $done++
    [void]$log.AppendText($line + "`r`n")
    Write-Host ("  " + $line)
}
[void]$log.AppendText("扫描完成。")
$prog = $tForm.GetField('ScanProgress', $BF).GetValue($form)
$prog.Maximum = 3
$prog.Value = 3
[System.Windows.Forms.Application]::DoEvents()
Start-Sleep -Milliseconds 500
$bmp = New-Object System.Drawing.Bitmap($form.Width, $form.Height)
$form.DrawToBitmap($bmp, (New-Object System.Drawing.Rectangle(0, 0, $form.Width, $form.Height)))
$bmp.Save((Join-Path $assets 'screenshot-scan.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host 'shot 2 (scan) ok'

$form.Close(); $form.Dispose()

# ============ 截图3：关于窗口（补上真实图标） ============
$about = $asm.CreateInstance('_163lyric.AboutForm')
$about.Icon = New-Object System.Drawing.Icon($icoPath)
$pbLogo = $asm.GetType('_163lyric.AboutForm').GetField('pbLogo', $BF).GetValue($about)
$ms = New-Object System.IO.MemoryStream (, [IO.File]::ReadAllBytes('D:\Project\163lyrics\assets\logo-256.png'))
$pbLogo.Image = [System.Drawing.Image]::FromStream($ms)
$about.StartPosition = 'Manual'
$about.Show()
[System.Windows.Forms.Application]::DoEvents()
Start-Sleep -Milliseconds 500
$bmp = New-Object System.Drawing.Bitmap($about.Width, $about.Height)
$about.DrawToBitmap($bmp, (New-Object System.Drawing.Rectangle(0, 0, $about.Width, $about.Height)))
$bmp.Save((Join-Path $assets 'screenshot-about.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
$about.Close(); $about.Dispose()
Write-Host 'shot 3 (about) ok'

Remove-Item -Recurse -Force $sampleDir
Write-Host '=== screenshots done ==='
