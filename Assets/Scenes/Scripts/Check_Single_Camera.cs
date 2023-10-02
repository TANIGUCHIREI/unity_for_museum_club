using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check_Single_Camera : MonoBehaviour
{
    public static Check_Single_Camera instance;

    void Awake()
    {
        CheckInstance();
    }


    void CheckInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
