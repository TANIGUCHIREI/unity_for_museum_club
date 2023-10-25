using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class change_result_panel : MonoBehaviour
{
    public GameObject ThanksMessage;
    public GameObject Results;
    public GameObject Result_View;


    // Start is called before the first frame update
    private void Awake()
    {
        Result_View.SetActive(true); //�V�[���J�ڂ�nullreference����Ȃ��悤�ɑ΍�
        ThanksMessage.SetActive(true); 
        Results = GameObject.Find("Results");
        ThanksMessage.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
    }
    void Start()
    {
        //StartCoroutine(menu_move(speed:1000));//���ۂ�ClientManager��FixedUpdate�֐����ŏ�������܂�
        //Fake_Print_Finish_Arrive()��change_wall.cs����change_3_to_result()�ōs���Ă��܂�
    }

    // Update is called once per frame
    public IEnumerator menu_move(float speed)
    {

        //menus��transform��rect�̂ق��������₷���E�E�E�Ƃ��������������𑜓x�ŊǗ�����Ă���̂ł�����ŏ������܂�
        float init_x = Results.GetComponent<RectTransform>().anchoredPosition.x;
        float menus_position_x = init_x;
        Debug.Log("���x:" + speed);
        while (Mathf.Abs(menus_position_x - init_x) < Screen.width)
        {

            Results.GetComponent<RectTransform>().Translate(-speed*Time.deltaTime, 0, 0);
            menus_position_x = Results.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //���ꂷ�邱�Ƃŉ�ʂɏo�͂����悤�ɂȂ�
        }

        yield break;
    }

    public void OnBackToHome()
    {
        GameObject Change_walls_UI = GameObject.Find("Change_walls_UI");
        Change_walls_UI.GetComponent<change_wall>().StartCoroutine(Change_walls_UI.GetComponent<change_wall>().change_Result_to_menu());
        //����Ń��j���[�֖߂�I�E�E�E�͂�
    }


    
}
