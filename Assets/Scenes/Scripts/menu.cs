using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

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

    public float back_treshhold_time = 0;
    public bool menu_moving = false; //連打して動いちゃわない用のやつ

    public float ViewportScreenWidth;
    //public GameObject cameraobj;
    //public Camera cam;
    public float speed = 0;

    public GameObject Canvas_for_Confirmation;
    public GameObject Input_Confirmation;
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

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(back_treshhold_time > 0)
        {
            back_treshhold_time -= 0.03f;
            //Debug.Log(back_treshhold_time);
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

    public void Change_to_2_1()
    {
        _isOmakase = false;
        menu2_1.SetActive(true);
        menu2_2.SetActive(false);
        StartCoroutine(menu_move(-speed));
    }

    public void Change_to_2_2()
    {
        _isOmakase = true;
        menu2_2.SetActive(true);
        menu2_1.SetActive(false);
        StartCoroutine(menu_move(-speed));
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
            menus.GetComponent<RectTransform>().Translate(speed, 0, 0);
            menus_position_x = menus.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //これすることで画面に出力されるようになる
        }
        menu_moving = false;
        yield break;
    }

   
}
