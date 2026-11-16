# 163lyric v1.2 冒烟测试：反射调用 exe 内部类，不开 UI
# 覆盖：三源 Search/GetLyric、归一化/打分、带标签 WAV 走 ScanOneFile 批量路径、lrc 保存字节
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$bin = 'D:\Project\163lyrics\163lyric\163lyric\bin\Release\net48'
$asm = [System.Reflection.Assembly]::LoadFrom((Join-Path $bin 'OneLyric.exe'))
[System.Reflection.Assembly]::LoadFrom((Join-Path $bin 'TagLibSharp.dll')) | Out-Null

function Show-Result($r, $max = 3) {
    Write-Host ("  count=" + @($r).Count)
    $i = 0
    foreach ($s in @($r)) {
        if ($i -ge $max) { break }
        Write-Host ("    [" + $i + "] " + $s.Title + " | " + $s.Artist + " | " + $s.Album + " | " + $s.Duration + "s | id=" + $s.NetEaseId + " | hash=" + $s.KugouHash)
        $i++
    }
}

function Show-Lyric($lyr) {
    Write-Host ("  error='" + $lyr.Error + "'")
    Write-Host ("  lyric length=" + $lyr.Lyric.Length + "  translation length=" + $lyr.TLyric.Length)
    $t = $lyr.ToString()
    Write-Host ("  merged length=" + $t.Length)
    Write-Host ("  head: " + $t.Substring(0, [Math]::Min(180, $t.Length)).Replace("`r`n", " / "))
}

# ---------- 1. 网易：搜索 + 歌词（含翻译合并） ----------
Write-Host '=== 1. NetEase Search + GetLyric ==='
try {
    $ne = $asm.CreateInstance('_163lyric.NetEaseSource')
    $r = $ne.Search('晴天 周杰伦')
    Show-Result $r
    if (@($r).Count -gt 0) { Show-Lyric ($ne.GetLyric(@($r)[0])) }
} catch { Write-Host ("  FAILED: " + $_.Exception.Message) }

# ---------- 2. 酷狗：搜索 + 歌词（hash -> id/accesskey -> base64） ----------
Write-Host '=== 2. Kugou Search + GetLyric ==='
try {
    $kg = $asm.CreateInstance('_163lyric.KugouSource')
    $kr = $kg.Search('晴天 周杰伦')
    Show-Result $kr
    if (@($kr).Count -gt 0) { Show-Lyric ($kg.GetLyric(@($kr)[0])) }
} catch { Write-Host ("  FAILED: " + $_.Exception.Message) }

# ---------- 3. LRCLIB：关键词搜索 + 字段搜索 + 歌词 ----------
Write-Host '=== 3. LRCLIB Search + GetLyric ==='
try {
    $lr = $asm.CreateInstance('_163lyric.LrclibSource')
    $lres = $lr.Search('Hello Adele')
    Show-Result $lres
    if (@($lres).Count -gt 0) { Show-Lyric ($lr.GetLyric(@($lres)[0])) }
    Write-Host '  -- SearchDetail(Hello, Adele) --'
    $ld = $lr.SearchDetail('Hello', 'Adele', '')
    Write-Host ("  detail count=" + @($ld).Count)
} catch { Write-Host ("  FAILED: " + $_.Exception.Message) }

# ---------- 4. 归一化与打分（内部类型需反射调用） ----------
Write-Host '=== 4. Normalize / Score ==='
$msType = $asm.GetType('_163lyric.MusicFileScanner')
$mNorm = $msType.GetMethod('Normalize')
$mScore = $msType.GetMethod('Score')
Write-Host ("  Normalize('晴天(Live)') = '" + $mNorm.Invoke($null, @('晴天(Live)')) + "'")
$a = $asm.CreateInstance('_163lyric.SongInfo'); $a.Title = '晴天'; $a.Artist = '周杰伦'; $a.Duration = 269
$b = $asm.CreateInstance('_163lyric.SongInfo'); $b.Title = '晴天'; $b.Artist = '周杰伦'; $b.Duration = 268
Write-Host ("  Score(同名同歌手时长±1) = " + $mScore.Invoke($null, @($a, $b)) + "  (expect 100)")
$c = $asm.CreateInstance('_163lyric.SongInfo'); $c.Title = '晴天'; $c.Artist = ''; $c.Duration = 0
Write-Host ("  Score(仅同名) = " + $mScore.Invoke($null, @($a, $c)) + "  (expect 50, 低于阈值60)")

# ---------- 5. 批量扫描：生成带标签的静音 WAV，走真实 ScanOneFile ----------
Write-Host '=== 5. Batch ScanOneFile (tagged WAV) ==='
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('163lyric-smoke-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($tmp) | Out-Null
$wav = Join-Path $tmp '周杰伦 - 晴天.wav'
try {
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter($ms)
    function WriteAscii($bw, $s) { foreach ($ch in $s.ToCharArray()) { $bw.Write([byte][int]$ch) } }
    WriteAscii $bw 'RIFF'; $bw.Write([int](36 + 8000))
    WriteAscii $bw 'WAVE'
    WriteAscii $bw 'fmt '; $bw.Write([int]16); $bw.Write([int16]1); $bw.Write([int16]1)
    $bw.Write([int]8000); $bw.Write([int]16000); $bw.Write([int16]2); $bw.Write([int16]16)
    WriteAscii $bw 'data'; $bw.Write([int]8000)
    for ($i = 0; $i -lt 4000; $i++) { $bw.Write([int16]0) }
    $bw.Flush(); [IO.File]::WriteAllBytes($wav, $ms.ToArray()); $bw.Close()

    $tf = [TagLib.File]::Create($wav)
    $tf.Tag.Title = '晴天'
    $tf.Tag.Performers = [string[]]@('周杰伦')
    $tf.Tag.Album = '叶惠美'
    $tf.Save(); $tf.Dispose()
    Write-Host ("  wav created: " + (Test-Path $wav))

    $form = $asm.CreateInstance('_163lyric.Form1')
    $tForm = $asm.GetType('_163lyric.Form1')
    $m = $tForm.GetMethod('ScanOneFile', [System.Reflection.BindingFlags]'NonPublic,Instance')
    $listType = [System.Collections.Generic.List`1].MakeGenericType($asm.GetType('_163lyric.ILyricSource'))
    $sources = [Activator]::CreateInstance($listType)
    $add = $listType.GetMethod('Add')
    [void]$add.Invoke($sources, @($asm.CreateInstance('_163lyric.NetEaseSource')))
    [void]$add.Invoke($sources, @($asm.CreateInstance('_163lyric.KugouSource')))
    [void]$add.Invoke($sources, @($asm.CreateInstance('_163lyric.LrclibSource')))
    # PS 会把 string 包成 PSObject，.NET 反射不认，需解包
    $wavRaw = if ($wav -is [PSObject]) { $wav.psobject.BaseObject } else { $wav }
    $line = $m.Invoke($form, @($wavRaw, $sources, $true))
    Write-Host ("  LOG: " + $line)

    $lrc = [IO.Path]::ChangeExtension($wav, '.lrc')
    Write-Host ("  lrc exists=" + (Test-Path $lrc))
    if (Test-Path $lrc) {
        $bytes2 = [IO.File]::ReadAllBytes($lrc)
        Write-Host ("  BOM bytes: " + $bytes2[0] + "," + $bytes2[1] + "," + $bytes2[2] + "  (expect 239,187,191)")
        $content = [IO.File]::ReadAllText($lrc)
        Write-Host ("  head: " + $content.Substring(0, [Math]::Min(180, $content.Length)).Replace("`r`n", " / "))
    }
} catch { Write-Host ("  FAILED: " + $_.Exception.ToString()) }

Remove-Item -Recurse -Force $tmp
Write-Host '=== smoke done ==='
