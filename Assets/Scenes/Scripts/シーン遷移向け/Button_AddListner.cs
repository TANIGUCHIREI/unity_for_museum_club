using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Button_AddListner : MonoBehaviour
{
    [SerializeField] 
    Button testBtn;
    //public GameObject Text;
    // Start is called before the first frame update
    void Start()
    {
        testBtn = gameObject.GetComponent<Button>(); //���̃{�^���̂���
        GameObject changewall = GameObject.Find("Change_walls_UI");
        testBtn.onClick.AddListener(changewall.GetComponent<change_wall>().change_2_3); //����ŃV�[���J�ڂ�init_menu�ɖ߂����Ƃ��Ă��{�^���̃N���b�N���̃X�N���v�g�ɓo�^�����H
   
    }

    // Update is called once per frame
    
   
}
