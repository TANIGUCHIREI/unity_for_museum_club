# unity_for_museum_club

まちかね祭り(大阪大学の秋の学祭)にて、所属しているミュージアム同好会のアプリを作成し出展いたしました。製作は研究室の先輩と二人で行い、サーバーサイド(pythonで実装)は二人で共同で、クライアントサイドのアプリケーション(Unityで実装、このリポジトリにてコードを公開)は私が行いました。

アプリの内容としては、来客者の方が興味のある言葉を入力すると、それを元に関西または全国からおすすめの展示を紹介してくれて、さらにおすすめした理由が自動で印刷され持って帰ることができる、というものになっています。

全国の展示データは部員により作成され(およそ300個)、それを基にtext-embedding→GPT4による厳選とおすすめ理由の記述　といった流れにて処理が行われています。

サーバーサイドの実装は[こちら](https://github.com/rana421/museum-machikane)にて公開しています。

Unityアプリ側では、子供から大人までの幅広い方に直感的に利用していただけて、またGPT4による回答の待ち時間を楽しんで頂けるようなギミックを追加することを心がけました。

さらにopenaiのwhisperを利用することで音声入力にも対応しています。

# 外部公開用に動作している様子を追加しました！

↓のような展示を行いました。３日間で述べ120人以上の方に利用していただき、学祭出展費を上回る利益を得ることが出来ました。

![F-DkODBXwAA-QKt](https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/7eae8bbf-59d4-4445-8b8a-85d015e265a8)

具体的なアプリの動作は以下のようになります（ファイルが大きいため２つに分けています）

https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/c965ca57-05c9-43a0-9e70-7c544230229f

https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/7f29703d-d72b-478d-bf7d-42d7e1ae2f7c

上の２つ目の動画にて印刷中・・・と出ていますが、このときにGPT4からの回答からpdfを作成して下のような検索結果を自動的に印刷します。
![F-ey5F4XsAEW3hA](https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/bea80016-a753-4d2b-8fd2-d28d8dfe5c18)

アプリ中に登場するミュージアム同好会のロゴ、昔懐かしいガチャガチャ・日本地図の3Dモデル、ガチャガチャに書かれているミュシャ風のチラシなども全て自作しました。（illustratorとphotoshop及びblenderを使用）

![スクリーンショット 2024-04-06 173130](https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/3483235a-a194-47d9-9961-9db51304cfca)

![スクリーンショット 2024-04-06 180848](https://github.com/TANIGUCHIREI/unity_for_museum_club/assets/120480219/b295955c-7af4-42f7-a7b8-d562440491a6)

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
