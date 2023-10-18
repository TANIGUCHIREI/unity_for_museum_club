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
    //public GameObject Text;
    // Start is called before the first frame update
    void Start()
    {
        testBtn = gameObject.GetComponent<Button>(); //このボタンのこと
        GameObject changewall = GameObject.Find("Change_walls_UI");
        testBtn.onClick.AddListener(changewall.GetComponent<change_wall>().change_2_3); //これでシーン遷移でinit_menuに戻ったとしてもボタンのクリック時のスクリプトに登録される？
        init_camera = GameObject.Find("Init_Camera");
        inputField = GameObject.Find("InputField (TMP)");
        //Text  = GameObject.Find("Text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnThisButtonCrick()
    {
        //Init_Camera側に記述されたテキストを送信する
        TMP_InputField  input = inputField.GetComponent<TMP_InputField>();
        //Debug.Log(Text.GetComponent<Text>().text);
        //Debug.Log(input.text);
        init_camera.GetComponent<User_Input>().userinput_text = input.text;

        init_camera.GetComponent<ClientManager>().OnSendPython(); //テストでどうなるか動かしてみる！
    }
}
