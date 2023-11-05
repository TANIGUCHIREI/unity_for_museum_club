# unity_for_museum_club

## gitでunityを管理する場合は以下のURLが参考になりました！
https://styly.cc/ja/tips/michi_unity_github/
全体をgit add とかするのはしんどいため、sourcetreeというソフトを使用しています。


## 仕組み解説
### 超重要スクリプト（すべてのシーンで利用）
#### ClientManager
・websocketサーバでpythonと相互通信するために使用。
wavデータを分割して送信するSendWavFileもこの中にあります
#### change_walls
・シーン遷移時に動く壁？を動作させます。
遷移前後の設定等の引き継ぎやGameObject.Findもこの中で行います
bgm操作もこの中で

### 重要スクリプト（部分的に必須機能）
#### Setting_Menu
・通信テストとか音量調整とか。Find多用しまくりのやつ。wsをmenuのやつとは別に新規作成して使ってます
