using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User_Input : MonoBehaviour
{
    public string userinput_text = "���܂���"; //�����ݒ�
    public bool _is_kansai_only = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset_Inputs()
    {
        userinput_text = "���܂���";
        _is_kansai_only = true;
    }
}
