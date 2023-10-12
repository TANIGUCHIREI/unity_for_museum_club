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

    public string user_input = "おまかせでお願いします。";
    public Dictionary<string, string> user_input_dict = new Dictionary<string, string>();

    public string userinput_text;
    public bool _is_kansai_only;


    void Start()
    {

        userinput_text = gameObject.GetComponent<User_Input>().userinput_text; //gameObject.GetComponent<>()は参照を返すため、その後の変更も反映されます。bygpt4
        _is_kansai_only = gameObject.GetComponent<User_Input>()._is_kansai_only;

        //接続処理。接続先サーバと、ポート番号を指定する
        ws = new WebSocket("ws://localhost:8001/");
        ws.Connect();
        OnSendPython();
        //送信ボタンが押されたときに実行する処理「SendText」を登録する
        //endButton.onClick.AddListener(SendText);

        //サーバからメッセージを受信したときに実行する処理「RecvText」を登録する
        ws.OnMessage += (sender, e) => RecvFromPython(e.RawData);
        //サーバとの接続が切れたときに実行する処理「RecvClose」を登録する
        ws.OnClose += (sender, e) => RecvClose();


    }


    public void OnSendPython()
    {
        string send_word = CreateInput();
        SendToPython(send_word);
    }

    public string CreateInput()
    {
        string input = "";
        if( userinput_text == "おまかせ"){
            input += "おまかせでお願いします。なにか面白い展示を教えてください。";
        }
        else
        {
            input += userinput_text;
        }
        if (_is_kansai_only)
        {
            input += "\nただ、場所は絶対に関西(大阪・兵庫・京都・奈良・和歌山・滋賀のどこか)でお願いします。";
        }
    
        return input;
    }
    public void SendToPython(string user_input)
    {
        //ws.Send(messageInput.text);
        user_input_dict = new Dictionary<string, string> { { "user_input", user_input } };
        string json = JsonConvert.SerializeObject(user_input_dict);
        byte[] json_bytes = Encoding.UTF8.GetBytes(json);
        ws.Send(json_bytes);
    }

    //サーバから受け取ったメッセージを、ChatTextに表示する
    public void RecvFromPython(byte[] msg)
    {
        var recieved_words_data = Encoding.UTF8.GetString(msg);
        Dictionary<string, object>  dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        //Debug.Log(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        if((string) dictionary["TYPE"] == "QUERY")
        {
            //Debug.Log("クエリだよ");
            //Debug.Log(dictionary["QUERY"].GetType());
            List<string> QUERY = (dictionary["QUERY"] as JArray).ToObject<List<string>>();
            foreach (var word in QUERY)
            {
                Debug.Log(word);
            }
            
        }else if ((string) dictionary["TYPE"] == "ANSWER")
        {
            string prefecture = (string) dictionary["prefecture"];
            string museum_name = (string)dictionary["museum_name"];
            string exhibition_name = (string)dictionary["exhibition_name"];
            string exhibition_reason = (string)dictionary["exhibition_reason"];
            Debug.Log(prefecture + " " + museum_name + " " + exhibition_name);
            Debug.Log(exhibition_reason);

        }
    }
    //サーバの接続が切れたときのメッセージを、ChatTextに表示する
    public void RecvClose()
    {
        Debug.Log("Closed");
    }

    
}