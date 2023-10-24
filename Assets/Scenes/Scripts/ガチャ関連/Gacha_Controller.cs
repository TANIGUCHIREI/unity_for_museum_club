using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha_Controller : MonoBehaviour
{
    public GameObject Init_Camera;
    public GameObject camera0;
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;
    public GameObject Gacha_Insert_Paper;
    public AudioClip shutter_sound;
    public AudioClip SpotLight_sound;
    public GameObject Lamps;
    public GameObject Finish_Lamp;
    AudioSource audioSource;
    GameObject Rotate_Arrow;
    public GameObject SpotLight;
    //public List<string> QUERY = new List<string>() { "これは", "テスト用の", "クエリーです！！！", "まぁどんなものが", "くるのか", "わかりませんけど", "こまっちまう", "ネタ切れ", "のび太", "hogehoge", "querys" };
    public List<string> QUERY = new List<string>() { "感嘆のためこれだけ！！！" };
    public GameObject Query_Text_Instance;

    public bool _isQueryArrive = false;
    public bool _isPaperInsertFinish = false;
    public bool _isQueryShowEnd = false;
    public bool _isGacha_knob_Rotate_finish = false;
    void Start()
    {
        Init_Camera = GameObject.Find("Init_Camera"); //�͂��߂�������p���ꂽ���
        SpotLight = GameObject.Find("Directional Light");
        SpotLight.GetComponent<Light>().intensity = 0f;
        camera0.SetActive(true);
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(false);
        //Gacha_Insert_Paper.SetActive(false);
        Gacha_Insert_Paper.GetComponent<SpriteRenderer>().enabled = false;
        Gacha_Insert_Paper.GetComponent<Animator>().speed = 0; //setactive(false)だとlientMangagerからの入力でエラーが発生した！そのためスプライト消す＆スピードを０にする
        Lamps.GetComponent<Animator>().speed = 0;
        audioSource = GetComponent<AudioSource>();
        Rotate_Arrow = GameObject.Find("Rotate_Arrow");
        Rotate_Arrow.GetComponent<Animator>().speed = 0;


        camera0.GetComponent<Animator>().enabled = false;
        //StartCoroutine(Camera_Motion());


        try
        {
            if (Init_Camera.GetComponent<ClientManager>()._isStandAloneModeOne)
            {
                //もしスタンドアロンモードなら、疑似的なクエリを動かしてしまう
                StartCoroutine(Fake_Querry_Arrive());
            }
            //スタンドアロンモードでないならtryでおわり、catchは動作せずUpdateに続行
        }
        catch (System.NullReferenceException)
        {
            //NullReferenceの場合、つまりGachaの開発段階でこのシーンから動かしている場合
            Debug.Log("NullReferenceなのでGacha画面からの開始だと思われマス。そのためプリセットのクエリを表示させます");
            StartCoroutine(Fake_Querry_Arrive());
        }


    }

    // Update is called once per frame
    //writed in vsc on lab's PC
    void FixedUpdate()
    {
        
    }

    public IEnumerator Create_Query_Text_Bbble()
    {
  
        Debug.Log("Create_Queryは動いています");
        foreach (string query in QUERY)
        {
            string fixed_query = query.Replace("\"", ""); //クエリになぜか"がついてしまうのでこれで置き換え
            GameObject Instantiate_Query = Instantiate(Query_Text_Instance);
            Instantiate_Query.GetComponent<Query_Text>().query_text = fixed_query;
            Instantiate_Query.transform.parent = GameObject.Find("Query_Canvas").transform;
            float Scale = Random.Range(7f, 15f);
            Instantiate_Query.GetComponent<RectTransform>().localScale = new Vector3(Scale, Scale, Scale);
            Instantiate_Query.GetComponent<TypefaceAnimator>().positionTo = new Vector3(0, Random.Range(2f, 5f), 0);
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
        _isQueryShowEnd = true;
    }

    IEnumerator Fake_Querry_Arrive()
    {
        //standAloneMode時のみ使用する
        float WaitTime = Random.Range(5f, 10f);
        yield return new WaitForSeconds(WaitTime); //疑似的に待ち時間を作る
        _isQueryArrive = true;

    }
    IEnumerator Camera_Motion()
    {
        camera0.GetComponent<Animator>().speed = 0;
        yield return new WaitForSeconds(4.0f);
        audioSource.PlayOneShot(SpotLight_sound);
        SpotLight.GetComponent<Light>().intensity = 2f;
        yield return new WaitForSeconds(2.0f);
        audioSource.PlayOneShot(shutter_sound);
        camera1.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(shutter_sound);
        camera2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(shutter_sound);
        camera3.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(shutter_sound);
        camera4.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        camera0.SetActive(false);
        camera0.SetActive(true);
        camera0.GetComponent<Animator>().speed = 1;
        yield return new WaitForSeconds(10f); //上を見るまでの待ち時間
        camera0.GetComponent<Animator>().speed = 0;
        //Gacha_Insert_Paper.SetActive(true); //紙が落ちるの開始
        Gacha_Insert_Paper.GetComponent<SpriteRenderer>().enabled = true; //setactive(false)だとlientMangagerからの入力でエラーが発生した！そのためスプライト消す＆スピードを０にする
        Gacha_Insert_Paper.GetComponent<Animator>().speed = 1;
        
        while (true)
        {
            if(_isPaperInsertFinish == true)
            {
                break;
            }
            yield return null; //紙が落ちるのを待つ
        }
        yield return new WaitForSeconds(3f); //紙が落ちてくるまでの待ち時間
        camera0.GetComponent<Animator>().speed = 1;
        StartCoroutine(WaitAndShow_Query()); //ここでようやくクエリを店つことができる！
        Lamps.GetComponent<Animator>().speed = 1; //ランプが点灯して考えてる感じ出すの開始
        while (true)
        {
            if (_isQueryShowEnd == true)
            {
                break;
            }
            yield return null; //クエリが表示されるのが終わるのを待つ
        }

        Animator Lamp_anim = Lamps.GetComponent<Animator>();
        Lamp_anim.Play("Lamp_Finish", 0, 0f); //クエリ表示完了ランプをここで表示！

        Animator camera0_anim = camera0.GetComponent<Animator>();
        camera0_anim.Play("camera0_after_Query_show");
        yield return new WaitForSeconds(4.5f); //カメラがガチャに近づくまでの時間、近づいたら止める
        camera0.GetComponent<Animator>().speed = 0;

        Rotate_Arrow.GetComponent<Animator>().speed = 1; //回転再生

        while (true)
        {
            if (_isGacha_knob_Rotate_finish == true)
            {
                break;
            }
            yield return null; //ユーザーがガチャノブを回すまで待つ
        }

    }

    IEnumerator WaitAndShow_Query()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (_isQueryArrive)
            {
                Debug.Log("クエリ表示スタートします");
                StartCoroutine(Create_Query_Text_Bbble());
                _isQueryArrive = false;
                break;
            }
            yield return null; //クエリが到着するのを待つ
        }

        

    }
}
