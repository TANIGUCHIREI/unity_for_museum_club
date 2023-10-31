using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows;
using Unity.VisualScripting;

public class Gacha_Insert_Paper : MonoBehaviour
{
    // Start is called before the first frame update
    public bool Start_Write_Text = false;
    bool alredy_working = false;
    GameObject User_Input_Text;
    GameObject Gacha_Event;
    public AudioClip Inserting_Machine_Sound;
    
    public string Text; //Textはchange_walls.csでmenu２から３に遷移するときにユーザーインプットに変わる
    void Start()
    {
        User_Input_Text = GameObject.Find("User_Input_Text");
        Gacha_Event = GameObject.Find("Gacha_Event");

        GameObject Init_Camera = GameObject.Find("Init_Camera"); //見つからなくても進みます

        try
        {
            if (Init_Camera.GetComponent<ClientManager>()._isStandAloneModeOne)
            {
                //もしスタンドアロンモードならふつーの動作を行う（こちら側は変える必要がないから）
                User_Input_Text.GetComponent<TMP_Text>().text = ""; //はじめは何も書かないようにする

            }
            else
            {
                User_Input_Text.GetComponent<TMP_Text>().text = ""; //はじめは何も書かないようにする
                //スタンドアロンモードでなくてもこの処理は必要！
            }
            //スタンドアロンモードでないならtryでおわり、catchは動作せずUpdateに続行
        }
        catch (System.NullReferenceException)
        {
            //NullReferenceの場合、つまりGachaの開発段階でこのシーンから動かしている場合
            Debug.Log("NullReferenceなのでGacha画面からの開始だと思われマス。そのため初めに記述されていた文章を表示します");
            Text = User_Input_Text.GetComponent<TMP_Text>().text;
            User_Input_Text.GetComponent<TMP_Text>().text = ""; //初めから書かれている文字
        }

        
        //Text = User_Input_Text.GetComponent<TMP_Text>().text;


    }

    private void FixedUpdate()
    {
        
        if (Start_Write_Text && alredy_working == false)
        {
            StartCoroutine(Write_Text());
            Start_Write_Text = false;
            alredy_working =true;
        }
        
    
    }
    // Update is called once per frame
    IEnumerator Write_Text()
    {
        //Start（）内に書いたらClientManagerと干渉しておかしくなってしまったので、ClientManagerに書き換えてもらってからこちらのほうで変更することにする
        
        

        GetComponent<Animator>().speed = 0; //まずはアニメーションの動作を停止
        gameObject.GetComponent<AudioSource>().Play();
        string[] characters = Text.Select(c => c.ToString()).ToArray();
        foreach (string word in characters)
        {
            Debug.Log(word);
            User_Input_Text.GetComponent<TMP_Text>().text += word; //文字を一文字ずつ追加していく
            yield return new WaitForSeconds(0.1f);
        }

        gameObject.GetComponent<AudioSource>().Stop(); //タイピング音再生停止
        yield return new WaitForSeconds(1f); //文字入力が終了して、インサートされるまでの時間
        GetComponent<Animator>().speed = 1; //アニメーション再開
        yield return new WaitForSeconds(2f); //音がなるまでの時間
        gameObject.GetComponent<AudioSource>().clip = Inserting_Machine_Sound;
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(4f); //インサート始まってから待つ時間


        Gacha_Event.GetComponent<Gacha_Controller>()._isPaperInsertFinish = true;
        yield return new WaitForSeconds(2.5f);
        gameObject.GetComponent<AudioSource>().Stop();
    }
}
