using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class cahngewall_3to2 : MonoBehaviour
{
    public GameObject black_wall;
    public GameObject white_wall;

    public GameObject init_camera;
    public float back_treshhold_time = 0;
    public bool menu_moving = false; //連打して動いちゃわない用のやつ
    // Start is called before the first frame update
    void Start()
    {
        black_wall = GameObject.Find("black_wall");
        init_camera = GameObject.Find("Init_Camera");
        white_wall = GameObject.Find("white_wall");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (back_treshhold_time > 0)
        {
            back_treshhold_time -= 0.03f;
            Debug.Log(back_treshhold_time);
        }
    }


    public void OnbackButtonOn()
    {
        Debug.Log("Back");
        if (SceneManager.GetActiveScene().name == "Gacha")
        {
            Debug.Log("Gacha");
            back_treshhold_time += 0.5f;
            if (menu_moving == false && back_treshhold_time > 1f)
            {
                change_3_2();
                back_treshhold_time = 0;

            }
        }


    }
    public void change_3_2()
    {
        //buttonではIEnumeratorがなんか見つからなかったからその対策用
        //camera.SetActive(false);
        //camera.SetActive(true);
        //コルーチン内ではどんなSetActiveもしちゃだめっぽい？
        StartCoroutine(menu_change3_to_2());
    }
    IEnumerator menu_change3_to_2()
    {
        menu_moving = true;
        
        yield return null;
        Debug.Log("yield return nullが動いた！！");
        StartCoroutine(Relative_Line_move(obj: black_wall, 0, Screen.width + 100, 2)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        
        yield return new WaitForSeconds(0.5f);
        Debug.Log("0.5f passed");
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, Screen.width + 100, 2));
        Debug.Log("さあここからLoadScene!!!");
        SceneManager.LoadScene("init_menu");
        //Destroy(GameObject.Find("Init_Camera")); //初めのやつを消去？
        init_camera.GetComponent<Camera>().orthographic = true; //これすることで並行図法から透視図法になる
        init_camera.GetComponent<User_Input>().Reset_Inputs(); //これでインプットをリセット!
        //camera.GetComponent<Camera>().fieldOfView = 71;   //見た感じこれが最適解っぽい
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Relative_Line_move(obj: white_wall, 0, Screen.width + 100, 2));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, Screen.width + 100, 2));
        menu_moving = false;
    }

    IEnumerator Relative_Line_move(GameObject obj, float angle, float length, float mov_time)
    {

        Debug.Log(obj.name);
        Vector2 init_pos = obj.transform.position;
        float mov_velocity = length / (mov_time * (1 / Time.deltaTime));
        float radian_angle = angle * Mathf.PI / 180;
        Vector2 normalized_direction = new Vector2(Mathf.Cos(radian_angle), Mathf.Sin(radian_angle));
        float now_length = 0;
        while (now_length < length)
        {
            mov_velocity = length / (mov_time * (1 / Time.deltaTime));
            Vector2 my_position = obj.transform.position;
            obj.transform.position = my_position + mov_velocity * normalized_direction;
            now_length = Vector2.Distance(init_pos, obj.transform.position);
            yield return null; //これすることで画面に出力されるようになる
        }
        Debug.Log( obj.name +" moved");
        yield break;
    }
}
