using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using static Unity.Burst.Intrinsics.X86.Avx;

public class change_wall : MonoBehaviour
{

    public GameObject black_wall;
    public GameObject white_wall;
    public GameObject Blined_Panel; //これで操作中のタップを防止する。これのRayCast Targetを調整
    public GameObject init_camera;
    public GameObject Now_Loading;
    public GameObject Canvas_for_Warning; //もし接続が切れたらこれを表示する


    public float move_time = 2f;
    public float screen_width = 0f;

    public bool _isAnserArrive = false;

    public bool _isWall_Moving = false; //今回は使ってません！
    // Start is called before the first frame update
    AudioSource audioSource;
    public AudioClip result_bgm;
    public AudioClip Gacha_BackGround_bgm;
    public AudioClip menu_bgm;
    public AudioClip Gacha_result_bgm;
    public float Audio_Volume = 0.3f;
    public float Init_Audio_Volume;
    public float Multiply_Volume_value = 1;

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
        Init_Audio_Volume = Audio_Volume; //このInitのやつにMultipyをかけていく形で、値が収束するのを回避する
        //Debug.Log(Screen.width);
        Blined_Panel.GetComponent<Image>().raycastTarget = false;
        Now_Loading.GetComponent<Text>().enabled = false; //now loadingは初めは表示されないように
        gameObject.GetComponent<AudioSource>().volume = Audio_Volume;

        audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.volume = Audio_Volume;  
        audioSource.clip = menu_bgm;
        audioSource.Play();  //はじめのやつ流す

        //gameObject.GetComponent<Animator>().Play("LetterBox_show", 0, 0f);
        //gameObject.GetComponent<Animator>().Play("Letter_Box_hide", 0, 0f);
        gameObject.GetComponent<Animator>().speed = 0f; //初期動作を停止させる
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Setting_Menu" && !_isWall_Moving)
        {

            Multiply_Volume_value = GameObject.Find("Volume_Slider").GetComponent<Slider>().value;
            Audio_Volume = Multiply_Volume_value * Init_Audio_Volume; //これで値を変更
            audioSource.volume = Audio_Volume;
        }
    }


    public void change_2_3()
    {
        //StartCoroutine(GameObject.Find("EventSystem").GetComponent<menu>().VolumeDown());
        //buttonではIEnumeratorがなんか見つからなかったからその対策用
        StartCoroutine(menu_change2_to_3());
    }

    public void change_2_setting()
    {
        //buttonではIEnumeratorがなんか見つからなかったからその対策用
        StartCoroutine(menu_change2_to_Setting());
    }

    public void change_setting_to_menu()
    {
        StartCoroutine(change_setting2_to_menu());
        
    }

    public IEnumerator change_3_to_result()
    {
        StartCoroutine(VolumeDown()); //やっちゃった！の音量を小さくして結果画面へ
        
        yield return new WaitForSeconds(5f);
        Now_Loading.GetComponent<Text>().enabled = true;
        //yield return new WaitForSeconds(1f); //Gachaシーンでの最後の待ち！
        while (true)
        {
            if (_isAnserArrive)
            {
                break;
            }
            yield return null; //chatgptからの回答が来るまでは待機！
            //このあたりにchangewall_UIのnow loadingの処理を入れる。。。。
        }

        Now_Loading.GetComponent<Text>().enabled = false;
        SceneManager.LoadScene("Result"); //これでリザルト画面へ遷移！
        //yield return new WaitForSeconds(1f); //時間置いたほうがいいかなと思って

        gameObject.GetComponent<AudioSource>().volume = Audio_Volume; //リザルト画面の音楽再生開始！のためにまずはボリュームを戻す
        gameObject.GetComponent<AudioSource>().clip = result_bgm;
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<AudioSource>().loop = true;
        //audioSource.PlayOneShot(result_bgm);

        yield return new WaitForSeconds(3f); //時間置いたほうがいいかなと思って
        GameObject museum_name = GameObject.Find("museum_name");
        GameObject exhibition_type = GameObject.Find("exhibition_type");
        GameObject prefecture = GameObject.Find("prefecture");
        museum_name.GetComponent<TMP_Text>().text = init_camera.GetComponent<ClientManager>().museum_name;
        exhibition_type.GetComponent<TMP_Text>().text = init_camera.GetComponent<ClientManager>().exhibition_name;
        prefecture.GetComponent<TMP_Text>().text = init_camera.GetComponent<ClientManager>().prefecture;

        //ここからテキストサイズがちょうどよい大きさになるように調整する
        StartCoroutine(AdjustFontSizeBasedOnPreferredHeight(museum_name, maxPreferredHeight: 265));
        StartCoroutine(AdjustFontSizeBasedOnPreferredHeight(exhibition_type, maxPreferredHeight: 500));


        yield return new WaitForSeconds(4f); //印刷開始まで、リザルト表示中にスタンバってる、表示中の途中辺りからPRINT_STARTをサーバに送信する
        //ここから"PRINT_START"を送って印刷を開始する
        if(init_camera.GetComponent<ClientManager>()._isStandAloneModeOne == false)
        {
            //通常モードだとほんとに印刷に入るために通信を行う
            Dictionary<string, string> user_input_dict = new Dictionary<string, string> { { "TYPE", "PRINT_START" } };
            string json = JsonConvert.SerializeObject(user_input_dict);
            byte[] json_bytes = Encoding.UTF8.GetBytes(json);
            init_camera.GetComponent<ClientManager>().ws.Send(json_bytes); //これいいけるのか・・・・？よくわからん
            Debug.Log("PRINT_START　Sent a Message");
        }
        else
        {

            //StandAloneModeならランダム時間待機してからPRINT_FINISHを受信する
            StartCoroutine(Fake_Print_Finish_Arrive());
        }

        yield return new WaitForSeconds(4f);
        //GameObject.Find("PrintingNow...").GetComponent<Animator>().enabled = true; //これで印刷しています・・・がチカチカするようになる
    }

    IEnumerator AdjustFontSizeBasedOnPreferredHeight(GameObject obj,float maxPreferredHeight)
    {
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        // Preferred Heightが指定した最大値より大きい場合、whileループでフォントサイズを調整
        while (tmp.preferredHeight > maxPreferredHeight && tmp.fontSize > 0)
        {
            tmp.fontSize -= 10f;
            yield return null;
        }
    }

    IEnumerator Fake_Print_Finish_Arrive()
    {
        float WaitTime = Random.Range(15f, 20f);
        yield return new WaitForSeconds(WaitTime); //疑似的に待ち時間を作る
        init_camera.GetComponent<ClientManager>()._isPrintFinish = true;

    }

    public IEnumerator Connection_Error_and_change_to_menu()
    {
        Canvas_for_Warning.GetComponent<Animator>().SetFloat("speed", 1); 
        Canvas_for_Warning.GetComponent<Animator>().Play("Warning", 0, 0f);
        StartCoroutine(VolumeDown());
        yield return new WaitForSeconds(2.5f); //ちょっと待ってから遷移
        if(SceneManager.GetActiveScene().name == "Result")
        {
            Now_Loading.GetComponent<Text>().enabled = false; //これしないといつまでも「Now Loading...」が出続けてしまう！
        }
        SceneManager.LoadScene("init_menu");
        yield return new WaitForSeconds(.2f);
        

        yield return new WaitForSeconds(2.5f);
        init_camera.GetComponent<Camera>().orthographic = true; //これすることで並行図法から透視図法になる
        init_camera.GetComponent<User_Input>().Reset_Inputs(); //これでインプットをリセット!

        Canvas_for_Warning.GetComponent<Animator>().SetFloat("speed", -1);
        Canvas_for_Warning.GetComponent<Animator>().Play("Warning", 0, 1f);

        init_camera.GetComponent<ClientManager>().Start_func(); //これ１つにまとめました!
        audioSource.volume = Audio_Volume;
        audioSource.clip = menu_bgm;
        audioSource.Play();  //はじめのやつ流す


    }
    IEnumerator menu_change2_to_3()
    {
        _isWall_Moving = true;
        StartCoroutine(VolumeDown()); //bgmをフェードアウト
        //まずは入力された文字と関西or関西じゃない　の情報をClientManagerへ伝える
        GameObject Menu = GameObject.Find("EventSystem");
        init_camera.GetComponent<ClientManager>()._is_kansai_only = Menu.GetComponent<menu>()._isKansaiOnly;


        //ClientManager側に入力情報を送信
        if (Menu.GetComponent<menu>()._isOmakase)
        {
            init_camera.GetComponent<ClientManager>().userinput_text = "おまかせ";
        }
        else
        {
            init_camera.GetComponent<ClientManager>().userinput_text = Menu.GetComponent<menu>().User_Input_Field.GetComponent<TMP_InputField>().text;
        }


        init_camera.GetComponent<ClientManager>().async_SendPython(); //ここでサーバーに情報を送信

        Blined_Panel.GetComponent<Image>().raycastTarget = true;

        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("Gacha");
        init_camera.GetComponent<Camera>().orthographic = false; //これすることで並行図法から透視図法になる

        Debug.Log(init_camera.GetComponent<User_Input>().userinput_text);
        //camera.GetComponent<Camera>().fieldOfView = 71;   //見た感じこれが最適解っぽい
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);
      


        yield return new WaitForSeconds(2f); //扉を開くまでの時間
        gameObject.GetComponent<AudioSource>().volume = Audio_Volume; //これがないとならない！
        gameObject.GetComponent<AudioSource>().clip = Gacha_BackGround_bgm;
        gameObject.GetComponent<AudioSource>().Play(); //やっちゃた！の再生開始

        yield return new WaitForSeconds(2f); //扉を開くまでの時間

        //GameObject User_Input_Text = GameObject.Find("User_Input_Text"); //ここからの処理でUserInputがガチャの入力用紙に反映されるようにする、Wwaitの後に置かないとNullReferenceになる！
        string userinput_text = init_camera.GetComponent<ClientManager>().userinput_text;
        //User_Input_Text.GetComponent<TMP_Text>().text = "「" + userinput_text + "」\n";

        GameObject Gacha_Insert_Paper = GameObject.Find("Gacha_Insert_Papaer");
        Gacha_Insert_Paper.GetComponent<Gacha_Insert_Paper>().Text = "「" + userinput_text + "」\n"; //ペーパーに直接反映するとエラーが生じたので代わりにこのようにした
        
        //User_Input_Text.SetActive(false);
        if (init_camera.GetComponent<ClientManager>()._is_kansai_only)
        {
            //User_Input_Text.GetComponent<TMP_Text>().text += "in関西";
            Gacha_Insert_Paper.GetComponent<Gacha_Insert_Paper>().Text += "in関西";
        }
        else
        {
            //User_Input_Text.GetComponent<TMP_Text>().text += "in日本全国";
            Gacha_Insert_Paper.GetComponent<Gacha_Insert_Paper>().Text += "in日本全国";
        }

        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));
        Blined_Panel.GetComponent<Image>().raycastTarget = false;
        _isWall_Moving = false;
    }

    IEnumerator menu_change2_to_Setting()
    {
        _isWall_Moving = true;
        Blined_Panel.GetComponent<Image>().raycastTarget = true;
        Debug.Log("設定画面へ遷移します！" + " この名前は" +gameObject.name);
        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("Setting_Menu");
        //init_camera.GetComponent<Camera>().orthographic = false; //これすることで並行図法から透視図法になる settingへの遷移ではこれはいらない

        //Debug.Log(init_camera.GetComponent<User_Input>().userinput_text);
        //camera.GetComponent<Camera>().fieldOfView = 71;   //見た感じこれが最適解っぽい
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);
        
        yield return new WaitForSeconds(0.5f);
        GameObject Main_Camaera = GameObject.Find("Main Camera"); //LoadSceneの直後だと動かなかった。ここだと動いた
        Debug.Log(" menu_change2_to_Setting");
        Main_Camaera.SetActive(false);

        //次に前の設定の反映を個々で行う
        GameObject Inputfield_IPAdress = GameObject.Find("InputField_IPAdress");
        GameObject InputField_Port = GameObject.Find("InputField_Port");
        GameObject StandAloneModeToggle = GameObject.Find("StandAloneModeToggle");
        GameObject.Find("Volume_Slider").GetComponent<Slider>().value = Multiply_Volume_value;
        GameObject.Find("Volume_Slider").GetComponentInChildren<TMP_Text>().text = "bgm volume: " + Mathf.CeilToInt((100 * Multiply_Volume_value)).ToString() + "%";
        Inputfield_IPAdress.GetComponent<TMP_InputField>().text = init_camera.GetComponent<ClientManager>().IPAdress;
        InputField_Port.GetComponent<TMP_InputField>().text = init_camera.GetComponent<ClientManager>().Port;
        StandAloneModeToggle.GetComponent<Toggle>().isOn = init_camera.GetComponent<ClientManager>()._isStandAloneModeOne;
        
        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));

        Blined_Panel.GetComponent<Image>().raycastTarget = false;
        _isWall_Moving = false;
    }

    IEnumerator change_setting2_to_menu()
    {
        _isWall_Moving = true;
        Blined_Panel.GetComponent<Image>().raycastTarget = true;

        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        SceneManager.LoadScene("init_menu");
        //init_camera.GetComponent<Camera>().orthographic = false; //これすることで並行図法から透視図法になる settingへの遷移ではこれはいらない

        //Debug.Log(init_camera.GetComponent<User_Input>().userinput_text);
        //camera.GetComponent<Camera>().fieldOfView = 71;   //見た感じこれが最適解っぽい
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);

        yield return new WaitForSeconds(0.5f);
       
        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));

        

        init_camera.GetComponent<ClientManager>().Start_func(); //これ１つにまとめました!

        //init_camera.GetComponent<ClientManager>().Status_Text = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        //init_camera.GetComponent<ClientManager>().menu1_Blined_Panel = GameObject.Find("menu1_Blined_Panel"); //Sceneが切り替わるたびにdestroyed扱いになるのでその都度設定してあげる必要がある
        //init_camera.GetComponent<ClientManager>().menu1_Blined_Panel.GetComponent<Image>().raycastTarget = true; //はじめはメニューが動かないように
        //init_camera.GetComponent<ClientManager>().async_SendPython(Type: "Init_Connection"); //メニュー１に戻るたびに接続がきちんとできるか確認する
        _isWall_Moving = false;
        Blined_Panel.GetComponent<Image>().raycastTarget = false;
    }

    public IEnumerator change_Result_to_menu()
    {
        StartCoroutine(VolumeDown()); //bgmをフェードアウト
        _isWall_Moving = true;
        Blined_Panel.GetComponent<Image>().raycastTarget = true;

        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, move_time)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, move_time));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("init_menu");

        init_camera.GetComponent<Camera>().orthographic = true; //これすることで並行図法から透視図法になる
        init_camera.GetComponent<User_Input>().Reset_Inputs(); //これでインプットをリセット!

        audioSource.volume = Audio_Volume;
        audioSource.clip = menu_bgm;
        audioSource.Play();  //はじめのやつ流す

        yield return new WaitForSeconds(2f);

        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, move_time));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, move_time));

        Blined_Panel.GetComponent<Image>().raycastTarget = false;

        init_camera.GetComponent<ClientManager>().Start_func(); //これ１つにまとめました!
        _isWall_Moving = false;
    }


    /*
    IEnumerator Change_Scene_move()
    {
        List<(GameObject,)>
    }
    */
    //ポケモン風戦闘シーンから持ってきたやつ
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
            yield return null; //これすることで画面に出力されるようになる
        }
        yield break;
    }

    public IEnumerator VolumeDown(float DownTo = 0f)
    {
        

        while (audioSource.volume > DownTo)
        {
            audioSource.volume -= 0.01f* Multiply_Volume_value;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator VolumeUp(float UpTo = 0.3f)
    {
        if(UpTo == 0.3f)
        {
            UpTo = Audio_Volume; //初期値の場合は今現在のAudio_Volumeの値に変換する（float UpTo =Audio_Volumeができなかったのでそのかわりに代用）
        }
  

        while (audioSource.volume < UpTo)
        {
            audioSource.volume += 0.01f * Multiply_Volume_value;
            yield return new WaitForSeconds(0.1f);
        }
    }


}
