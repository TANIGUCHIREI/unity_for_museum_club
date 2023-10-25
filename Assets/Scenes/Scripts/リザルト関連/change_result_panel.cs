using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class change_result_panel : MonoBehaviour
{
    public GameObject ThanksMessage;
    public GameObject Results;

    // Start is called before the first frame update
    private void Awake()
    {
        ThanksMessage = GameObject.Find("ThanksMessage");
        Results = GameObject.Find("Results");
        ThanksMessage.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
    }
    void Start()
    {
        //StartCoroutine(menu_move(speed:2));���ۂ�ClientManager��FixedUpdate�֐����ŏ�������܂�
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

            Results.GetComponent<RectTransform>().Translate(-speed, 0, 0);
            menus_position_x = Results.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //���ꂷ�邱�Ƃŉ�ʂɏo�͂����悤�ɂȂ�
        }

        yield break;
    }

}
