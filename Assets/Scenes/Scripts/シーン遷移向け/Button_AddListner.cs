using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Button_AddListner : MonoBehaviour
{
    [SerializeField] 
    Button testBtn;
    public GameObject init_camera;
    public GameObject inputField;
    public GameObject JapanUI;
    //public GameObject Text;
    // Start is called before the first frame update
    void Start()
    {
        testBtn = gameObject.GetComponent<Button>(); //このボタンのこと
        GameObject changewall = GameObject.Find("Change_walls_UI");
        testBtn.onClick.AddListener(changewall.GetComponent<change_wall>().change_2_3); //これでシーン遷移でinit_menuに戻ったとしてもボタンのクリック時のスクリプトに登録される？
        init_camera = GameObject.Find("Init_Camera");
        inputField = GameObject.Find("InputField (TMP)");
        JapanUI = GameObject.Find("日本地図UI");
        //Text  = GameObject.Find("Text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnThisButtonCrick()
    {
        //Init_Camera側に記述されたテキストを送信する

        if(gameObject.name == "SearchButton2_1")
        {
            TMP_InputField input = inputField.GetComponent<TMP_InputField>();
            init_camera.GetComponent<ClientManager>().userinput_text = input.text; //テキストの値を反映

        }
        else if(gameObject.name == "SearchButton2_2")
        {
            init_camera.GetComponent<ClientManager>().userinput_text = "おまかせ"; //テキストの値を反映、2_2の場合はおまかせとなる
            //値がおまかせのときは、ClientManagerの操作で”おませでお願いします”みたいな入力に変換される
        }
        
        //Debug.Log(Text.GetComponent<Text>().text);
        //Debug.Log(input.text);
        init_camera.GetComponent<ClientManager>()._is_kansai_only = JapanUI.GetComponent<KansaiOrAllJapan>().kansai_check_is_on; //ボタンのTrueかFalseを反映
        init_camera.GetComponent<ClientManager>().async_SendPython(); //テストでどうなるか動かしてみる！
    }
}
