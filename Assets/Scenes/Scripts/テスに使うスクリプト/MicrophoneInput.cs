using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    private AudioSource audioSource;
    public float sensitivity = 100.0f;
    public GameObject targetObject;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // マイクを開始
        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
    }

    void Update()
    {
        float volume = GetAverageVolume() * sensitivity;
        targetObject.transform.localScale = new Vector3(volume, volume, volume);
    }

    float GetAverageVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audioSource.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256.0f;
    }
}