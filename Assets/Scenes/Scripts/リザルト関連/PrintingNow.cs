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
        yield return new WaitForSeconds(8f); //���ׂĂ��\�������܂ł̑҂�����
        gameObject.GetComponent<Animator>().enabled = true; //����ň�����Ă��܂��E�E�E���`�J�`�J����悤�ɂȂ�
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if(gameObject.GetComponent<TMP_Text>().text == "���ʂ�������Ă��܂�")
            {
                gameObject.GetComponent<TMP_Text>().text = "���ʂ�������Ă��܂��E";
            }
            else if (gameObject.GetComponent<TMP_Text>().text == "���ʂ�������Ă��܂��E")
            {
                gameObject.GetComponent<TMP_Text>().text = "���ʂ�������Ă��܂��E�E";
            }
            else if (gameObject.GetComponent<TMP_Text>().text == "���ʂ�������Ă��܂��E�E")
            {
                gameObject.GetComponent<TMP_Text>().text = "���ʂ�������Ă��܂��E�E�E";
            }
            else if (gameObject.GetComponent<TMP_Text>().text == "���ʂ�������Ă��܂��E�E�E")
            {
                gameObject.GetComponent<TMP_Text>().text = "���ʂ�������Ă��܂�";
            }

            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
