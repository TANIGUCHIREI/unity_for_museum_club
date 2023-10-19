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

    public  string IPAdress = "127.0.0.1"; //この辺はSettingMenuにて代わります
    public  string Port = "8001";

    public GameObject menu1_Blined_Panel;
    public TMP_Text Status_Text;

    void Start()
    {
        Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        //Status_Text.text = "";
        menu1_Blined_Panel = GameObject.Find("menu1_Blined_Panel");
        menu1_Blined_Panel.GetComponent<Image>().raycastTarget = true; //はじめはメニューが動かないように
        async_SendPython(Type :"Init_Connection");




    }


    public void OnButtonClick()
    {
        async_SendPython(); //ラッパー関数を使ってasyncをバックグラウンドで動作させる。これで動作がなめらかになる
    }


    public async void async_SendPython(string Type = null)
    {
        if(Type == "Init_Connection")
        {
            //メニュー１にて（つまり初回起動時とか）ws connectできるかどうかの確認

            bool _is_ConnectSuccess = await Init_Test_CoMwithPython(); //初回画面にて接続に成功したら次の画面に行ける、ムリポなら設定しかいけない

            if (_is_ConnectSuccess)
            {
                menu1_Blined_Panel.GetComponent<Image>().raycastTarget = false; //接続に成功したら次の画面に行けるようにする！
            }
        }
        else{
            string send_word = CreateInput();
            await SendToPython(send_word);
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
            //Debug.Log("クエリだよ");
            //Debug.Log(dictionary["QUERY"].GetType());
            List<string> QUERY = (recv_dict["QUERY"] as JArray).ToObject<List<string>>();
            foreach (var word in QUERY)
            {
                Debug.Log(word);
            }
            
        }else if (Recv_Type == "ANSWER")
        {
            string prefecture = (string) recv_dict["prefecture"];
            string museum_name = (string)recv_dict["museum_name"];
            string exhibition_name = (string)recv_dict["exhibition_name"];
            string exhibition_reason = (string)recv_dict["exhibition_reason"];
            Debug.Log(prefecture + " " + museum_name + " " + exhibition_name);
            Debug.Log(exhibition_reason);

        }else if (Recv_Type == "COM_TEST")
        {
            string Response_from_Python = (string)recv_dict["RESPONSE"]; //表示はここ（別スレッド）ではできませんので変更して、Update内で表示を行っている
            Debug.Log(Response_from_Python);
        }
    }
    //サーバの接続が切れたときのメッセージを、ChatTextに表示する
    public void RecvClose()
    {
        Debug.Log("Closed");
    }

    
}