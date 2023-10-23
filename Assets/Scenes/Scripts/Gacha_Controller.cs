using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha_Controller : MonoBehaviour
{
    public GameObject camera0;
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;

    public AudioClip shutter_sound;
    public AudioClip SpotLight_sound;

    AudioSource audioSource;

    public GameObject SpotLight;
    public List<string> QUERY = new List<string>() { "これは", "テスト用の", "クエリーです！！！", "まぁどんなものが", "くるのか", "わかりませんけど", "こまっちまう", "ネタ切れ", "のび太", "hogehoge", "querys" };

    public GameObject Query_Text_Instance;


    void Start()
    {
        camera0 = GameObject.Find("Init_Camera"); //�͂��߂�������p���ꂽ���
        SpotLight = GameObject.Find("Directional Light");
        SpotLight.GetComponent<Light>().intensity = 0f;
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(false);


        audioSource = GetComponent<AudioSource>();

        StartCoroutine(Camera_Motion());
        //StartCoroutine(Create_Query_Text_Bbble());
    }

    // Update is called once per frame
    //writed in vsc on lab's PC
    void Update()
    {
        
    }

    public IEnumerator Create_Query_Text_Bbble()
    {
  
        Debug.Log("Create_Queryは動いています");
        foreach (string query in QUERY)
        {
            GameObject Instantiate_Query = Instantiate(Query_Text_Instance);
            Instantiate_Query.GetComponent<Query_Text>().query_text = query;
            Instantiate_Query.transform.parent = GameObject.Find("Query_Canvas").transform;
            float Scale = Random.Range(7f, 15f);
            Instantiate_Query.GetComponent<RectTransform>().localScale = new Vector3(Scale, Scale, Scale);
            Instantiate_Query.GetComponent<TypefaceAnimator>().positionTo = new Vector3(0, Random.Range(2f, 5f), 0);
            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }
    IEnumerator Camera_Motion()
    {
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
    }
}
