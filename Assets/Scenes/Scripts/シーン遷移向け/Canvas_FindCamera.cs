using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_FindCamera : MonoBehaviour
{
    // Start is called before the first frame update

    void Awake()
    {
        Camera cam = GameObject.Find("Init_Camera").GetComponent<Camera>();
        gameObject.GetComponent<Canvas>().worldCamera = cam;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
