using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using Shapes2D;

public class menu : MonoBehaviour
{
    public GameObject menus;
    public GameObject menu1;
    public GameObject menu2_1;
    public GameObject menu2_2;
    //public GameObject menu3; //多分使わない

    public GameObject kansai_obj;
    public GameObject other_japan_obj;

    public GameObject kansainomi;
    public GameObject zenkoku;

    public GameObject japan_3Dmap;
    //public GameObject PopUpwindow;
    public GameObject change_walls;

    public GameObject init_camera;
    public GameObject User_Input_Field;
    public GameObject Record_Button;

    public GameObject BackGroud_WallPaper;
    public GameObject BackGroud_WallPaper2;
    public float back_treshhold_time = 0;
    public bool menu_moving = false; //連打して動いちゃわない用のやつ

    public float ViewportScreenWidth;
    //public GameObject cameraobj;
    //public Camera cam;
    private float speed = 5f;
    private float background_moving_speed = 30f;

    public GameObject Canvas_for_Confirmation;
    public GameObject Input_Confirmation;
    public GameObject Canvas_for_Recording;
    public GameObject Blined_Panel;
    public bool _isOmakase;
    public bool _isKansaiOnly = true;

   

    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 60; //これで全体的に固定されるとか？→実際に固定された！必ず一度は通ることろにこれを置いておけばOK

        menu1.SetActive(true);
        menu2_1.SetActive(false);
        menu2_2.SetActive(false);
        japan_3Dmap.SetActive(true);
        Canvas_for_Confirmation.SetActive(true);
        Canvas_for_Confirmation.GetComponent<Animator>().SetFloat("speed", 0); //multipleする値を0にすることで実質速度を0、つまりストップさせる

        Canvas_for_Recording.SetActive(true);
        Canvas_for_Recording.GetComponent<Animator>().SetFloat("speed", 0); //multipleする値を0にすることで実質速度を0、つまりストップさせる


        //menu3.SetActive(true);

        //cameraobj = GameObject.Find("Main Camera");
        //cam = cameraobj.GetComponent<Camera>();
        //ViewportScreenWidth =  cam.ScreenToViewportPoint(new Vector3(Screen.width, 0, 0)).x;
        menu2_1.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
        //menu2_1.transform.Translate(new Vector3(Screen.width, 0,0));
        Debug.Log(menu2_1.GetComponent<RectTransform>().anchoredPosition.x);
        menu2_2.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
        Vector3 init_japn_pos = japan_3Dmap.GetComponent<RectTransform>().anchoredPosition;
        japan_3Dmap.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0) + init_japn_pos;
        //menu3.GetComponent<RectTransform>().anchoredPosition = new Vector3(2*Screen.width, 0, 0);
        //普通のtransform.position指定ではrectTransformの値が全くおかしくなってしまったので、RectTransformを使用した

        zenkoku.GetComponent<Animator>().SetBool("transparent", true); //これで全国というテキストを最初に半透明にしている！

        change_walls = GameObject.Find("change_walls"); //なんか仏ーにserialized fielndからやっていると、シーン遷移の段階で消えてしまう
        init_camera = GameObject.Find("Init_Camera");

        Blined_Panel = GameObject.Find("Blined_Panel");

        StartCoroutine(BackGround_WallPaper_Rotation(speed:1f)); //背景を回転させる（ただの装飾）

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(back_treshhold_time > 0)
        {
            back_treshhold_time -= 0.03f;
            //Debug.Log(back_treshhold_time);
        }

        if (menu_moving)
        {
            Blined_Panel.GetComponent<Image>().raycastTarget = true; //超アナログだけど、動いてるときはボタンを押せないようにしてる（負荷大きそう・・・）
        }
     
    }

    public void OnSearchButtonOn()
    {
        string Input_Confirmation_text = "";
        if (_isKansaiOnly)
        {
            Input_Confirmation_text += "関西の展示で\n";
        }
        else
        {
            Input_Confirmation_text += "全国の展示で\n";
        }

        if (_isOmakase)
        {
            Input_Confirmation_text += "「おまかせ」\n";
        }
        else
        {
            Input_Confirmation_text += "「" + User_Input_Field.GetComponent<TMP_InputField>().text + "」\n";
        }

        Input_Confirmation_text += "という内容でガチャを回します。\nよろしいですか？";
        Input_Confirmation.GetComponent<TMP_Text>().text = Input_Confirmation_text;

        Canvas_for_Confirmation.GetComponent<Animator>().SetFloat("speed", 1); //これでポップアップのアニメーションが動作します
        Canvas_for_Confirmation.GetComponent<Animator>().Play("Menu_Confirmation", 0, 0f); //はじめから動作させる
    }

    public void OnConfirmationBackOn()
    {
        

        Canvas_for_Confirmation.GetComponent<Animator>().SetFloat("speed", -1); //これで再生を逆向きにする・・・？
        Canvas_for_Confirmation.GetComponent<Animator>().Play("Menu_Confirmation", 0,1f); //1fは最後ということ・最後から逆向き（－１）に再生する
        //Canvas_for_Confirmation.SetActive(false); 
    }

    public void OnRecordingBackOn()
    {
        GameObject Canvas_for_Recording = GameObject.Find("Canvas_for_Recording");

        Canvas_for_Recording.GetComponent<Animator>().SetFloat("speed", -1); //これで再生を逆向きにする・・・？
        Canvas_for_Recording.GetComponent<Animator>().Play("Menu_Recoding", 0, 1f); //1fは最後ということ・最後から逆向き（－１）に再生する
        //Canvas_for_Confirmation.SetActive(false); 

        
    }

    public void OnSearchByVoiceOn()
    {
        Canvas_for_Recording.GetComponent<Animator>().SetFloat("speed", 1); //これでポップアップのアニメーションが動作します
        Canvas_for_Recording.GetComponent<Animator>().Play("Menu_Recoding", 0, 0f); //はじめから動作させる

        Record_Button.GetComponent<Shape>().settings.blur = 0 ; //リセットしなおす

    }

    public void Change_to_2_1()
    {
        _isOmakase = false;
        menu2_1.SetActive(true);
        menu2_2.SetActive(false);
        StartCoroutine(menu_move(-speed));
        StartCoroutine(BackGround_WallPaper_move(speed: background_moving_speed));
    }

    public void Change_to_2_2()
    {
        _isOmakase = true;
        menu2_2.SetActive(true);
        menu2_1.SetActive(false);
        StartCoroutine(menu_move(-speed));
        StartCoroutine(BackGround_WallPaper_move(speed: background_moving_speed));
    }

    public void Change_to_3()
    {
        //StartCoroutine(menu_move(-10));
        
    }

    public void menu_back()
    {
        if(menus.GetComponent<RectTransform>().anchoredPosition.x! <-100)
        {
            //メニューが１ではなく2_1とか2_2とか3ならバックできる
            //speed分translateさせて動くから、実際には若干座標がずれることに注意
            StartCoroutine(menu_move(speed));
            StartCoroutine(BackGround_WallPaper_move(speed: -background_moving_speed));
        }
        
    }

    public void OnbackButtonOn()
    {

        if (SceneManager.GetActiveScene().name == "init_menu")
        {
            if (menu_moving == false && menus.transform.position.x! <= 0)
            {
                back_treshhold_time += 0.5f;
                if (back_treshhold_time > 1f)
                {
                    menu_back();
                    back_treshhold_time = 0;

                }
            }
        }
       
        
    }

    public void OnKansaiOnlyOn()
    {
        _isKansaiOnly = true;
        kansai_obj.GetComponent<Animator>().SetBool("pop", true);
        other_japan_obj.GetComponent<Animator>().SetBool("pop", false);


        kansainomi.GetComponent<Animator>().SetBool("transparent", false);
        zenkoku.GetComponent<Animator>().SetBool("transparent", true);
        init_camera.GetComponent<User_Input>()._is_kansai_only = true; 

    }

    public void OnAllJapnOn()
    {
        _isKansaiOnly = false;
        
        kansai_obj.GetComponent<Animator>().SetBool("pop", false);
        other_japan_obj.GetComponent<Animator>().SetBool("pop", true);

        kansainomi.GetComponent<Animator>().SetBool("transparent", true);
        zenkoku.GetComponent<Animator>().SetBool("transparent", false);

        init_camera.GetComponent<User_Input>()._is_kansai_only = false;



    }

  

    IEnumerator menu_move(float speed)
    {
        menu_moving = true;
        //menusはtransformはrectのほうが扱いやすい・・・というかそっちが解像度で管理されているのでそちらで処理します
        float init_x = menus.GetComponent<RectTransform>().anchoredPosition.x;
        float menus_position_x = init_x;
        Debug.Log("速度:" +  speed);
        while (Mathf.Abs(menus_position_x - init_x) < Screen.width)
        {
            //Debug.Log(Mathf.Abs(menus_position_x - init_x));
            //Debug.Log(menus.GetComponent<RectTransform>().anchoredPosition.x);
            menus.GetComponent<RectTransform>().Translate(speed*Time.deltaTime, 0, 0);
            menus_position_x = menus.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //これすることで画面に出力されるようになる
        }
        menu_moving = false;
        Blined_Panel.GetComponent<Image>().raycastTarget = false;
        yield break;
    }

    IEnumerator BackGround_WallPaper_move(float speed = 10f)
    {
        
        while (menu_moving)
        {
            //BackGroud_WallPaper.GetComponent<Transform>().transform.Rotate(new Vector3(0, 0, speed*Time.deltaTime));//とにかく背景を回転させる！
            BackGroud_WallPaper.GetComponent<Transform>().transform.position += new Vector3(speed * -0.06f*Time.deltaTime, 0,0);
            BackGroud_WallPaper2.GetComponent<Transform>().transform.position += new Vector3(speed * -0.03f * Time.deltaTime, 0, 0);
            yield return null; 
        }
        

    }

    IEnumerator BackGround_WallPaper_Rotation(float speed = 10f)
    {
      while (true)
        {
           
            BackGroud_WallPaper.GetComponent<Transform>().transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));//とにかく背景を回転させる！
            BackGroud_WallPaper2.GetComponent<Transform>().transform.Rotate(new Vector3(0, 0, -speed * Time.deltaTime));//とにかく背景を回転させる！
            yield return null;
        }
    }

    public IEnumerator VolumeDown()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        while (audioSource.volume > 0)
        {
            audioSource.volume -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
