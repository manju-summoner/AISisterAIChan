===============================================================
SHIOLINK.DLL：栞プロキシDLL
===============================================================

■SHIOLINKとは？
匿名PIPE通信を利用した子プロセスとの通信プロキシSHIORIです。起
動時に指定されたexeファイルを別プロセスで起動し、本体から送られ
るSHIORI REQUESTを起動したexeの標準入出力へのやり取りに変換し、
その結果を本体に返します。


■ライセンス
本ソフトウェアのghost部分に関しては、修正BSDライセンスとします。
現在のshellについては、フリーシェル配布者のライセンスに従ってく
ださい。

・修正BSDライセンス日本語訳
http://sourceforge.jp/projects/opensource/wiki/licenses%2Fnew_BSD_license


■著作（敬称略）
dll および ghost   : どっとステーション
shell              : 白兎一哉（フリーシェル「自由なみかず」）
使用ライブラリ等については「ビルドについて」を参照。


■配布ファイル
サンプルゴースト(動作には.Net Framework 2.0が必要です)


■ディレクトリ構成
[GHOST-DIR]
├─[ghost]
│　　　[master]
│　　　　├─descript.txt　：SHIORI設定記述ファイル
│　　　　├─SHIOLINK.dll　：プロキシSHIORI DLL
│　　　　├─SHIOLINK.ini　：プロキシSHIORIのINIファイル
│　　　　│　　　　　　　　　(DLL名の拡張子を.iniにしたもの)
│　　　　└─LINKSHIO.exe　：プロキシ栞が呼び出すexe
│　　　　　　　　　　　　　　(サンプル。要.Net Framework 2.0)
└─[shell]
　　　...


■設定ファイルの記述
SHIOLINK.ini
*--------------------------------------------------------------
[SHIOLINK]
; コマンドライン:
commandline = cscript /Nologo LINKJS.js

; 文字モード: ANSI(Shift_JIS) / UTF-8 の何れか
charmode  = ANSI

[LOGGING]
; ログファイル名:
file  = SHIOLINK.log

; ログレベル: debug / info / warn / error / fatal / none
level = debug
*--------------------------------------------------------------


■起動されるexeの環境
SHIOLINK.dllは指定されたexeなどを起動しますが、起動に先立ち次
の設定を行います。

    * カレントディレクトリを[/ghost/master]ディレクトリに
    * exeのSTDIN/STDOUTに、SHIOLINK.dllの匿名パイプをつなぐ

つまり、exe側は、[/ghost/master]ディレクトリで起動されたと仮定
し、標準入出力を相手にコマンド処理を随時行えばいいことになりま
す。実装試験などの場合は標準入力に対し適当にテキストを投入して
やることで、試験を行うことができます。

# >> LINKSHIO.exe < request.txt > response.txt
#   *これで、response.txtに結果が返る


■通信プロトコル
本システムの文字エンコードはUTF-8/ANSI(SHIFT-JIS)の選択です。

SHIORI REQUESTに、同期処理用のコマンドを若干追加しています。

例) --> 栞プロキシのrequest
    <-- exe側のresponse
*--------------------------------------------------------------
1 | --> *L:C:\[省略]\ghost\SHIOLINK\ghost\master
  | <-- [応答なし]
  |
*--------------------------------------------------------------
2 | --> *S:F6BA264E-AEA9-4886-B291-01C4664C504D
  |
*--------------------------------------------------------------
3 | --> *S:F6BA264E-AEA9-4886-B291-01C4664C504D
  |
*--------------------------------------------------------------
4 | --> GET SHIORI/3.0
  |     Sender: SSP
  |     Charset: UTF-8
  |     SecurityLevel: local
  |     ID: OnBoot
  |     Reference0: しおLINK！
  |     [空行]
  |
*--------------------------------------------------------------
5 | <-- SHIORI/3.0 200 OK
  |     Charset: UTF-8
  |     Sender: LINKSHIO
  |     Value: \u\s[10]\h\s[0]さて、[～省略～]\-
  |     [空行]
  |
*--------------------------------------------------------------
6 | --> *U:
  | <-- [応答なし、終了]
*--------------------------------------------------------------


■流れ
すべてのやり取りは改行で区切られます。*で始まる行が同期コマン
ドです。*で始まる、exe側が知らないコマンドが流れてきた場合はす
べて読み捨ててください。

   1. [*L:]コマンドは[SHIORI::load]命令に相当します。起動時に
      [*L:(loaddir)]と送られます。このコマンドに対する応答はあ
      りません。

   2. [*S:]コマンドは[SHIORI::request]命令の同期のために発行さ
      れます。[*S:(GUID)]となっています。

   3. exe側は、来た[*S:]コマンドの内容をそのままオウム返しして
      ください。SHIOLINK.DLLはこの応答を待って[SHIORI::request]
      本体を送信します。

   4. [SHIORI::request]本体です。空行までがリクエストです。
      exe側は空行が来るまでテキストを読み込んでください。

   5. [SHIORI::request]に対するレスポンスです。空行までがレスポ
      ンス扱いとなります。

   6. [*U:]コマンドは[SHIORI::unload]命令に相当します。exe側は終
      了処理を行い、直ちに終了してください。このコマンドに対する
      応答はありません。


■ビルドについて
SHIOLINK.DLLでは、以下のライブラリを利用しています。

・POCO C++ Libraries
  http://www.pocoproject.org/

・INIMONI C++用iniファイル読み書きテンプレート作成ソフト
  http://www18.ocn.ne.jp/~amedas/inimoni/

・RLOG（）
  http://hogeinstein.blog93.fc2.com/blog-entry-109.html


POCOに関してはincludeパス、libパスに含まれている必要があります。

