using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Text;

public class change_wall : MonoBehaviour
{

    public GameObject black_wall;
    public GameObject white_wall;
    public GameObject Blined_Panel; //����ő��쒆�̃^�b�v��h�~����B�����RayCast Target�𒲐�
    public GameObject init_camera;
    public GameObject Now_Loading;
    public GameObject Canvas_for_Warning; //�����ڑ����؂ꂽ�炱���\������


    public float move_time = 2f;
    public float screen_width = 0f;

    public bool _isAnserArrive = false;
    // Start is called before the first frame update

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(init_camera);
        //black_wall.GetComponent<RectTransform>().offsetMax = new Vector2(right, top);
        //white_wall.GetComponent<RectTransform>(). = new Vector3(Screen.width, 0, 0);
        screen_width = Screen.width;
    }
    void Start()
    {
        //Debug.Log(Screen.width);
        Blined_Panel.GetComponent<Image>().raycastTarget = false;
        Now_Loading.GetComponent<Text>().enabled = false; //now loading�͏��߂͕\������Ȃ��悤��
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }


    public void change_2_3()
    {
        //button�ł�IEnumerator���Ȃ񂩌�����Ȃ��������炻�̑΍��p
        StartCoroutine(menu_change2_to_3());
    }

    public void change_2_setting()
    {
        //button�ł�IEnumerator���Ȃ񂩌�����Ȃ��������炻�̑΍��p
        StartCoroutine(menu_change2_to_Setting());
    }

    public void change_setting_to_menu()
    {
        StartCoroutine(change_setting2_to_menu());
        
    }

    public IEnumerator change_3_to_result()
    {
        yield return new WaitForSeconds(2f);
        Now_Loading.GetComponent<Text>().enabled = true;
        while (true)
        {
            if (_isAnserArrive)
            {
                break;
            }
            yield return null; //chatgpt����̉񓚂�����܂ł͑ҋ@�I
            //���̂������changewall_UI��now loading�̏���������B�B�B�B
        }

        Now_Loading.GetComponent<Text>().enabled = false;
        SceneManager.LoadScene("Result"); //����Ń��U���g��ʂ֑J�ځI
        yield return new WaitForSeconds(1f); //���Ԓu�����ق����������ȂƎv����
        
        TMP_Text museum_name = GameObject.Find("museum_name").GetComponent<TMP_Text>();
        TMP_Text exhibition_type = GameObject.Find("exhibition_type").GetComponent<TMP_Text>();
        TMP_Text prefecture = GameObject.Find("prefecture").GetComponent<TMP_Text>();
        museum_name.text = init_camera.GetComponent<ClientManager>().museum_name;
        exhibition_type.text = init_camera.GetComponent<ClientManager>().exhibition_name;
        prefecture.text = init_camera.GetComponent<ClientManager>().prefecture;

        yield return new WaitForSeconds(4f); //����J�n�܂ŁA���U���g�\�����ɃX�^���o���Ă�A�\�����̓r���ӂ肩��PRINT_START���T�[�o�ɑ��M����
        //��������"PRINT_START"�𑗂��Ĉ�����J�n����
        if(init_camera.GetComponent<ClientManager>()._isStandAloneModeOne == false)
        {
            //�ʏ탂�[�h���Ƃق�ƂɈ���ɓ��邽�߂ɒʐM���s��
            Dictionary<string, string> user_input_dict = new Dictionary<string, string> { { "TYPE", "PRINT_START" } };
            string json = JsonConvert.SerializeObject(user_input_dict);
            byte[] json_bytes = Encoding.UTF8.GetBytes(json);
            init_camera.GetComponent<ClientManager>().ws.Send(json_bytes); //���ꂢ������̂��E�E�E�E�H�悭�킩���
            Debug.Log("PRINT_START�@Sent a Message");
        }
        else
        {

            //StandAloneMode�Ȃ烉���_�����ԑҋ@���Ă���PRINT_FINISH����M����
            StartCoroutine(Fake_Print_Finish_Arrive());
        }

        yield return new WaitForSeconds(4f);
        //GameObject.Find("PrintingNow...").GetComponent<Animator>().enabled = true; //����ň�����Ă��܂��E�E�E���`�J�`�J����悤�ɂȂ�


        IEnumerator Fake_Print_Finish_Arrive()
        {
            float WaitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(WaitTime); //�^���I�ɑ҂����Ԃ����
            init_camera.GetComponent<ClientManager>()._isPrintFinish = true;

        }


    }

    public IEnumerator Connection_Error_and_change_to_menu()
    {
        Canvas_for_Warning.GetComponent<Animator>().SetFloat("speed", 1); 
        Canvas_for_Warning.GetComponent<Animator>().Play("Warning", 0, 0f);
        yield return new WaitForSeconds(2.5f); //������Ƒ҂��Ă���J��
        if(SceneManager.GetActiveScene().name == "Result")
        {
            Now_Loading.GetComponent<Text>().enabled = false; //���ꂵ�Ȃ��Ƃ��܂ł��uNow Loading...�v���o�����Ă��܂��I
        }
        SceneManager.LoadScene("init_menu");

        yield return new WaitForSeconds(2.5f);
        init_camera.GetComponent<Camera>().orthographic = true; //���ꂷ�邱�Ƃŕ��s�}�@���瓧���}�@�ɂȂ�
        init_camera.GetComponent<User_Input>().Reset_Inputs(); //����ŃC���v�b�g�����Z�b�g!

        Canvas_for_Warning.GetComponent<Animator>().SetFloat("speed", -1);
        Canvas_for_Warning.GetComponent<Animator>().Play("Warning", 0, 1f);

        init_camera.GetComponent<ClientManager>().Start_func(); //����P�ɂ܂Ƃ߂܂���!


    }
    IEnumerator menu_change2_to_3()
    {
        //�܂��͓��͂��ꂽ�����Ɗ֐�or�֐�����Ȃ��@�̏���ClientManager�֓`����
        GameObject Menu = GameObject.Find("EventSystem");
        init_camera.GetComponent<ClientManager>()._is_kansai_only = Menu.GetComponent<menu>()._isKansaiOnly;


        //ClientManager���ɓ��͏��𑗐M
        if (Menu.GetComponent<menu>()._isOmakase)
        {
            init_camera.GetComponent<ClientManager>().userinput_text = "���܂���";
        }
        else
        {
            init_camera.GetComponent<ClientManager>().userinput_text = Menu.GetComponent<menu>().User_Input_Field.GetComponent<TMP_InputField>().text;
        }


        init_camera.GetComponent<ClientManager>().async_SendPython(); //�����ŃT�[�o�[�ɏ��𑗐M

        Blined_Panel.GetComponent<Image>().raycastTarget = true;

        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield return���Ȃ���yiled���Ȃ��̂ł����ɉ��̏������n�܂�
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("Gacha");
        init_camera.GetComponent<Camera>().orthographic = false; //���ꂷ�邱�Ƃŕ��s�}�@���瓧���}�@�ɂȂ�

        Debug.Log(init_camera.GetComponent<User_Input>().userinput_text);
        //camera.GetComponent<Camera>().fieldOfView = 71;   //�����������ꂪ�œK�����ۂ�
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);
      


        yield return new WaitForSeconds(0.5f);

        //GameObject User_Input_Text = GameObject.Find("User_Input_Text"); //��������̏�����UserInput���K�`���̓��͗p���ɔ��f�����悤�ɂ���AWwait�̌�ɒu���Ȃ���NullReference�ɂȂ�I
        string userinput_text = init_camera.GetComponent<ClientManager>().userinput_text;
        //User_Input_Text.GetComponent<TMP_Text>().text = "�u" + userinput_text + "�v\n";

        GameObject Gacha_Insert_Paper = GameObject.Find("Gacha_Insert_Papaer");
        Gacha_Insert_Paper.GetComponent<Gacha_Insert_Paper>().Text = "�u" + userinput_text + "�v\n"; //�y�[�p�[�ɒ��ڔ��f����ƃG���[���������̂ő���ɂ��̂悤�ɂ���
        
        //User_Input_Text.SetActive(false);
        if (init_camera.GetComponent<ClientManager>()._is_kansai_only)
        {
            //User_Input_Text.GetComponent<TMP_Text>().text += "in�֐�";
            Gacha_Insert_Paper.GetComponent<Gacha_Insert_Paper>().Text += "in�֐�";
        }
        else
        {
            //User_Input_Text.GetComponent<TMP_Text>().text += "in���{�S��";
            Gacha_Insert_Paper.GetComponent<Gacha_Insert_Paper>().Text += "in���{�S��";
        }

        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));
        Blined_Panel.GetComponent<Image>().raycastTarget = false;
    }

    IEnumerator menu_change2_to_Setting()
    {
        Blined_Panel.GetComponent<Image>().raycastTarget = true;
        Debug.Log("�ݒ��ʂ֑J�ڂ��܂��I" + " ���̖��O��" +gameObject.name);
        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield return���Ȃ���yiled���Ȃ��̂ł����ɉ��̏������n�܂�
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("Setting_Menu");
        //init_camera.GetComponent<Camera>().orthographic = false; //���ꂷ�邱�Ƃŕ��s�}�@���瓧���}�@�ɂȂ� setting�ւ̑J�ڂł͂���͂���Ȃ�

        //Debug.Log(init_camera.GetComponent<User_Input>().userinput_text);
        //camera.GetComponent<Camera>().fieldOfView = 71;   //�����������ꂪ�œK�����ۂ�
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);
        
        yield return new WaitForSeconds(0.5f);
        GameObject Main_Camaera = GameObject.Find("Main Camera"); //LoadScene�̒��ゾ�Ɠ����Ȃ������B�������Ɠ�����
        Debug.Log(" menu_change2_to_Setting");
        Main_Camaera.SetActive(false);

        //���ɑO�̐ݒ�̔��f���X�ōs��
        GameObject Inputfield_IPAdress = GameObject.Find("InputField_IPAdress");
        GameObject InputField_Port = GameObject.Find("InputField_Port");
        GameObject StandAloneModeToggle = GameObject.Find("StandAloneModeToggle");
        Inputfield_IPAdress.GetComponent<TMP_InputField>().text = init_camera.GetComponent<ClientManager>().IPAdress;
        InputField_Port.GetComponent<TMP_InputField>().text = init_camera.GetComponent<ClientManager>().Port;
        StandAloneModeToggle.GetComponent<Toggle>().isOn = init_camera.GetComponent<ClientManager>()._isStandAloneModeOne;
        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));

        Blined_Panel.GetComponent<Image>().raycastTarget = false;
    }

    IEnumerator change_setting2_to_menu()
    {
        Blined_Panel.GetComponent<Image>().raycastTarget = true;

        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield return���Ȃ���yiled���Ȃ��̂ł����ɉ��̏������n�܂�
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("init_menu");
        //init_camera.GetComponent<Camera>().orthographic = false; //���ꂷ�邱�Ƃŕ��s�}�@���瓧���}�@�ɂȂ� setting�ւ̑J�ڂł͂���͂���Ȃ�

        //Debug.Log(init_camera.GetComponent<User_Input>().userinput_text);
        //camera.GetComponent<Camera>().fieldOfView = 71;   //�����������ꂪ�œK�����ۂ�
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);

        yield return new WaitForSeconds(0.5f);
       
        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));

        Blined_Panel.GetComponent<Image>().raycastTarget = false;

        init_camera.GetComponent<ClientManager>().Start_func(); //����P�ɂ܂Ƃ߂܂���!
        //init_camera.GetComponent<ClientManager>().Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        //init_camera.GetComponent<ClientManager>().menu1_Blined_Panel = GameObject.Find("menu1_Blined_Panel"); //Scene���؂�ւ�邽�т�destroyed�����ɂȂ�̂ł��̓s�x�ݒ肵�Ă�����K�v������
        //init_camera.GetComponent<ClientManager>().menu1_Blined_Panel.GetComponent<Image>().raycastTarget = true; //�͂��߂̓��j���[�������Ȃ��悤��
        //init_camera.GetComponent<ClientManager>().async_SendPython(Type: "Init_Connection"); //���j���[�P�ɖ߂邽�тɐڑ���������Ƃł��邩�m�F����
    }

    public IEnumerator change_Result_to_menu()
    {
        Blined_Panel.GetComponent<Image>().raycastTarget = true;

        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield return���Ȃ���yiled���Ȃ��̂ł����ɉ��̏������n�܂�
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("init_menu");

        init_camera.GetComponent<Camera>().orthographic = true; //���ꂷ�邱�Ƃŕ��s�}�@���瓧���}�@�ɂȂ�
        init_camera.GetComponent<User_Input>().Reset_Inputs(); //����ŃC���v�b�g�����Z�b�g!

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));

        Blined_Panel.GetComponent<Image>().raycastTarget = false;

        init_camera.GetComponent<ClientManager>().Start_func(); //����P�ɂ܂Ƃ߂܂���!
    }


    /*
    IEnumerator Change_Scene_move()
    {
        List<(GameObject,)>
    }
    */
    //�|�P�������퓬�V�[�����玝���Ă������
    IEnumerator Relative_Line_move(GameObject obj, float angle, float length, float mov_time)
    {


        Vector2 init_pos = obj.GetComponent<RectTransform>().anchoredPosition;
        float mov_velocity = length / (mov_time * (1 / Time.deltaTime));
        float radian_angle = angle * Mathf.PI / 180;
        Vector2 normalized_direction = new Vector2(Mathf.Cos(radian_angle), Mathf.Sin(radian_angle));
        float now_length = 0;
        while (now_length < length)
        {
            mov_velocity = length / (mov_time * (1 / Time.deltaTime));
            Vector2 my_position = obj.GetComponent<RectTransform>().anchoredPosition;
            obj.GetComponent<RectTransform>().anchoredPosition = my_position + mov_velocity * normalized_direction;
            now_length = Vector2.Distance(init_pos, obj.GetComponent<RectTransform>().anchoredPosition);
            yield return null; //���ꂷ�邱�Ƃŉ�ʂɏo�͂����悤�ɂȂ�
        }
        yield break;
    }
}
