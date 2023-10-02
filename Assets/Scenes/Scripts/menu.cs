using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public GameObject PopUpwindow;
    public GameObject change_walls;

    public float back_treshhold_time = 0;
    public bool menu_moving = false; //連打して動いちゃわない用のやつ

    public float ViewportScreenWidth;
    //public GameObject cameraobj;
    //public Camera cam;
    public float speed = 0;

    
    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 60; //これで全体的に固定されるとか？→実際に固定された！必ず一度は通ることろにこれを置いておけばOK

        menu1.SetActive(true);
        menu2_1.SetActive(false);
        menu2_2.SetActive(false);
        japan_3Dmap.SetActive(true);
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

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(back_treshhold_time > 0)
        {
            back_treshhold_time -= 0.03f;
            Debug.Log(back_treshhold_time);
        }
    }

    public void Change_to_2_1()
    {
        menu2_1.SetActive(true);
        menu2_2.SetActive(false);
        StartCoroutine(menu_move(-speed));
    }

    public void Change_to_2_2()
    {
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

        if(SceneManager.GetActiveScene().name == "init_menu")
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
        }else if(SceneManager.GetActiveScene().name == "Gacha")
        {
                back_treshhold_time += 0.5f;
                if (back_treshhold_time > 1f)
                {
                    change_walls.GetComponent<change_wall>().change_3_2();
                    back_treshhold_time = 0;

                }
        }
            
        
    }

    public void OnKansaiOnlyOn()
    {
        kansai_obj.GetComponent<Animator>().SetBool("pop", false);
        other_japan_obj.GetComponent<Animator>().SetBool("pop", true);

        kansainomi.GetComponent<Animator>().SetBool("transparent", true);
        zenkoku.GetComponent<Animator>().SetBool("transparent", false);

    }

    public void OnAllJapnOn()
    {
        kansai_obj.GetComponent<Animator>().SetBool("pop", true);
        other_japan_obj.GetComponent<Animator>().SetBool("pop",false);

        
        kansainomi.GetComponent<Animator>().SetBool("transparent", false);
        zenkoku.GetComponent<Animator>().SetBool("transparent", true);


        
    }

    public void OnGoTomenu3Enter()
    {
        PopUpwindow.GetComponent<Animator>().SetBool("Popup", true);
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
            Debug.Log(Mathf.Abs(menus_position_x - init_x));
            //Debug.Log(menus.GetComponent<RectTransform>().anchoredPosition.x);
            menus.GetComponent<RectTransform>().Translate(speed, 0, 0);
            menus_position_x = menus.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //これすることで画面に出力されるようになる
        }
        menu_moving = false;
        yield break;
    }

   
}
