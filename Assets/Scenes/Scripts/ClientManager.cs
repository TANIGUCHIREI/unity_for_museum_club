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

    public string user_input = "���܂����ł��肢���܂��B";
    public Dictionary<string, string> user_input_dict = new Dictionary<string, string>();

    public string userinput_text;
    public bool _is_kansai_only;


    void Start()
    {

        userinput_text = gameObject.GetComponent<User_Input>().userinput_text; //gameObject.GetComponent<>()�͎Q�Ƃ�Ԃ����߁A���̌�̕ύX�����f����܂��Bbygpt4
        _is_kansai_only = gameObject.GetComponent<User_Input>()._is_kansai_only;

        //�ڑ������B�ڑ���T�[�o�ƁA�|�[�g�ԍ����w�肷��
        ws = new WebSocket("ws://localhost:8001/");
        ws.Connect();
        OnSendPython();
        //���M�{�^���������ꂽ�Ƃ��Ɏ��s���鏈���uSendText�v��o�^����
        //endButton.onClick.AddListener(SendText);

        //�T�[�o���烁�b�Z�[�W����M�����Ƃ��Ɏ��s���鏈���uRecvText�v��o�^����
        ws.OnMessage += (sender, e) => RecvFromPython(e.RawData);
        //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
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
        if( userinput_text == "���܂���"){
            input += "���܂����ł��肢���܂��B�Ȃɂ��ʔ����W���������Ă��������B";
        }
        else
        {
            input += userinput_text;
        }
        if (_is_kansai_only)
        {
            input += "\n�����A�ꏊ�͐�΂Ɋ֐�(���E���ɁE���s�E�ޗǁE�a�̎R�E����̂ǂ���)�ł��肢���܂��B";
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

    //�T�[�o����󂯎�������b�Z�[�W���AChatText�ɕ\������
    public void RecvFromPython(byte[] msg)
    {
        var recieved_words_data = Encoding.UTF8.GetString(msg);
        Dictionary<string, object>  dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        //Debug.Log(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        if((string) dictionary["TYPE"] == "QUERY")
        {
            //Debug.Log("�N�G������");
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
    //�T�[�o�̐ڑ����؂ꂽ�Ƃ��̃��b�Z�[�W���AChatText�ɕ\������
    public void RecvClose()
    {
        Debug.Log("Closed");
    }

    
}