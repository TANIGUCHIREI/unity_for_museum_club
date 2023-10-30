using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Button_On_Effect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Button_size_change);
    }

    // Update is called once per frame

    public void Button_size_change()
    {
        StartCoroutine(Button_size_change_Coroutine());
    }

    IEnumerator Button_size_change_Coroutine()
    {
        Vector3 init_size = gameObject.GetComponent<RectTransform>().localScale;
        float speed = 1.2f;
        while (gameObject.GetComponent<RectTransform>().localScale.x < 1.2* init_size.x)
        {
            //Ç‹Ç∏ägëÂ
            gameObject.GetComponent<RectTransform>().localScale += speed * Time.deltaTime * new Vector3(1f, 1f, 1f);
            yield return null;
        }

        while (gameObject.GetComponent<RectTransform>().localScale.x > init_size.x)
        {
            //éüÇ…èkè¨
            gameObject.GetComponent<RectTransform>().localScale -= speed * Time.deltaTime * new Vector3(1f, 1f, 1f);
            yield return null;
        }
    }
}
