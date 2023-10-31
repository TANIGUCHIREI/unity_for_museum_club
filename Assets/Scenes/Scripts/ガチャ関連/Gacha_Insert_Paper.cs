using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows;
using Unity.VisualScripting;

public class Gacha_Insert_Paper : MonoBehaviour
{
    // Start is called before the first frame update
    public bool Start_Write_Text = false;
    bool alredy_working = false;
    GameObject User_Input_Text;
    GameObject Gacha_Event;
    public AudioClip Inserting_Machine_Sound;
    
    public string Text; //Text��change_walls.cs��menu�Q����R�ɑJ�ڂ���Ƃ��Ƀ��[�U�[�C���v�b�g�ɕς��
    void Start()
    {
        User_Input_Text = GameObject.Find("User_Input_Text");
        Gacha_Event = GameObject.Find("Gacha_Event");

        GameObject Init_Camera = GameObject.Find("Init_Camera"); //������Ȃ��Ă��i�݂܂�

        try
        {
            if (Init_Camera.GetComponent<ClientManager>()._isStandAloneModeOne)
            {
                //�����X�^���h�A�������[�h�Ȃ�ӂ[�̓�����s���i�����瑤�͕ς���K�v���Ȃ�����j
                User_Input_Text.GetComponent<TMP_Text>().text = ""; //�͂��߂͉��������Ȃ��悤�ɂ���

            }
            else
            {
                User_Input_Text.GetComponent<TMP_Text>().text = ""; //�͂��߂͉��������Ȃ��悤�ɂ���
                //�X�^���h�A�������[�h�łȂ��Ă����̏����͕K�v�I
            }
            //�X�^���h�A�������[�h�łȂ��Ȃ�try�ł����Acatch�͓��삹��Update�ɑ��s
        }
        catch (System.NullReferenceException)
        {
            //NullReference�̏ꍇ�A�܂�Gacha�̊J���i�K�ł��̃V�[�����瓮�����Ă���ꍇ
            Debug.Log("NullReference�Ȃ̂�Gacha��ʂ���̊J�n���Ǝv���}�X�B���̂��ߏ��߂ɋL�q����Ă������͂�\�����܂�");
            Text = User_Input_Text.GetComponent<TMP_Text>().text;
            User_Input_Text.GetComponent<TMP_Text>().text = ""; //���߂��珑����Ă��镶��
        }

        
        //Text = User_Input_Text.GetComponent<TMP_Text>().text;


    }

    private void FixedUpdate()
    {
        
        if (Start_Write_Text && alredy_working == false)
        {
            StartCoroutine(Write_Text());
            Start_Write_Text = false;
            alredy_working =true;
        }
        
    
    }
    // Update is called once per frame
    IEnumerator Write_Text()
    {
        //Start�i�j���ɏ�������ClientManager�Ɗ����Ă��������Ȃ��Ă��܂����̂ŁAClientManager�ɏ��������Ă�����Ă��炱����̂ق��ŕύX���邱�Ƃɂ���
        
        

        GetComponent<Animator>().speed = 0; //�܂��̓A�j���[�V�����̓�����~
        gameObject.GetComponent<AudioSource>().Play();
        string[] characters = Text.Select(c => c.ToString()).ToArray();
        foreach (string word in characters)
        {
            Debug.Log(word);
            User_Input_Text.GetComponent<TMP_Text>().text += word; //�������ꕶ�����ǉ����Ă���
            yield return new WaitForSeconds(0.1f);
        }

        gameObject.GetComponent<AudioSource>().Stop(); //�^�C�s���O���Đ���~
        yield return new WaitForSeconds(1f); //�������͂��I�����āA�C���T�[�g�����܂ł̎���
        GetComponent<Animator>().speed = 1; //�A�j���[�V�����ĊJ
        yield return new WaitForSeconds(2f); //�����Ȃ�܂ł̎���
        gameObject.GetComponent<AudioSource>().clip = Inserting_Machine_Sound;
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(4f); //�C���T�[�g�n�܂��Ă���҂���


        Gacha_Event.GetComponent<Gacha_Controller>()._isPaperInsertFinish = true;
        yield return new WaitForSeconds(2.5f);
        gameObject.GetComponent<AudioSource>().Stop();
    }
}
