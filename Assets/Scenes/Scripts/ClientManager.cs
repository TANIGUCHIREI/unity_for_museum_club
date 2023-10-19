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
using System.Threading.Tasks; //�񓯊���websocket�ł�������s�����߂̂���
using TMPro;

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

    public GameObject menu1_Blined_Panel;
    public TMP_Text Status_Text;

    void Start()
    {
        Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        //Status_Text.text = "";
        menu1_Blined_Panel = GameObject.Find("menu1_Blined_Panel");
        menu1_Blined_Panel.GetComponent<Image>().raycastTarget = true; //�͂��߂̓��j���[�������Ȃ��悤��
        async_SendPython(Type :"Init_Connection");




    }


    public void OnButtonClick()
    {
        async_SendPython(); //���b�p�[�֐����g����async���o�b�N�O���E���h�œ��삳����B����œ��삪�Ȃ߂炩�ɂȂ�
    }


    public async void async_SendPython(string Type = null)
    {
        if(Type == "Init_Connection")
        {
            //���j���[�P�ɂāi�܂菉��N�����Ƃ��jws connect�ł��邩�ǂ����̊m�F

            bool _is_ConnectSuccess = await Init_Test_CoMwithPython(); //�����ʂɂĐڑ��ɐ��������玟�̉�ʂɍs����A�����|�Ȃ�ݒ肵�������Ȃ�

            if (_is_ConnectSuccess)
            {
                menu1_Blined_Panel.GetComponent<Image>().raycastTarget = false; //�ڑ��ɐ��������玟�̉�ʂɍs����悤�ɂ���I
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
        if( userinput_text == "���܂���"){
            input += "���܂����ł��肢���܂��B�Ȃɂ��ʔ����W���������Ă��������B";
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
        //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
        //ws.OnClose += (sender, e) => RecvClose();

        await Task.Run(() =>  ws.Connect());
        user_input_dict = new Dictionary<string, string> { { "TYPE", "USER_INPUT" } ,{ "user_input", user_input },{ "_is_kansai_only",_is_kansai_only.ToString() } };
        string json = JsonConvert.SerializeObject(user_input_dict);
        byte[] json_bytes = Encoding.UTF8.GetBytes(json);
        ws.Send(json_bytes);
    }



    public async Task<bool> Init_Test_CoMwithPython()
    {
        Debug.Log("Init_Test_COmwithPython ��������J�n���܂���");

        try
        {
            //ws.Close(); //�͂��߂ɂ������������
            Debug.Log("trying to connetct to " + "ws://" + IPAdress + ":" + Port + "/");
            ws = new WebSocket("ws://" + IPAdress + ":" + Port + "/");
            //�T�[�o���烁�b�Z�[�W����M�����Ƃ��Ɏ��s���鏈���uRecvText�v��o�^����
            ws.OnMessage += (sender, e) => RecvFromPython(e.RawData);
            //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
            ws.OnClose += (sender, e) => RecvClose();

            //����ws��V������邽�тɒ�`����K�v������̂ł͂Ȃ����H
            await Task.Run(() => ws.Connect()); // ������񓯊��ɕύX

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
            Debug.Log("�ڑ��悪�I�[�v������Ă��܂���I");
            Status_Text.text = "Cannot Connect To the Server ! \nPlease Check the Setting";

            return false;

        }


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