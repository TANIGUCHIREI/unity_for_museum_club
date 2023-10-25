using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Radius_change : MonoBehaviour
{
    public float shadowRadius = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Light>().areaSize = new Vector2(shadowRadius,shadowRadius); //‚±‚ê‚ªLight‚ÌƒtƒŒƒA‚ÌL‚³
    }
}
