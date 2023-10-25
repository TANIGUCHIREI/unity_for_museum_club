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
        Result_View.SetActive(true); //シーン遷移でnullreferenceされないように対策
        ThanksMessage.SetActive(true); 
        Results = GameObject.Find("Results");
        ThanksMessage.GetComponent<RectTransform>().anchoredPosition = new Vector3(Screen.width, 0, 0);
    }
    void Start()
    {
        //StartCoroutine(menu_move(speed:1000));//実際はClientManagerのFixedUpdate関数内で処理されます
        //Fake_Print_Finish_Arrive()はchange_wall.cs内のchange_3_to_result()で行っています
    }

    // Update is called once per frame
    public IEnumerator menu_move(float speed)
    {

        //menusはtransformはrectのほうが扱いやすい・・・というかそっちが解像度で管理されているのでそちらで処理します
        float init_x = Results.GetComponent<RectTransform>().anchoredPosition.x;
        float menus_position_x = init_x;
        Debug.Log("速度:" + speed);
        while (Mathf.Abs(menus_position_x - init_x) < Screen.width)
        {

            Results.GetComponent<RectTransform>().Translate(-speed*Time.deltaTime, 0, 0);
            menus_position_x = Results.GetComponent<RectTransform>().anchoredPosition.x;
            yield return null; //これすることで画面に出力されるようになる
        }

        yield break;
    }

    public void OnBackToHome()
    {
        GameObject Change_walls_UI = GameObject.Find("Change_walls_UI");
        Change_walls_UI.GetComponent<change_wall>().StartCoroutine(Change_walls_UI.GetComponent<change_wall>().change_Result_to_menu());
        //これでメニューへ戻る！・・・はず
    }


    
}
