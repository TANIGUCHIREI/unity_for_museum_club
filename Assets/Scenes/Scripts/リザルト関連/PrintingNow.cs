using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PrintingNow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Blinking_Text_WawitAndStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Blinking_Text_WawitAndStart()
    {
        StartCoroutine(Blinking_Text());
    }
    IEnumerator Blinking_Text()
    {
        yield return new WaitForSeconds(8f); //すべてが表示されるまでの待ち時間
        gameObject.GetComponent<Animator>().enabled = true; //これで印刷しています・・・がチカチカするようになる
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if(gameObject.GetComponent<TMP_Text>().text == "結果を印刷しています")
            {
                gameObject.GetComponent<TMP_Text>().text = "結果を印刷しています・";
            }
            else if (gameObject.GetComponent<TMP_Text>().text == "結果を印刷しています・")
            {
                gameObject.GetComponent<TMP_Text>().text = "結果を印刷しています・・";
            }
            else if (gameObject.GetComponent<TMP_Text>().text == "結果を印刷しています・・")
            {
                gameObject.GetComponent<TMP_Text>().text = "結果を印刷しています・・・";
            }
            else if (gameObject.GetComponent<TMP_Text>().text == "結果を印刷しています・・・")
            {
                gameObject.GetComponent<TMP_Text>().text = "結果を印刷しています";
            }

            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
