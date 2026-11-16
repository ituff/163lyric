# 163lyric logo 生成器：从同一份几何定义产出 SVG（设计源文件）、
# 多尺寸 PNG（README/展示用）和 app.ico（exe/窗口图标，内嵌 PNG 格式条目，Win10+）。
# 生成物：assets/logo.svg、assets/logo-{256,64,32,16}.png、163lyric/163lyric/app.ico
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path   # assets/
$repo = Split-Path -Parent $root

# ============ 几何定义（归一化 0..1，SVG 与 GDI+ 共用同一份数字） ============
$G = @{
    radius = 0.21
    top    = '#E6353D'   # 网易红，上浅
    bottom = '#B00A10'   # 下深
    head   = @{ cx = 0.235; cy = 0.665; w = 0.27; h = 0.195; rot = -18 }   # 音符符头
    stem   = @{ x = 0.352; y = 0.215; w = 0.043; h = 0.465 }               # 符干
    flag   = @{ p0 = @(0.395, 0.215); c1 = @(0.505, 0.225); c2 = @(0.575, 0.300); p1 = @(0.570, 0.405)
                d1 = @(0.485, 0.375); d2 = @(0.452, 0.315); p2 = @(0.395, 0.330) }  # 符尾
    lines  = @(   # 三条歌词行（透明度递减做出韵律）
        @{ x = 0.66; y = 0.335; w = 0.24; h = 0.05; a = 255 },
        @{ x = 0.66; y = 0.490; w = 0.19; h = 0.05; a = 222 },
        @{ x = 0.66; y = 0.645; w = 0.14; h = 0.05; a = 185 }
    )
}

function Format-Num([double]$v) {
    $v.ToString('0.##', [Globalization.CultureInfo]::InvariantCulture)
}

# ============ SVG（设计源文件） ============
function New-Svg($G, [int]$canvas = 256) {
    $hx  = Format-Num ($G.head.cx * $canvas)
    $hy  = Format-Num ($G.head.cy * $canvas)
    $hrx = Format-Num ($G.head.w * $canvas / 2)
    $hry = Format-Num ($G.head.h * $canvas / 2)
    $rot = $G.head.rot

    $sx = Format-Num ($G.stem.x * $canvas)
    $sy = Format-Num ($G.stem.y * $canvas)
    $sw = Format-Num ($G.stem.w * $canvas)
    $sh = Format-Num ($G.stem.h * $canvas)

    $fx = { param($p) '{0} {1}' -f (Format-Num ($p[0] * $canvas)), (Format-Num ($p[1] * $canvas)) }
    $flag = ('M {0} C {1}, {2}, {3} C {4}, {5}, {6} Z' -f @(
        (& $fx $G.flag.p0),
        (& $fx $G.flag.c1), (& $fx $G.flag.c2), (& $fx $G.flag.p1),
        (& $fx $G.flag.d1), (& $fx $G.flag.d2), (& $fx $G.flag.p2)))

    $linesSvg = ($G.lines | ForEach-Object {
        $op = Format-Num ([math]::Round($_.a / 255.0, 3))
        '    <rect x="{0}" y="{1}" width="{2}" height="{3}" rx="{4}" opacity="{5}"/>' -f @(
            (Format-Num ($_.x * $canvas)), (Format-Num ($_.y * $canvas)),
            (Format-Num ($_.w * $canvas)), (Format-Num ($_.h * $canvas)),
            (Format-Num ($_.h * $canvas / 2)), $op)
    }) -join "`n"

    $r = Format-Num ($G.radius * $canvas)
@"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 $canvas $canvas" width="$canvas" height="$canvas">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0" stop-color="$($G.top)"/>
      <stop offset="1" stop-color="$($G.bottom)"/>
    </linearGradient>
  </defs>
  <rect x="0" y="0" width="$canvas" height="$canvas" rx="$r" fill="url(#bg)"/>
  <g fill="#ffffff">
    <ellipse cx="$hx" cy="$hy" rx="$hrx" ry="$hry" transform="rotate($rot $hx $hy)"/>
    <rect x="$sx" y="$sy" width="$sw" height="$sh"/>
    <path d="$flag"/>
    <g>
$linesSvg
    </g>
  </g>
</svg>
"@
}

# ============ GDI+ 渲染（2048 超采样后高质量缩小） ============
function New-RoundedPath([float]$x, [float]$y, [float]$w, [float]$h, [float]$r) {
    $p = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = 2 * $r
    $p.AddArc($x, $y, $d, $d, 180, 90)
    $p.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
    $p.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
    $p.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
    $p.CloseFigure()
    $p
}

function New-LogoBitmap($G, [int]$S = 2048) {
    # 注意：PowerShell 变量名不区分大小写，绘图对象不能叫 $g，否则会覆盖参数 $G
    $bmp = New-Object System.Drawing.Bitmap($S, $S)
    $gfx = [System.Drawing.Graphics]::FromImage($bmp)
    $gfx.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    # 背景圆角矩形 + 垂直渐变
    $rr = New-RoundedPath 0 0 $S $S ($G.radius * $S)
    $rect = New-Object System.Drawing.Rectangle(0, 0, $S, $S)
    $ctop = [System.Drawing.ColorTranslator]::FromHtml($G.top)
    $cbot = [System.Drawing.ColorTranslator]::FromHtml($G.bottom)
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, $ctop, $cbot, 90)
    $gfx.FillPath($brush, $rr)
    $brush.Dispose(); $rr.Dispose()

    $white = [System.Drawing.Brushes]::White
    $hw = [float]($G.head.w * $S); $hh = [float]($G.head.h * $S)

    # 符头（旋转椭圆）
    $gstate = $gfx.Save()
    $gfx.TranslateTransform([float]($G.head.cx * $S), [float]($G.head.cy * $S))
    $gfx.RotateTransform($G.head.rot)
    $gfx.FillEllipse($white, -$hw / 2, -$hh / 2, $hw, $hh)
    $gfx.Restore($gstate)

    # 符干
    $gfx.FillRectangle($white, [float]($G.stem.x * $S), [float]($G.stem.y * $S), [float]($G.stem.w * $S), [float]($G.stem.h * $S))

    # 符尾（两段贝塞尔闭合）
    $pt = { param($p) New-Object System.Drawing.PointF([float]($p[0] * $S), [float]($p[1] * $S)) }
    $p0 = & $pt $G.flag.p0; $c1 = & $pt $G.flag.c1; $c2 = & $pt $G.flag.c2; $p1 = & $pt $G.flag.p1
    $d1 = & $pt $G.flag.d1; $d2 = & $pt $G.flag.d2; $p2 = & $pt $G.flag.p2
    $flagPath = New-Object System.Drawing.Drawing2D.GraphicsPath
    $flagPath.AddBezier($p0, $c1, $c2, $p1)
    $flagPath.AddBezier($p1, $d1, $d2, $p2)
    $flagPath.CloseFigure()
    $gfx.FillPath($white, $flagPath)
    $flagPath.Dispose()

    # 歌词行（圆角胶囊，透明度递减）
    foreach ($ln in $G.lines) {
        $lbrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb($ln.a, 255, 255, 255))
        $lp = New-RoundedPath ([float]($ln.x * $S)) ([float]($ln.y * $S)) ([float]($ln.w * $S)) ([float]($ln.h * $S)) ([float]($ln.h * $S / 2))
        $gfx.FillPath($lbrush, $lp)
        $lbrush.Dispose(); $lp.Dispose()
    }

    $gfx.Dispose()
    $bmp
}

function Resize-Bitmap([System.Drawing.Bitmap]$src, [int]$size) {
    $out = New-Object System.Drawing.Bitmap($size, $size)
    $g = [System.Drawing.Graphics]::FromImage($out)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.DrawImage($src, 0, 0, $size, $size)
    $g.Dispose()
    $out
}

function Get-PngBytes([System.Drawing.Bitmap]$bmp) {
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    , $ms.ToArray()
}

# ICO 封装：ICONDIR + ICONDIRENTRY×N + PNG 载荷（Windows Vista+ 支持 PNG 条目）
function Save-Ico([object[]]$items, [string]$path) {
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter($ms)
    $bw.Write([uint16]0); $bw.Write([uint16]1); $bw.Write([uint16]$items.Count)
    $offset = 6 + 16 * $items.Count
    foreach ($it in $items) {
        $dim = if ($it.size -ge 256) { 0 } else { $it.size }
        $bw.Write([byte]$dim); $bw.Write([byte]$dim); $bw.Write([byte]0); $bw.Write([byte]0)
        $bw.Write([uint16]1); $bw.Write([uint16]32)
        $bw.Write([uint32]$it.bytes.Length); $bw.Write([uint32]$offset)
        $offset += $it.bytes.Length
    }
    foreach ($it in $items) { $bw.Write([byte[]]$it.bytes) }
    $bw.Flush()
    [IO.File]::WriteAllBytes($path, $ms.ToArray())
    $bw.Close()
}

# ============ 产出 ============
$logo = New-LogoBitmap $G 2048

New-Svg $G 256 | Set-Content -Path (Join-Path $root 'logo.svg') -Encoding UTF8

# 展示用 PNG：256/64/32/16
foreach ($s in 256, 64, 32, 16) {
    $small = Resize-Bitmap $logo $s
    $small.Save((Join-Path $root ("logo-$s.png")), [System.Drawing.Imaging.ImageFormat]::Png)
    $small.Dispose()
}

# ico 用 256/48/32/16（48 只进 ico）
$icoItems = @()
foreach ($s in 256, 48, 32, 16) {
    $small = Resize-Bitmap $logo $s
    $icoItems += @{ size = $s; bytes = (Get-PngBytes $small) }
    $small.Dispose()
}
Save-Ico $icoItems (Join-Path $repo '163lyric\163lyric\app.ico')

$logo.Dispose()
Write-Host 'logo 生成完成：assets/logo.svg + logo-{256,64,32,16}.png + 163lyric/163lyric/app.ico'
