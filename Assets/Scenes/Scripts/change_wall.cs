using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class change_wall : MonoBehaviour
{

    public GameObject black_wall;
    public GameObject white_wall;
    public GameObject camera;
    

    // Start is called before the first frame update

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(camera);
    }
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }


    public void change_2_3()
    {
        //buttonではIEnumeratorがなんか見つからなかったからその対策用
        StartCoroutine(menu_change2_to_3());
    }
    IEnumerator menu_change2_to_3()
    {
        StartCoroutine(Relative_Line_move(obj: black_wall, 0, 10, 2)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, 10, 2));
        SceneManager.LoadScene("Gacha");
        camera.GetComponent<Camera>().orthographic = false; //これすることで並行図法から透視図法になる
        camera.GetComponent<Camera>().fieldOfView = 71;   //見た感じこれが最適解っぽい
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);

        yield return new WaitForSeconds(0.5f);
         StartCoroutine(Relative_Line_move(obj: white_wall, 0, 10, 2));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, 10, 2));
    }


    public void change_3_2()
    {
        //buttonではIEnumeratorがなんか見つからなかったからその対策用
        StartCoroutine(menu_change3_to_2());
    }
    IEnumerator menu_change3_to_2()
    {
        StartCoroutine(Relative_Line_move(obj: black_wall, 0, 10, 2)); //yield returnしないとyiledしないのですぐに下の処理が始まる
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: white_wall, 180, 10, 2));
        SceneManager.LoadScene("init_menu");
        camera.GetComponent<Camera>().orthographic = false; //これすることで並行図法から透視図法になる
        camera.GetComponent<Camera>().fieldOfView = 71;   //見た感じこれが最適解っぽい
        //camera.transform.position = new Vector3(0, 1.5f, 2.7f);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Relative_Line_move(obj: white_wall, 0, 10, 2));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Relative_Line_move(obj: black_wall, 180, 10, 2));
    }

    //ポケモン風戦闘シーンから持ってきたやつ
    IEnumerator Relative_Line_move(GameObject obj, float angle, float length, float mov_time)
    {


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
        yield break;
    }
}
