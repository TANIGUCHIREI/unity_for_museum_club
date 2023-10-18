using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Canvas_FindCamera : MonoBehaviour
{
    // Start is called before the first frame update


    void Awake()
    {
        try {
            Camera cam = GameObject.Find("Init_Camera").GetComponent<Camera>();
            gameObject.GetComponent<Canvas>().worldCamera = cam;
        }catch (System.NullReferenceException)
        {
            Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            gameObject.GetComponent<Canvas>().worldCamera = cam;
            Debug.Log("Init_Camera�������̂Ńf�t�H���g�̃J�������g���܂�");
        }

        if (SceneManager.GetActiveScene().name == "init_menu")
        {
            //�{�^���Ȃǂ̃��X�i�[�̒ǉ��ݒ�������ł��Ă���
            GameObject Gear = GameObject.Find("Gear");
            GameObject Change_walls_UI = GameObject.Find("Change_walls_UI");
            Gear.GetComponent<Button>().onClick.AddListener(Change_walls_UI.GetComponent<change_wall>().change_2_setting); 
            //�����ł���̂ŁA�͂��߂���Button�����Ă��܂��Ă���ƃC���X�y�N�^��ɕ\������Ă��Ȃ��Ă����ۂ͂��Ă���I

        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
