#网易云音乐歌词下载器

网易音乐是目前最好用的音乐播放器，很多朋友喜欢从上面下载免费的音乐，但是可惜没法下载到对应的歌词。

这个小工具就是为你解决这个小问题的。

## 使用方法

1、首先在网易云音乐网页版上搜到歌曲，复制地址栏里歌曲的ID（例如 `https://music.163.com/#/song?id=30953009` 中的 `30953009`）；

2、把ID复制进小工具，点击 GET 即可获取歌词；

3、获取到的歌词会自动复制到剪贴板，也可以点击 COPY 再复制一次。

如果歌曲有翻译（比如英文歌的中文翻译），获取时会自动把翻译歌词按时间轴合并到原文歌词的每一行下面，与网易云音乐客户端的显示效果一致。

## 下载地址

最新版本：https://github.com/ituff/163lyric/releases

源代码：https://github.com/ituff/163lyric

## 构建方法

安装任意较新的 .NET SDK（6.0+）后，在仓库根目录执行：

```
dotnet build 163lyric/163lyric -c Release
```

产物在 `163lyric/163lyric/bin/Release/net48/163lyric.exe`。

注意：该工具需要 .NET Framework 4.8 支持（Windows 10 1903 及以上 / Windows 11 系统自带）。
