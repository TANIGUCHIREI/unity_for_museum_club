using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SearchButton2_1 : MonoBehaviour
{

    public GameObject user_input_obj;
    string user_input_Text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        user_input_Text = user_input_obj.GetComponent<TMP_InputField>().text;
        if (user_input_Text == "")
        {
            //文字が何も入力されていなかったら
            gameObject.GetComponent<Button>().interactable = false;
            gameObject.GetComponentInChildren<TMP_Text>().text = "入力がありません";
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
            gameObject.GetComponentInChildren<TMP_Text>().text = "これで検索！";
        }
    }
}
