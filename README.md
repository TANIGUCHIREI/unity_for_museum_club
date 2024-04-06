# unity_for_museum_club

# 外部公開用に動作している様子を追加しました！
https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/c965ca57-05c9-43a0-9e70-7c544230229f

https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/7f29703d-d72b-478d-bf7d-42d7e1ae2f7c
上の２つ目の動画にて印刷中・・・と出ていますが、このときにGPT4からの回答からpdfを作成して下のような検索結果を自動的に印刷します。
![F-ey5F4XsAEW3hA](https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/bea80016-a753-4d2b-8fd2-d28d8dfe5c18)


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
