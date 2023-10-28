using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.IO;
using System.Text;
using WebSocketSharp.Net;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks; //非同期にwebsocketでつうしんを行うためのもの
using TMPro;
using System;

public class ClientManager : MonoBehaviour
{
    //https://qiita.com/riyosy/items/5789ccdeee644b34a743
    //参考にしたサイトです
    //https://www.create-forever.games/unity-json-net/
    //newtonsoft jsonのインストールのやり方
    public WebSocket ws;
    //public Text chatText;
    //public Button sendButton;
    //public InputField messageInput;

    public Dictionary<string, string> user_input_dict = new Dictionary<string, string>();

    public string userinput_text;
    public bool _is_kansai_only =true;

    public  string IPAdress = "192.168.100.116"; //この辺はSettingMenuにて代わります
    public  string Port = "8001";

    public GameObject menu1_Blined_Panel; //ここの設定された値は、change_wall.cs内でシーン遷移時に設定側に反映されるように設定しています。逆もしかり？
    public TMP_Text Status_Text;
    public GameObject Change_walls_UI;
    public bool _isStandAloneModeOne = false; //これがOnであるときは通信がいらない、というかテストモードになる

    public List<string> QUERY_Buffer = new List<string>(); //これは別スレッドのWebsocketClientからこれを操作し、さらにメインスレッドのここでGetcompomentでGachatControllerを操作するようのやつ
    public bool _isQueryArrive = false;
    public bool _isAnserArrive = false;

    public string prefecture;
    public string museum_name;
    public string exhibition_name;
    public string exhibition_reason;

    public bool _isPrintFinish = false;

    public bool _isAudio_Input_Converted_Arrive = false;

    private bool _is_Coneection_Closed = false;
    private bool Doing_InitConnection_test = false;

    void Start()
    {

        Start_func();

    }

    private void FixedUpdate()
    {
        if (_isQueryArrive)
        {

            Debug.Log("ClientManagerからGachaControllerに送信");
            GameObject Gacha_Event = GameObject.Find("Gacha_Event");
            Gacha_Event.GetComponent<Gacha_Controller>().QUERY = QUERY_Buffer;
            Gacha_Event.GetComponent<Gacha_Controller>()._isQueryArrive = true;
            _isQueryArrive = false;
            //↑についてhttps://mono-pro.net/archives/9029が参考になりました
        }

        if (_isAnserArrive)
        {
            GameObject change_wall = GameObject.Find("Change_walls_UI");
            change_wall.GetComponent<change_wall>()._isAnserArrive = true;
            //change_wallsにシーン遷移してもいいことを伝える
            _isAnserArrive = false;
        }

        if (_isPrintFinish)
        {
            //リザルト画面になっている前提で処理しています、そうでないと_isPrintFinish = trueにはならないから！
            Debug.Log("Print Finish!!");

            GameObject Result_EventSystem = GameObject.Find("Result_EventSystem");
            Result_EventSystem.GetComponent<change_result_panel>().StartCoroutine(Result_EventSystem.GetComponent<change_result_panel>().menu_move(speed:1000));
            _isPrintFinish = false;
        }

        if (_isAudio_Input_Converted_Arrive)
        {
            Debug.Log("音声入力が変換されました！ : " + userinput_text);
            GameObject menu_EventSystem = GameObject.Find("EventSystem");
            GameObject InputField = GameObject.Find("InputField (TMP)");
            GameObject Record_Button = GameObject.Find("Record_Button"); //「変換中です・・・」のアニメーションを停止させるために必要
            Record_Button.GetComponent<VoiceRecorder>()._is_convert_to_text_finish = true;
            InputField.GetComponent<TMP_InputField>().text = userinput_text;
            menu_EventSystem.GetComponent<menu>().OnRecordingBackOn(); //ポップアップを戻す
            
            _isAudio_Input_Converted_Arrive = false;
        }

        if(_is_Coneection_Closed && !_isPrintFinish && !Doing_InitConnection_test)
        {
            //コネクションが切れたが印刷が終わっていない（つまりラスト画面に達していないのに接続が切れた場合）は以上なのでタイトル画面に戻る
            //またタイトル画面でのコネクションエラーは仕様なので無視する
            StartCoroutine(Change_walls_UI.GetComponent<change_wall>().Connection_Error_and_change_to_menu());
            _is_Coneection_Closed = false;
        }
    }

    
    public void Start_func()
    {
        Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        //Status_Text.text = "";
        menu1_Blined_Panel = GameObject.Find("menu1_Blined_Panel");
        menu1_Blined_Panel.GetComponent<Image>().raycastTarget = true; //はじめはメニューが動かないように

        if (_isStandAloneModeOne)
        {
            //通信をしないモード
            menu1_Blined_Panel.GetComponent<Image>().raycastTarget = false; //接続に成功したら次の画面に行けるようにする！
            Status_Text.text = "Stand Alone Mode";

        }
        else
        {
            async_SendPython(Type: "Init_Connection");
        }
    }
    public void OnButtonClick()
    {
   
         async_SendPython(); //ラッパー関数を使ってasyncをバックグラウンドで動作させる。これで動作がなめらかになる
   
    }


    public async void async_SendPython(string Type = null)
    {

        if (_isStandAloneModeOne == false)
        {
            //スタンドアロンモードならこの先の操作は実行しない！
            if (Type == "Init_Connection")
            {
                //メニュー１にて（つまり初回起動時とか）ws connectできるかどうかの確認

                Doing_InitConnection_test = true; //初回の接続テストをしていることを全体掲示
                bool _is_ConnectSuccess = await Init_Test_CoMwithPython(); //初回画面にて接続に成功したら次の画面に行ける、ムリポなら設定しかいけない

                if (_is_ConnectSuccess)
                {
                    menu1_Blined_Panel.GetComponent<Image>().raycastTarget = false; //接続に成功したら次の画面に行けるようにする！
                }

                await Task.Delay(100); // 0.1秒待つ

                Doing_InitConnection_test = false;
            }
            else
            {
                string send_word = CreateInput();
                await SendToPython(send_word);
            }
        }
        
        
    }

    public string CreateInput()
    {
        string input = "";
        if( userinput_text == "おまかせ"){
            input += "おまかせでお願いします。なにか面白い展示を教えてください。";
        }
        else
        {
            input = userinput_text;
        }


        return input;
    }
    public async Task SendToPython(string user_input)

    {


        Debug.Log("try to connetct to " + "ws://" + IPAdress + ":" + Port + "/");
        //ws = new WebSocket("ws://" + IPAdress + ":" + Port + "/");
        //ws.OnMessage += (sender, e) => RecvFromPython(e.RawData);
        //サーバとの接続が切れたときに実行する処理「RecvClose」を登録する
        //ws.OnClose += (sender, e) => RecvClose();

        await Task.Run(() =>  ws.Connect());
        user_input_dict = new Dictionary<string, string> { { "TYPE", "USER_INPUT" } ,{ "user_input", user_input },{ "_is_kansai_only",_is_kansai_only.ToString() } };
        string json = JsonConvert.SerializeObject(user_input_dict);
        byte[] json_bytes = Encoding.UTF8.GetBytes(json);
        ws.Send(json_bytes);
    }

    


    public async Task<bool> Init_Test_CoMwithPython()
    {
        Debug.Log("Init_Test_COmwithPython が動作を開始しました");

        try
        {
            //ws.Close(); //はじめにあったやつを消す
            Debug.Log("trying to connetct to " + "ws://" + IPAdress + ":" + Port + "/");
            ws = new WebSocket("ws://" + IPAdress + ":" + Port + "/");
            //サーバからメッセージを受信したときに実行する処理「RecvText」を登録する
            ws.OnMessage += (sender, e) => RecvFromPython(e.RawData);
            //サーバとの接続が切れたときに実行する処理「RecvClose」を登録する
            ws.OnClose += (sender, e) => RecvClose();

            //↑はwsを新しく作るたびに定義する必要があるのではないか？
            await Task.Run(() => ws.Connect()); // ここを非同期に変更

            user_input_dict = new Dictionary<string, string> { { "TYPE", "COM_TEST" } };
            string json = JsonConvert.SerializeObject(user_input_dict);
            byte[] json_bytes = Encoding.UTF8.GetBytes(json);
            ws.Send(json_bytes);
            Debug.Log("Sent a Message");
            Status_Text.text = "";
            return true;
        }
        catch (System.InvalidOperationException e)
        {
            Debug.Log("接続先がオープンされていません！");
            Status_Text.text = "Cannot Connect To the Server ! \nPlease Check the Setting";
            return false;

        }


    }

    //サーバから受け取ったメッセージを、ChatTextに表示する
    public void RecvFromPython(byte[] msg)
    {
        var recieved_words_data = Encoding.UTF8.GetString(msg);
        Dictionary<string, object>  recv_dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        //Debug.Log(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        string Recv_Type = (string)recv_dict["TYPE"];

        if(Recv_Type == "QUERY")
        {
            Debug.Log("クエリだよ");
            //Debug.Log(dictionary["QUERY"].GetType());
            List<string> QUERY = (recv_dict["QUERY"] as JArray).ToObject<List<string>>();

            //此処から先はGachaシーンに移行していることを念頭に考えている
            
            foreach (var word in QUERY)
            {
                Debug.Log(word);
            }
            
            Debug.Log("クエリClientManager受信");
            QUERY_Buffer = QUERY; //まずはバッファーに保存する
            _isQueryArrive = true;
            //わかった・・・・コルーチンはここ（別スレッド）じゃきどうできないわけだアホ＆GetComponentのようなUnityコンポーネントやAPIを扱うことは別スレッドじゃ無理！




        }
        else if (Recv_Type == "ANSWER")
        {
            _isAnserArrive = true; //Arriveしたことを告げる
            prefecture = (string) recv_dict["prefecture"];
            museum_name = (string)recv_dict["museum_name"];
            exhibition_name = (string)recv_dict["exhibition_name"];
            exhibition_reason = (string)recv_dict["exhibition_reason"];
            Debug.Log(prefecture + " " + museum_name + " " + exhibition_name);
            Debug.Log(exhibition_reason);

        }else if (Recv_Type == "COM_TEST")
        {
            string Response_from_Python = (string)recv_dict["RESPONSE"]; //表示はここ（別スレッド）ではできませんので変更して、Update内で表示を行っている
            Debug.Log(Response_from_Python);
        }
        else if (Recv_Type == "PRINT_FINISH")
        {
            Debug.Log("プリントが終了されました！websocketをクローズします");
            _isPrintFinish = true;
            ws.Close();
        }else if (Recv_Type == "AUDIO")
        {
            userinput_text = (string)recv_dict["user_input"]; //音声から変換された入力テキストを受信している
            _isAudio_Input_Converted_Arrive = true;
        }
    }
    //サーバの接続が切れたときのメッセージを、ChatTextに表示する
    public void RecvClose()
    {
        Debug.Log("Closed");
        ws.Close();
        if (!Doing_InitConnection_test)
        {
            _is_Coneection_Closed = true; //初回接続時はこれ（エラー処理用のもの）は必要ない
        }
        
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        if (!_isStandAloneModeOne)
        {
            ws.Close(); //Close処理をアプリが終了するときにやる
        }
    }


    public IEnumerator SendWavFile(byte[] wavData, WebSocket ws, int chunkSize = 4096)
    {
        //GPT4が出力した音声データをチャンクに分けて送信する方法
        // 全体のチャンク数を計算
        int totalChunks = (int)Math.Ceiling((double)wavData.Length / chunkSize);

        for (int i = 0; i < totalChunks; i++)
        {
            int start = i * chunkSize;
            int end = Math.Min(start + chunkSize, wavData.Length);
            byte[] chunk = new byte[end - start];
            Array.Copy(wavData, start, chunk, 0, end - start);

            // チャンクを送信
            ws.Send(chunk);

            // 短い待機時間を追加（サーバーがデータを受信・処理する時間を確保）
            yield return new WaitForSeconds(0.1f); // 0.1秒待機。必要に応じて調整可能
        }

        ws.Send(Encoding.UTF8.GetBytes("")); //松原さんのws_client_test.pyを参考に、空のバイナリデータを送ることで処理が終了した・・・！！！
    }


}