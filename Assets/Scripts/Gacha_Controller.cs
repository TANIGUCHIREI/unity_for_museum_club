using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha_Controller : MonoBehaviour
{
    public GameObject camera0;
    public GameObject camera1;
    public GameObject camera2;
    public GameObject camera3;
    public GameObject camera4;

    public AudioClip shutter_sound;
    public AudioClip SpotLight_sound;

    AudioSource audioSource;

    public GameObject SpotLight;

    // Start is called before the first frame update
    void Start()
    {
        camera0 = GameObject.Find("Init_Camera"); //‚Í‚¶‚ß‚©‚çˆø‚«Œp‚ª‚ê‚½‚â‚Â
        SpotLight = GameObject.Find("Directional Light");
        SpotLight.GetComponent<Light>().intensity = 0f;
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(false);
        camera4.SetActive(false);


        audioSource = GetComponent<AudioSource>();

        StartCoroutine(Camera_Motion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Camera_Motion()
    {
        yield return new WaitForSeconds(4.0f);
        audioSource.PlayOneShot(SpotLight_sound);
        SpotLight.GetComponent<Light>().intensity = 2f;
        yield return new WaitForSeconds(2.0f);
        audioSource.PlayOneShot(shutter_sound);
        camera1.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(shutter_sound);
        camera2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(shutter_sound);
        camera3.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(shutter_sound);
        camera4.SetActive(true);
        yield return new WaitForSeconds(0.5f);
    }
}
