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
    //�Q�l�ɂ����T�C�g�ł�
    //https://www.create-forever.games/unity-json-net/
    //newtonsoft json�̃C���X�g�[���̂���
    public WebSocket ws;
    //public Text chatText;
    //public Button sendButton;
    //public InputField messageInput;

    public Dictionary<string, string> user_input_dict = new Dictionary<string, string>();

    public string userinput_text;
    public bool _is_kansai_only =true;

    public  string IPAdress = "127.0.0.1"; //���̕ӂ�SettingMenu�ɂđ���܂�
    public  string Port = "8001";

    void Start()
    {
   

        //�ڑ������B�ڑ���T�[�o�ƁA�|�[�g�ԍ����w�肷��
        //���M�{�^���������ꂽ�Ƃ��Ɏ��s���鏈���uSendText�v��o�^����
        //endButton.onClick.AddListener(SendText);

        //�T�[�o���烁�b�Z�[�W����M�����Ƃ��Ɏ��s���鏈���uRecvText�v��o�^����
        

    }

    
    
    public void OnSendPython()
    {
        string send_word = CreateInput();
        SendToPython(send_word);
    }

    public string CreateInput()
    {
        string input = "";
        if( userinput_text == "���܂���"){
            input += "���܂����ł��肢���܂��B�Ȃɂ��ʔ����W���������Ă��������B";
        }
        else
        {
            input = userinput_text;
        }


        return input;
    }
    public void SendToPython(string user_input)

    {
        Debug.Log("try to connetct to " + "ws://" + IPAdress + ":" + Port + "/");
        ws = new WebSocket("ws://" + IPAdress + ":" + Port + "/");
        ws.OnMessage += (sender, e) => RecvFromPython(e.RawData);
        //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
        ws.OnClose += (sender, e) => RecvClose();

        ws.Connect();
        user_input_dict = new Dictionary<string, string> { { "TYPE", "USER_INPUT" } ,{ "user_input", user_input },{ "_is_kansai_only",_is_kansai_only.ToString() } };
        string json = JsonConvert.SerializeObject(user_input_dict);
        byte[] json_bytes = Encoding.UTF8.GetBytes(json);
        ws.Send(json_bytes);
    }

    //�T�[�o����󂯎�������b�Z�[�W���AChatText�ɕ\������
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
            //Debug.Log("�N�G������");
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
            string Response_from_Python = (string)recv_dict["RESPONSE"]; //�\���͂����i�ʃX���b�h�j�ł͂ł��܂���̂ŕύX���āAUpdate���ŕ\�����s���Ă���
            Debug.Log(Response_from_Python);
        }
    }
    //�T�[�o�̐ڑ����؂ꂽ�Ƃ��̃��b�Z�[�W���AChatText�ɕ\������
    public void RecvClose()
    {
        Debug.Log("Closed");
    }

    
}