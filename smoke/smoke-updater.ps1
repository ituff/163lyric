# 关于窗口 + 更新器冒烟测试（反射调用，不真正执行替换脚本）
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$bin = 'D:\Project\163lyrics\163lyric\163lyric\bin\Release\net48'
$asm = [System.Reflection.Assembly]::LoadFrom((Join-Path $bin 'OneLyric.exe'))

Write-Host '=== 1. 嵌入资源（二维码） ==='
$res = $asm.GetManifestResourceNames()
Write-Host ("  resources: " + ($res -join ', '))

Write-Host '=== 2. 版本比较 ==='
$upd = $asm.GetType('_163lyric.Updater')
$mNewer = $upd.GetMethod('IsNewer')
Write-Host ("  CurrentVersion = " + $upd.GetProperty('CurrentVersion').GetValue($null))
Write-Host ("  IsNewer('v1.1') = " + $mNewer.Invoke($null, @('v1.1')) + "  (expect False)")
Write-Host ("  IsNewer('v1.3') = " + $mNewer.Invoke($null, @('v1.3')) + "  (expect True)")
Write-Host ("  IsNewer('garbage') = " + $mNewer.Invoke($null, @('garbage')) + "  (expect False)")

Write-Host '=== 3. GitHub Releases API ==='
$mGet = $upd.GetMethod('GetLatestRelease')
$rel = $mGet.Invoke($null, $null)
Write-Host ("  latest tag=" + $rel.Tag + "  zip=" + $rel.ZipUrl)

Write-Host '=== 4. 下载+解压+脚本生成（不执行） ==='
$mDl = $upd.GetMethod('DownloadAndPrepareUpdate')
$scriptPath = $mDl.Invoke($null, @($rel.ZipUrl))
Write-Host ("  script: " + $scriptPath + "  exists=" + (Test-Path $scriptPath))
$workDir = Split-Path -Parent $scriptPath
$newDir = Join-Path $workDir 'new'
Write-Host ("  extracted files: " + ((Get-ChildItem $newDir | ForEach-Object { $_.Name }) -join ', '))
Write-Host '--- update.cmd ---'
Get-Content $scriptPath | ForEach-Object { Write-Host ("  | " + $_) }

Write-Host '=== 5. AboutForm 实例化（含二维码加载） ==='
$form = $asm.CreateInstance('_163lyric.AboutForm')
Write-Host ("  AboutForm created: " + ($form -ne $null))
$form.Dispose()

# 清理测试产物
Remove-Item -Recurse -Force $workDir
Write-Host '=== smoke done ==='
