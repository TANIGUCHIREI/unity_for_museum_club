using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public AudioClip Gacha_Emit_sound;
    public AudioClip ElectroMagnetic_wave_sound;
    
    public AudioClip Gacha_Thinking_Dancing;
    public GameObject Lamps;
    public GameObject Finish_Lamp;
    AudioSource audioSource;
    GameObject Rotate_Arrow;
    public GameObject SpotLight;
    public List<string> QUERY = new List<string>() { "これは", "テスト用の", "クエリーです！！！", "まぁどんなものが", "くるのか", "わかりませんけど", "こまっちまう", "ネタ切れ", "のび太", "hogehoge", "querys" };
    //public List<string> QUERY = new List<string>() { "感嘆のためこれだけ！！！" };
    public GameObject Query_Text_Instance;
    public GameObject Gacha_capsule;
    public GameObject White_Blined_Circle; //リザルト画面への遷移用、ホワイト
    public GameObject Change_walls_UI;
    public bool _isQueryArrive = false;
    public bool _isPaperInsertFinish = false;
    public bool _isQueryShowEnd = false;
    public bool _isGacha_knob_Rotate_finish = false;

    public Material mySkyboxMaterial;
    void Start()
    {

        StartCoroutine(Rotate_skybox(speed: 0.5f)); //skyboxを回す！


        Init_Camera = GameObject.Find("Init_Camera"); //�͂��߂�������p���ꂽ���
        SpotLight = GameObject.Find("Directional Light");
        Change_walls_UI = GameObject.Find("Change_walls_UI");
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

        Gacha_capsule = GameObject.Find("Gacha_capsule");
        Gacha_capsule.GetComponent<Animator>().speed = 0;
        Gacha_capsule.GetComponent<Rigidbody>().useGravity = false;

        White_Blined_Circle = GameObject.Find("White_Blined_Circle");
        White_Blined_Circle.GetComponent<Animator>().speed = 0;

        //RenderSettings.skybox = mySkyboxMaterial; //背景のマテリアルを強制的に変更
        //DynamicGI.UpdateEnvironment();

        //SpotLight.GetComponent<Light>().intensity = 2f; //ガチャカプセルのモーションテスト用　ライトつけて見やすくします
        //camera0.GetComponent<Animator>().enabled = false; //テスト用、実際は動きます
        //StartCoroutine(Gacha_Capsule_Move()); //テスト用↑とこれをコメントアウト外してください
        StartCoroutine(Camera_Motion()); //これがメインで動きます


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

    IEnumerator Rotate_skybox(float speed)
    {
        float rot = 0f;
        
        while (true)
        {
            rot += speed * Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Rotation", rot);    //skyboxを回す
            yield return null;
        }
    }
    public IEnumerator Gacha_Capsule_Move()
    {

       


        float r = Random.Range(0.1f, 2); //なぜかわからんけどこんな変な値でいい感じの色になった・・・256だと真っ白、1fだと真っ黒なのに・・・意味わからんけど動くからいいや
        float g = Random.Range(0.1f, 2);
        float b = Random.Range(0.1f, 2);
        
        GameObject.Find("Sphere").GetComponent<Renderer>().sharedMaterial.color = new Color(r, g, b,1); //出るガチャの色が毎回変わります
        GameObject.Find("Sphere").GetComponent<Transform>().Rotate(new Vector3(0,0, Random.Range(-90, 90)));
        audioSource.PlayOneShot(Gacha_Emit_sound);
        Gacha_capsule.GetComponent<Rigidbody>().useGravity = false;


        Change_walls_UI.GetComponent<Animator>().speed = 1f; //初回動作移行は常に動くようになる！
        Change_walls_UI.GetComponent<Animator>().Play("LetterBox_show", 0, 0f); //映画風の演出スタート（上下の黒帯はレターボックスというらしい）


        yield return new WaitForSeconds(1f);

        Change_walls_UI.GetComponent<AudioSource>().clip = Change_walls_UI.GetComponent<change_wall>().Gacha_result_bgm;
        Change_walls_UI.GetComponent<AudioSource>().Play();//ガチャ出るときのかっこいい音を再生
        Change_walls_UI.GetComponent<AudioSource>().loop = false;

        
        

        yield return new WaitForSeconds(2f);
        Gacha_capsule.GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(0.1f);
        Gacha_capsule.GetComponent<Rigidbody>().AddForce(new Vector3(0, -200f, -3000f)); //若干初速を与える
        //Gacha_capsule.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0.1f, 0));
        yield return new WaitForSeconds(5.1f); //職人芸、これで多分大丈夫！
        Gacha_capsule.GetComponent<Rigidbody>().useGravity = false; //グラビティを個々で無効にする
        
       Debug.Log(Gacha_capsule.transform.position.y);

        float speed = 2f; // 1秒あたり0.5ユニットの速度で動かす、IEnumeratorで動作速度を一定にするための策
        while (Gacha_capsule.transform.position.y < 7f)
        {
            //Debug.Log(Gacha_capsule.transform.position.y);
            float moveAmount = speed * Time.deltaTime;
            Gacha_capsule.transform.position = Gacha_capsule.transform.position += new Vector3(0, moveAmount, 0);
            yield return null;

        }

        Gacha_capsule.GetComponent<Animator>().speed = 0.8f; //再生開始、若干おそめにするのがよいような
        yield return new WaitForSeconds(2f);
        White_Blined_Circle.GetComponent<Animator>().speed = 1; //これで画面真っ白になる！
        Change_walls_UI.GetComponent<Animator>().Play("Letter_Box_hide", 0, 0f); //これで黒帯を隠す！


        Change_walls_UI.GetComponent<change_wall>().StartCoroutine(Change_walls_UI.GetComponent<change_wall>().change_3_to_result()); //画面遷移の開始

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
            Instantiate_Query.GetComponent<TypefaceAnimator>().positionTo = new Vector3(0, Random.Range(0.7f, 2.5f), 0);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
        _isQueryShowEnd = true;
    }

    IEnumerator Fake_Querry_Arrive()
    {
        //standAloneMode時のみ使用する
        float WaitTime = Random.Range(5f, 10f);
        yield return new WaitForSeconds(WaitTime); //疑似的に待ち時間を作る
        _isQueryArrive = true;
        StartCoroutine(Fake_Answer_Arrive()); //疑似的な回答を作成開始
    }

    IEnumerator Fake_Answer_Arrive()
    {
        //standAloneMode時のみ使用する
        float WaitTime = Random.Range(10f, 20f);
        yield return new WaitForSeconds(WaitTime); //疑似的に待ち時間を作る
        Init_Camera.GetComponent<ClientManager>().prefecture = "鳥取県";
        Init_Camera.GetComponent<ClientManager>().museum_name = "なしっこ館あああああああああああああああああああああああああああああああああああああああああああああああ";
        Init_Camera.GetComponent<ClientManager>().exhibition_name = "常設展あああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああああ";
        Init_Camera.GetComponent<ClientManager>().exhibition_reason = "鳥取よいこと一度はおいで！";
        Init_Camera.GetComponent<ClientManager>()._isAnserArrive = true; //これで疑似的に回答が来たことにする

    }

    
    IEnumerator Camera_Motion()
    {
        
        camera0.GetComponent<Animator>().speed = 0;
        yield return new WaitForSeconds(10f);
        //yield return new WaitForSeconds(4.0f);
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
        audioSource.PlayOneShot(shutter_sound);
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
        yield return new WaitForSeconds(2f); //紙が落ちてくるまでの待ち時間
        camera0.GetComponent<Animator>().speed = 1;

        
        StartCoroutine(WaitAndShow_Query()); //ここでようやくクエリを店つことができる！
        Lamps.GetComponent<Animator>().speed = 1; //ランプが点灯して考えてる感じ出すの開始


        
        StartCoroutine(Gacha_Dancing()); //ガチャのダンス開始！
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

        GameObject Gacha_Knob = GameObject.Find("Gacha_Knob");
        Gacha_Knob.GetComponent<GachaKnob>().Gacha_Rotate_On = true; //これで回せるようになる！
        while (true)

        {

            if (_isGacha_knob_Rotate_finish == true)
            {
                break;
            }
            yield return null; //ユーザーがガチャノブを回すまで待つ
        }



        
        camera0.GetComponent<Animator>().speed = 1; //カメラ再生再開
        StartCoroutine(Gacha_Capsule_Move()); //ガチャをここで落とす
        //リザルト画面へのカメラを真っ白にする演出はGacha_Capsule_Move()内でやってます
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

    IEnumerator Gacha_Dancing()
    {

        float init_background_bgm_volume = Change_walls_UI.GetComponent<AudioSource>().volume;
        StartCoroutine(Change_walls_UI.GetComponent<change_wall>().VolumeDown(DownTo:0.05f)); //バックグラウンドの音量を小さく！

        GameObject Gacha = GameObject.Find("ガチャガチャ台");
        gameObject.GetComponent<AudioSource>().clip = Gacha_Thinking_Dancing;
        gameObject.GetComponent<AudioSource>().Play();

        Vector3 init_size = Gacha.GetComponent<Transform>().transform.localScale;
        Debug.Log(init_size);
        float speed = 0.15f;
        float Max_size = 1.1f;
        float Min_size = 0.9f;
        Vector3 normalized_Vector = new Vector3(0, 1, 0);
        while (!_isQueryShowEnd)
        {
            
            while(Gacha.GetComponent<Transform>().transform.localScale.y < Max_size * init_size.y)
            {
                Gacha.GetComponent<Transform>().transform.localScale += speed * Time.deltaTime*normalized_Vector;
                yield return null;
            }
            
            while(Gacha.GetComponent<Transform>().transform.localScale.y > Min_size*init_size.y)
            {
                Gacha.GetComponent<Transform>().transform.localScale -= speed * Time.deltaTime * normalized_Vector;
                yield return null;
            }

        }

        //クエリがきてダンシングを停止した後、元の大きさに戻す！
        gameObject.GetComponent<AudioSource>().Stop();
        //yield return new WaitForSeconds(0.5f);
        

        StartCoroutine(Change_walls_UI.GetComponent<change_wall>().VolumeUp()); //バックグラウンドミュージックの音量を徐々に戻す！

        if (Gacha.GetComponent<Transform>().transform.localScale.y > init_size.y)
        {
            while (Gacha.GetComponent<Transform>().transform.localScale.y > init_size.y)
            {
                Gacha.GetComponent<Transform>().transform.localScale -= speed * Time.deltaTime * normalized_Vector;
                yield return null;
            }
        }
        else
        {
            while (Gacha.GetComponent<Transform>().transform.localScale.y < init_size.y)
            {
                
                Gacha.GetComponent<Transform>().transform.localScale += speed * Time.deltaTime * normalized_Vector;
                yield return null;
            }
        }

        gameObject.GetComponent<AudioSource>().PlayOneShot(ElectroMagnetic_wave_sound);


    }
}
