using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes2D;
public class test : MonoBehaviour
{
    public GameObject kansai_button;
    public GameObject all_japan_button;
    public float  outlinesize = 10;

    bool kansai_check_is_on = true;
    bool all_japan_check_is_on = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnKansaicheck()
    {
        StartCoroutine(OnKansaiCheck());
    }

    public void OnAllJapancheck()
    {
        StartCoroutine(OnAllJapanCheck());
    }

    IEnumerator OnKansaiCheck()
    {
        float init_outlinesize = kansai_button.GetComponent<Shape>().settings.outlineSize;
        if (kansai_check_is_on == false)
        {
            while (kansai_button.GetComponent<Shape>().settings.outlineSize > 10f)
            {
                kansai_button.GetComponent<Shape>().settings.outlineSize -= 10f;
                all_japan_button.GetComponent<Shape>().settings.outlineSize += 10f;
                yield return null;
            }

            kansai_check_is_on = true;
            all_japan_check_is_on = false;
        }

        yield break;
        
    }


    IEnumerator OnAllJapanCheck()
    {
        float init_outlinesize = all_japan_button.GetComponent<Shape>().settings.outlineSize;
        if (all_japan_check_is_on == false)
        {
            while (all_japan_button.GetComponent<Shape>().settings.outlineSize > 10f)
            {
                all_japan_button.GetComponent<Shape>().settings.outlineSize -= 10f;
                kansai_button.GetComponent<Shape>().settings.outlineSize += 10f;
                yield return null;
            }

            kansai_check_is_on = false;
            all_japan_check_is_on = true;
        }

        yield break;

    }
}
