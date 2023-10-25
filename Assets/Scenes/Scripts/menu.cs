using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

public class menu : MonoBehaviour
{
    public GameObject menus;
    public GameObject menu1;
    public GameObject menu2_1;
    public GameObject menu2_2;
    //public GameObject menu3; //�����g��Ȃ�

    public GameObject kansai_obj;
    public GameObject other_japan_obj;

    public GameObject kansainomi;
    public GameObject zenkoku;

    public GameObject japan_3Dmap;
    //public GameObject PopUpwindow;
    public GameObject change_walls;

    public GameObject init_camera;
    public GameObject User_Input_Field;

    public float back_treshhold_time = 0;
    public bool menu_moving = false; //�A�ł��ē��������Ȃ��p�̂��

    public float ViewportScreenWidth;
    //public GameObject cameraobj;
    //public Camera cam;
    public float speed = 0;

    public GameObject Canvas_for_Confirmation;
    public GameObject Input_Confirmation;
    public bool _isOmakase;
    public bool _isKansaiOnly = true;


    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 60; //����őS�̓I�ɌŒ肳���Ƃ��H�����ۂɌŒ肳�ꂽ�I�K����x�͒ʂ邱�Ƃ�ɂ����u���Ă�����OK

        menu1.SetActive(true);
        menu2_1.SetActive(false);
        menu2_2.SetActive(false);
        japan_3Dmap.SetActive(true);
        Canvas_for_Confirmation.SetActive(true);
        Canvas_for_Confirmation.GetComponent<Animator>().SetFloat("speed", 0); //multiple����l��0�ɂ��邱�ƂŎ������x��0�A�܂�X�g�b�v������
        //menu3.SetActive(true);

        //cameraobj = GameObject.Find("Main Camera");
        //cam = cameraobj.GetComponent<Camera>();
        //ViewportScreenWidth =  cam.ScreenToViewportPoint(new Vector3(Screen.width, 0, 0)).x;
        menu2_1.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
        //menu2_1.transform.Translate(new Vector3(Screen.width, 0,0));
        Debug.Log(menu2_1.GetComponent<RectTransform>().anchoredPosition.x);
        menu2_2.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
        Vector3 init_japn_pos = japan_3Dmap.GetComponent<RectTransform>().anchoredPosition;
        japan_3Dmap.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0) + init_japn_pos;
        //menu3.GetComponent<RectTransform>().anchoredPosition = new Vector3(2*Screen.width, 0, 0);
        //���ʂ�transform.position�w��ł�rectTransform�̒l���S�����������Ȃ��Ă��܂����̂ŁARectTransform���g�p����

        zenkoku.GetComponent<Animator>().SetBool("transparent", true); //����őS���Ƃ����e�L�X�g���ŏ��ɔ������ɂ��Ă���I

        change_walls = GameObject.Find("change_walls"); //�Ȃ񂩕��[��serialized fielnd�������Ă���ƁA�V�[���J�ڂ̒i�K�ŏ����Ă��܂�
        init_camera = GameObject.Find("Init_Camera");

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(back_treshhold_time > 0)
        {
            back_treshhold_time -= 0.03f;
            //Debug.Log(back_treshhold_time);
        }
    }

    public void OnSearchButtonOn()
    {
        string Input_Confirmation_text = "";
        if (_isKansaiOnly)
        {
            Input_Confirmation_text += "�֐��̓W����\n";
        }
        else
        {
            Input_Confirmation_text += "�S���̓W����\n";
        }

        if (_isOmakase)
        {
            Input_Confirmation_text += "�u���܂����v\n";
        }
        else
        {
            Input_Confirmation_text += "�u" + User_Input_Field.GetComponent<TMP_InputField>().text + "�v\n";
        }

        Input_Confirmation_text += "�Ƃ������e�ŃK�`�����񂵂܂��B\n��낵���ł����H";
        Input_Confirmation.GetComponent<TMP_Text>().text = Input_Confirmation_text;

        Canvas_for_Confirmation.GetComponent<Animator>().SetFloat("speed", 1); //����Ń|�b�v�A�b�v�̃A�j���[�V���������삵�܂�
        Canvas_for_Confirmation.GetComponent<Animator>().Play("Menu_Confirmation", 0, 0f); //�͂��߂��瓮�삳����
    }

    public void OnConfirmationBackOn()
    {
        

        Canvas_for_Confirmation.GetComponent<Animator>().SetFloat("speed", -1); //����ōĐ����t�����ɂ���E�E�E�H
        Canvas_for_Confirmation.GetComponent<Animator>().Play("Menu_Confirmation", 0,1f); //1f�͍Ō�Ƃ������ƁE�Ōォ��t�����i�|�P�j�ɍĐ�����
        //Canvas_for_Confirmation.SetActive(false); 
    }

    public void Change_to_2_1()
    {
        _isOmakase = false;
        menu2_1.SetActive(true);
        menu2_2.SetActive(false);
        StartCoroutine(menu_move(-speed));
    }

    public void Change_to_2_2()
    {
        _isOmakase = true;
        menu2_2.SetActive(true);
        menu2_1.SetActive(false);
        StartCoroutine(menu_move(-speed));
    }

    public void Change_to_3()
    {
        //StartCoroutine(menu_move(-10));
        
    }

    public void menu_back()
    {
        if(menus.GetComponent<RectTransform>().anchoredPosition.x! <-100)
        {
            //���j���[���P�ł͂Ȃ�2_1�Ƃ�2_2�Ƃ�3�Ȃ�o�b�N�ł���
            //speed��translate�����ē�������A���ۂɂ͎኱���W������邱�Ƃɒ���
            StartCoroutine(menu_move(speed));
        }
        
    }

    public void OnbackButtonOn()
    {

        if (SceneManager.GetActiveScene().name == "init_menu")
        {
            if (menu_moving == false && menus.transform.position.x! <= 0)
            {
                back_treshhold_time += 0.5f;
                if (back_treshhold_time > 1f)
                {
                    menu_back();
                    back_treshhold_time = 0;

                }
            }
        }
       
        
    }

    public void OnKansaiOnlyOn()
    {
        _isKansaiOnly = true;
        kansai_obj.GetComponent<Animator>().SetBool("pop", true);
        other_japan_obj.GetComponent<Animator>().SetBool("pop", false);


        kansainomi.GetComponent<Animator>().SetBool("transparent", false);
        zenkoku.GetComponent<Animator>().SetBool("transparent", true);
        init_camera.GetComponent<User_Input>()._is_kansai_only = true; 

    }

    public void OnAllJapnOn()
    {
        _isKansaiOnly = false;
        
        kansai_obj.GetComponent<Animator>().SetBool("pop", false);
        other_japan_obj.GetComponent<Animator>().SetBool("pop", true);

        kansainomi.GetComponent<Animator>().SetBool("transparent", true);
        zenkoku.GetComponent<Animator>().SetBool("transparent", false);

        init_camera.GetComponent<User_Input>()._is_kansai_only = false;



    }

  

    IEnumerator menu_move(float speed)
    {
        menu_moving = true;
        //menus��transform��rect�̂ق��������₷���E�E�E�Ƃ��������������𑜓x�ŊǗ�����Ă���̂ł�����ŏ������܂�
        float init_x = menus.GetComponent<RectTransform>().anchoredPosition.x;
        float menus_position_x = init_x;
        Debug.Log("���x:" +  speed);
        while (Mathf.Abs(menus_position_x - init_x) < Screen.width)
        {
            //Debug.Log(Mathf.Abs(menus_position_x - init_x));
            //Debug.Log(menus.GetComponent<RectTransform>().anchoredPosition.x);
            menus.GetComponent<RectTransform>().Translate(speed, 0, 0);
            menus_position_x = menus.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //���ꂷ�邱�Ƃŉ�ʂɏo�͂����悤�ɂȂ�
        }
        menu_moving = false;
        yield break;
    }

   
}
