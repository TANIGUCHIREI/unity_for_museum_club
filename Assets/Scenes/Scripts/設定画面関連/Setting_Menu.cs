
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.Text;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using System.Threading.Tasks;�@//�񓯊���websocket�ł�������s�����߂̂���

public class Setting_Menu : MonoBehaviour
{
    public GameObject Change_walls_UI;
    public GameObject Init_Camera;
    public WebSocket ws;
    GameObject Input_IPAdress;
    GameObject Input_Port;
    public Slider mainSlider;
    public GameObject StandAloneModeToggle;

    public TMP_Text Status_Text;
    public string show_text = "Connection Status will be mentioned Here..."; //websocket�͕ʃX���b�h�œ����̂�gameobject�𒼐ڑ���ł��Ȃ����G���[���o�Ȃ��i�������j���̂��߂̃o�b�t�@�[�I�Ȃ��
    public static string IPAdress = "127.0.0.1";
    public static string Port = "8001";

    public Dictionary<string, string> user_input_dict = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        Change_walls_UI = GameObject.Find("Change_walls_UI");
        Init_Camera = GameObject.Find("Init_Camera");
        //Debug.Log("Setting_Mneu Works!");

        Input_IPAdress = GameObject.Find("InputField_IPAdress");
        Input_Port = GameObject.Find("InputField_Port");
        //Debug.Log(Input_IPAdress.name);
        StandAloneModeToggle = GameObject.Find("StandAloneModeToggle");
        Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();

        //Debug.Log("ws://" + IPAdress + ":" + Port + "/");

        mainSlider.onValueChanged.AddListener(delegate { OnVolume_Slider_Changed(); });



    }

    // Update is called once per frame
    void Update()
    {
        Status_Text.text = show_text;
    }

    public void OnVolume_Slider_Changed()
    {
        Debug.Log(mainSlider.value);
        mainSlider.GetComponentInChildren<TMP_Text>().text = "bgm volume: " + Mathf.CeilToInt((100*mainSlider.value)).ToString() + "%";
    }

    public void OnBackToMenuOn()
    {
        Change_walls_UI.GetComponent<change_wall>().change_setting_to_menu();
        Init_Camera.GetComponent<ClientManager>().IPAdress = IPAdress;
        Init_Camera.GetComponent<ClientManager>().Port = Port;
        Init_Camera.GetComponent<ClientManager>()._isStandAloneModeOne = StandAloneModeToggle.GetComponent<Toggle>().isOn;
    }

    public void OnButtonClick()
    {
        HandleButtonClickAsync();
    }

    private async void HandleButtonClickAsync()
    {
        await Test_CoMwithPython(); //async���{�^���œ��������߂̂Ȃ��̖�ځi���b�p�[���\�b�h�Ƃ����炵���j
    }

    public async Task Test_CoMwithPython()
    {
        Debug.Log("Test_COmwithPython ��������J�n���܂���");
        //�����Setting Menu�V�[���Ŏg���p�r�I�܂������͂߂��ራ�����ǂ������炸
        IPAdress = Input_IPAdress.GetComponent<TMP_InputField>().text;
        Port = Input_Port.GetComponent<TMP_InputField>().text;

        try
        {
            //ws.Close(); //�͂��߂ɂ������������
            Debug.Log("trying to connetct to " + "ws://" + IPAdress + ":" + Port + "/");
            ws = new WebSocket("ws://" + IPAdress + ":" + Port + "/");
            show_text = "try to connetct to " + "ws://" + IPAdress + ":" + Port + "/";
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
            show_text = "Sent a Message";
        }
        catch (System.InvalidOperationException e)
        {
            Debug.Log("�ڑ��悪�I�[�v������Ă��܂���I");
            show_text = "Cannot Connect To The Server!";

        }   
      

    }



    public void RecvFromPython(byte[] msg)
    {
        Debug.Log("received Message From Python");
        var recieved_words_data = Encoding.UTF8.GetString(msg);
        Dictionary<string, object> recv_dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        //Debug.Log(recieved_words_data);
        //Debug.Log(dict["TYPE"]);
        string Recv_Type = (string)recv_dict["TYPE"];

        if (Recv_Type == "QUERY")
        {
            //Debug.Log("�N�G������");
            //Debug.Log(dictionary["QUERY"].GetType());
            List<string> QUERY = (recv_dict["QUERY"] as JArray).ToObject<List<string>>();
            foreach (var word in QUERY)
            {
                Debug.Log(word);
            }

        }
        else if (Recv_Type == "ANSWER")
        {
            string prefecture = (string)recv_dict["prefecture"];
            string museum_name = (string)recv_dict["museum_name"];
            string exhibition_name = (string)recv_dict["exhibition_name"];
            string exhibition_reason = (string)recv_dict["exhibition_reason"];
            Debug.Log(prefecture + " " + museum_name + " " + exhibition_name);
            Debug.Log(exhibition_reason);

        }
        else if (Recv_Type == "COM_TEST")
        {
            string Response_from_Python = (string)recv_dict["RESPONSE"];
            Debug.Log(Response_from_Python);
            show_text = Response_from_Python;
        }
    }
    //�T�[�o�̐ڑ����؂ꂽ�Ƃ��̃��b�Z�[�W���AChatText�ɕ\������

  
    public void RecvClose()
    {
        Debug.Log("�T�[�o�[�Ƃ̐ڑ����؂�܂���");
        ws.Close();
    }

   

}
