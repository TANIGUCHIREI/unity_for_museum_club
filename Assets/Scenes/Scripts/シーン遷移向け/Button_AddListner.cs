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
        testBtn = gameObject.GetComponent<Button>(); //���̃{�^���̂���
        GameObject changewall = GameObject.Find("Change_walls_UI");
        testBtn.onClick.AddListener(changewall.GetComponent<change_wall>().change_2_3); //����ŃV�[���J�ڂ�init_menu�ɖ߂����Ƃ��Ă��{�^���̃N���b�N���̃X�N���v�g�ɓo�^�����H
        init_camera = GameObject.Find("Init_Camera");
        inputField = GameObject.Find("InputField (TMP)");
        JapanUI = GameObject.Find("���{�n�}UI");
        //Text  = GameObject.Find("Text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnThisButtonCrick()
    {
        //Init_Camera���ɋL�q���ꂽ�e�L�X�g�𑗐M����

        if(gameObject.name == "SearchButton2_1")
        {
            TMP_InputField input = inputField.GetComponent<TMP_InputField>();
            init_camera.GetComponent<ClientManager>().userinput_text = input.text; //�e�L�X�g�̒l�𔽉f

        }
        else if(gameObject.name == "SearchButton2_2")
        {
            init_camera.GetComponent<ClientManager>().userinput_text = "���܂���"; //�e�L�X�g�̒l�𔽉f�A2_2�̏ꍇ�͂��܂����ƂȂ�
            //�l�����܂����̂Ƃ��́AClientManager�̑���Łh���܂��ł��肢���܂��h�݂����ȓ��͂ɕϊ������
        }
        
        //Debug.Log(Text.GetComponent<Text>().text);
        //Debug.Log(input.text);
        init_camera.GetComponent<ClientManager>()._is_kansai_only = JapanUI.GetComponent<KansaiOrAllJapan>().kansai_check_is_on; //�{�^����True��False�𔽉f
        init_camera.GetComponent<ClientManager>().async_SendPython(); //�e�X�g�łǂ��Ȃ邩�������Ă݂�I
    }
}
