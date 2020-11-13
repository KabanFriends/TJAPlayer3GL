### TJAPlayer3GL ###
v1.0.0 by KabanFriends


# このプロジェクトについて - ABOUT THIS PROJECT
TJAPlayer3GLをダウンロードいただき、ありがとうございます。 このプロジェクトは、DirectXをベースに動いているAioiLight氏制作の太鼓の達人シミュレーター「TJAPlayer3」の描画エンジンをOpenGLに、音声出力のDirectSoundをOpenALに移植するなど、TJAPlayer3をDirectXに依存しないようにすることが目的のプロジェクトです。
Thank you for downloading TJAPlayer3GL. This project is a modification of a DirectX-based Taiko no Tatsujin simulator "TJAPlayer3" by AioiLight, to port the rendering engine to OpenGL, and the audio engine DirectSound to OpenAL. The goal of this project is to make TJAPlayer3 not use DirectX at all.


# このソフトウェアの使い方 - HOW TO USE THIS SOFTWARE
ソフトウェアの起動方法、操作方法などはこのソフトウェアのベースとなっているTJAPlayer3と変わりません。 TJAPlayer3の詳しい使用方法については、各自で調べてください。
The way to start and operate the software is the same as TJAPlayer3, which is the base of this software. For more information on how to use TJAPlayer3, please check by yourself.


# 推奨される設定 - RECOMMENDED SETTINGS
OpenGL、OpenALの対応の影響により、オリジナル版のTJAPlayer3と比較した際、当ソフトウェアの使用に不便が生じる可能性があります。 (ソフトウェアのカクつき、メモリの使いすぎ等)
そのような問題が発声している場合は、ソフトウェア内の設定画面またはConfig.iniから、以下の設定を使用すると解決する場合があります。
Due to OpenGL and OpenAL porting, when compared to the original TJAPlayer3, you may encounter some perfomance issues. (e.g. Lag Spikes, High Memory Usage)
If such problems are happening to you, try settign these options in either ingame config screen, or the Config.ini file.

➝ FastRender - OFF
➝ VSyncWait - OFF
これらの設定により、ソフトウェアのカクつきが解決する可能性があります。
These settings may solve lag spike issues.

➝ SoundType - ASIO/WASAPI
OpenALの仕様上、再生中の音声のデータ量に応じてソフトウェアが使用するメモリの量が大きく変化します。
メモリ使用量を安定させたい方はOpenAL以外のサウンドデバイスを選択してください。
In OpenAL, the larger audio files will take up a lot of memory space.
If you want to keep the memory usage low, please switch to a sound device that is not OpenGL.


# 更新履歴 - CHANGELOG
TJAPlayer3GL v1.0.0 (2020/11/15)
- 最初のリリース / The first release of the software