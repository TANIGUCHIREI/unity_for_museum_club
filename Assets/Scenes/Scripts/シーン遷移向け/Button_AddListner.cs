using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_AddListner : MonoBehaviour
{
    [SerializeField] 
    Button testBtn;

    // Start is called before the first frame update
    void Start()
    {
        testBtn = gameObject.GetComponent<Button>();
        GameObject changewall = GameObject.Find("Change_walls_UI");
        testBtn.onClick.AddListener(changewall.GetComponent<change_wall>().change_2_3); //これでシーン遷移でinit_menuに戻ったとしてもボタンに登録される？
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
