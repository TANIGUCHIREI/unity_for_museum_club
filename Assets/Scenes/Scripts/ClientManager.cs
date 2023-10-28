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
using System;

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

    public  string IPAdress = "192.168.100.116"; //���̕ӂ�SettingMenu�ɂđ���܂�
    public  string Port = "8001";

    public GameObject menu1_Blined_Panel; //�����̐ݒ肳�ꂽ�l�́Achange_wall.cs���ŃV�[���J�ڎ��ɐݒ葤�ɔ��f�����悤�ɐݒ肵�Ă��܂��B�t��������H
    public TMP_Text Status_Text;
    public GameObject Change_walls_UI;
    public bool _isStandAloneModeOne = false; //���ꂪOn�ł���Ƃ��͒ʐM������Ȃ��A�Ƃ������e�X�g���[�h�ɂȂ�

    public List<string> QUERY_Buffer = new List<string>(); //����͕ʃX���b�h��WebsocketClient���炱��𑀍삵�A����Ƀ��C���X���b�h�̂�����Getcompoment��GachatController�𑀍삷��悤�̂��
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

            Debug.Log("ClientManager����GachaController�ɑ��M");
            GameObject Gacha_Event = GameObject.Find("Gacha_Event");
            Gacha_Event.GetComponent<Gacha_Controller>().QUERY = QUERY_Buffer;
            Gacha_Event.GetComponent<Gacha_Controller>()._isQueryArrive = true;
            _isQueryArrive = false;
            //���ɂ���https://mono-pro.net/archives/9029���Q�l�ɂȂ�܂���
        }

        if (_isAnserArrive)
        {
            GameObject change_wall = GameObject.Find("Change_walls_UI");
            change_wall.GetComponent<change_wall>()._isAnserArrive = true;
            //change_walls�ɃV�[���J�ڂ��Ă��������Ƃ�`����
            _isAnserArrive = false;
        }

        if (_isPrintFinish)
        {
            //���U���g��ʂɂȂ��Ă���O��ŏ������Ă��܂��A�����łȂ���_isPrintFinish = true�ɂ͂Ȃ�Ȃ�����I
            Debug.Log("Print Finish!!");

            GameObject Result_EventSystem = GameObject.Find("Result_EventSystem");
            Result_EventSystem.GetComponent<change_result_panel>().StartCoroutine(Result_EventSystem.GetComponent<change_result_panel>().menu_move(speed:1000));
            _isPrintFinish = false;
        }

        if (_isAudio_Input_Converted_Arrive)
        {
            Debug.Log("�������͂��ϊ�����܂����I : " + userinput_text);
            GameObject menu_EventSystem = GameObject.Find("EventSystem");
            GameObject InputField = GameObject.Find("InputField (TMP)");
            GameObject Record_Button = GameObject.Find("Record_Button"); //�u�ϊ����ł��E�E�E�v�̃A�j���[�V�������~�����邽�߂ɕK�v
            Record_Button.GetComponent<VoiceRecorder>()._is_convert_to_text_finish = true;
            InputField.GetComponent<TMP_InputField>().text = userinput_text;
            menu_EventSystem.GetComponent<menu>().OnRecordingBackOn(); //�|�b�v�A�b�v��߂�
            
            _isAudio_Input_Converted_Arrive = false;
        }

        if(_is_Coneection_Closed && !_isPrintFinish && !Doing_InitConnection_test)
        {
            //�R�l�N�V�������؂ꂽ��������I����Ă��Ȃ��i�܂胉�X�g��ʂɒB���Ă��Ȃ��̂ɐڑ����؂ꂽ�ꍇ�j�͈ȏ�Ȃ̂Ń^�C�g����ʂɖ߂�
            //�܂��^�C�g����ʂł̃R�l�N�V�����G���[�͎d�l�Ȃ̂Ŗ�������
            StartCoroutine(Change_walls_UI.GetComponent<change_wall>().Connection_Error_and_change_to_menu());
            _is_Coneection_Closed = false;
        }
    }

    
    public void Start_func()
    {
        Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        //Status_Text.text = "";
        menu1_Blined_Panel = GameObject.Find("menu1_Blined_Panel");
        menu1_Blined_Panel.GetComponent<Image>().raycastTarget = true; //�͂��߂̓��j���[�������Ȃ��悤��

        if (_isStandAloneModeOne)
        {
            //�ʐM�����Ȃ����[�h
            menu1_Blined_Panel.GetComponent<Image>().raycastTarget = false; //�ڑ��ɐ��������玟�̉�ʂɍs����悤�ɂ���I
            Status_Text.text = "Stand Alone Mode";

        }
        else
        {
            async_SendPython(Type: "Init_Connection");
        }
    }
    public void OnButtonClick()
    {
   
         async_SendPython(); //���b�p�[�֐����g����async���o�b�N�O���E���h�œ��삳����B����œ��삪�Ȃ߂炩�ɂȂ�
   
    }


    public async void async_SendPython(string Type = null)
    {

        if (_isStandAloneModeOne == false)
        {
            //�X�^���h�A�������[�h�Ȃ炱�̐�̑���͎��s���Ȃ��I
            if (Type == "Init_Connection")
            {
                //���j���[�P�ɂāi�܂菉��N�����Ƃ��jws connect�ł��邩�ǂ����̊m�F

                Doing_InitConnection_test = true; //����̐ڑ��e�X�g�����Ă��邱�Ƃ�S�̌f��
                bool _is_ConnectSuccess = await Init_Test_CoMwithPython(); //�����ʂɂĐڑ��ɐ��������玟�̉�ʂɍs����A�����|�Ȃ�ݒ肵�������Ȃ�

                if (_is_ConnectSuccess)
                {
                    menu1_Blined_Panel.GetComponent<Image>().raycastTarget = false; //�ڑ��ɐ��������玟�̉�ʂɍs����悤�ɂ���I
                }

                await Task.Delay(100); // 0.1�b�҂�

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
            Debug.Log("�N�G������");
            //Debug.Log(dictionary["QUERY"].GetType());
            List<string> QUERY = (recv_dict["QUERY"] as JArray).ToObject<List<string>>();

            //����������Gacha�V�[���Ɉڍs���Ă��邱�Ƃ�O���ɍl���Ă���
            
            foreach (var word in QUERY)
            {
                Debug.Log(word);
            }
            
            Debug.Log("�N�G��ClientManager��M");
            QUERY_Buffer = QUERY; //�܂��̓o�b�t�@�[�ɕۑ�����
            _isQueryArrive = true;
            //�킩�����E�E�E�E�R���[�`���͂����i�ʃX���b�h�j���Ⴋ�ǂ��ł��Ȃ��킯���A�z��GetComponent�̂悤��Unity�R���|�[�l���g��API���������Ƃ͕ʃX���b�h���ᖳ���I




        }
        else if (Recv_Type == "ANSWER")
        {
            _isAnserArrive = true; //Arrive�������Ƃ�������
            prefecture = (string) recv_dict["prefecture"];
            museum_name = (string)recv_dict["museum_name"];
            exhibition_name = (string)recv_dict["exhibition_name"];
            exhibition_reason = (string)recv_dict["exhibition_reason"];
            Debug.Log(prefecture + " " + museum_name + " " + exhibition_name);
            Debug.Log(exhibition_reason);

        }else if (Recv_Type == "COM_TEST")
        {
            string Response_from_Python = (string)recv_dict["RESPONSE"]; //�\���͂����i�ʃX���b�h�j�ł͂ł��܂���̂ŕύX���āAUpdate���ŕ\�����s���Ă���
            Debug.Log(Response_from_Python);
        }
        else if (Recv_Type == "PRINT_FINISH")
        {
            Debug.Log("�v�����g���I������܂����Iwebsocket���N���[�Y���܂�");
            _isPrintFinish = true;
            ws.Close();
        }else if (Recv_Type == "AUDIO")
        {
            userinput_text = (string)recv_dict["user_input"]; //��������ϊ����ꂽ���̓e�L�X�g����M���Ă���
            _isAudio_Input_Converted_Arrive = true;
        }
    }
    //�T�[�o�̐ڑ����؂ꂽ�Ƃ��̃��b�Z�[�W���AChatText�ɕ\������
    public void RecvClose()
    {
        Debug.Log("Closed");
        ws.Close();
        if (!Doing_InitConnection_test)
        {
            _is_Coneection_Closed = true; //����ڑ����͂���i�G���[�����p�̂��́j�͕K�v�Ȃ�
        }
        
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        if (!_isStandAloneModeOne)
        {
            ws.Close(); //Close�������A�v�����I������Ƃ��ɂ��
        }
    }


    public IEnumerator SendWavFile(byte[] wavData, WebSocket ws, int chunkSize = 4096)
    {
        //GPT4���o�͂��������f�[�^���`�����N�ɕ����đ��M������@
        // �S�̂̃`�����N�����v�Z
        int totalChunks = (int)Math.Ceiling((double)wavData.Length / chunkSize);

        for (int i = 0; i < totalChunks; i++)
        {
            int start = i * chunkSize;
            int end = Math.Min(start + chunkSize, wavData.Length);
            byte[] chunk = new byte[end - start];
            Array.Copy(wavData, start, chunk, 0, end - start);

            // �`�����N�𑗐M
            ws.Send(chunk);

            // �Z���ҋ@���Ԃ�ǉ��i�T�[�o�[���f�[�^����M�E�������鎞�Ԃ��m�ہj
            yield return new WaitForSeconds(0.1f); // 0.1�b�ҋ@�B�K�v�ɉ����Ē����\
        }

        ws.Send(Encoding.UTF8.GetBytes("")); //���������ws_client_test.py���Q�l�ɁA��̃o�C�i���f�[�^�𑗂邱�Ƃŏ������I�������E�E�E�I�I�I
    }


}