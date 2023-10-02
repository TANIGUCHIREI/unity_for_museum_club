using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Do_change3to2 : MonoBehaviour
{
    public GameObject change_walls;

    //public cahngewall_3to2;
    // Start is called before the first frame update
    void Start()
    {
        change_walls = GameObject.Find("change_walls");
    }

    // Update is called once per frame
    public void OnBackButtonOn()
    {
        change_walls.GetComponent<cahngewall_3to2>().OnbackButtonOn();
    }
}
