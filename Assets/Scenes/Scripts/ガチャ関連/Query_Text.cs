using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Query_Text : MonoBehaviour
{
    // Start is called before the first frame update
    //https://ugcj.com/unity%E3%81%AEui%E3%82%92%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%97%E3%83%88%E3%81%8B%E3%82%89%E6%93%8D%E4%BD%9C%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95/
    //Å™RecttransformÇÕÇ¢ÇÎÇÒÇ»positionÇ™Ç†Ç¡ÇƒÇﬂÇÒÇ«Ç¢Ç»Ç±ÇÍÇÕ

    public string query_text;
    void Start()
    {
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(Random.Range(-2000, 2000), -5000, Random.Range(-300, 6000));
        gameObject.GetComponent<Text>().text = query_text;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Translate(new Vector3(0, 0.05f, 0));
        //Debug.Log(gameObject.GetComponent<RectTransform>().localPosition.y); Ç±ÇÍÇ≈Ç®Çã
        if (gameObject.GetComponent<RectTransform>().localPosition.y > 8000)
        {
            Destroy(gameObject);
        }
    }
}
