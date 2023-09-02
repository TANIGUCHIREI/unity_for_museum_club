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
    public GameObject menu3;

    public float back_treshhold_time = 0;
    public bool menu_moving = false; //連打して動いちゃわない用のやつ

    // Start is called before the first frame update
    void Start()
    {
        menu1.SetActive(true);
        menu2_1.SetActive(false);
        menu2_2.SetActive(false);
        menu3.SetActive(true);
        menu2_1.transform.Translate(new Vector3(Screen.width,0,0));
        menu2_2.transform.Translate(new Vector3(Screen.width, 0, 0));
        menu3.transform.Translate(new Vector3(2*Screen.width, 0, 0));

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
        StartCoroutine(menu_move(-10));
    }

    public void Change_to_2_2()
    {
        menu2_2.SetActive(true);
        menu2_1.SetActive(false);
        StartCoroutine(menu_move(-10));
    }

    public void Change_to_3()
    {
        StartCoroutine(menu_move(-10));
    }

    public void menu_back()
    {
        if(menus.transform.position.x !<= 0)
        {
            //メニューが１ではなく2_1とか2_2とか3ならバックできる
            StartCoroutine(menu_move(10));
        }
        
    }

    public void OnbackButtonOn()
    {
        if(menu_moving == false && menus.transform.position.x! <= 0) {
            back_treshhold_time += 0.5f;
            if (back_treshhold_time > 1f)
            {
                menu_back();
                back_treshhold_time = 0;

            }
        }
        
    }

    IEnumerator menu_move(float speed)
    {
        menu_moving = true;
        float init_x = menus.transform.position.x;
        float menus_position_x = init_x;
        while (Mathf.Abs(menus_position_x - init_x) < Screen.width)
        {
            //Debug.Log(Mathf.Abs(menus_position_x - init_x));
            menus.transform.Translate(speed, 0, 0);
            menus_position_x = menus.transform.position.x;
            yield return null; //これすることで画面に出力されるようになる
        }
        menu_moving = false;
        yield break;
    }
}
