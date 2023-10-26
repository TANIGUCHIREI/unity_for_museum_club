using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using Shapes2D;
using UnityEngine.UI;

public class VoiceRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private int microphoneDeviceIndex = -1;
    private string currentDevice;
    private bool isRecording = false;
    private int lastSample;

    public bool doing_convert_to_text_now = false;
    public bool _is_convert_to_text_finish = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isRecording)
            {
                StopRecording();
            }
            else
            {
                StartRecording();
            }
        }

        if (Input.GetKeyDown(KeyCode.S) && !isRecording)
        {
            SaveAsWav();
            byte[] wavData = GetWavData();
            // Do something with wavData if needed
        }
    }

    public void StartRecording()
    {

        isRecording = true;
        recordedClip = Microphone.Start(currentDevice, true, 300, 44100);
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        int position = Microphone.GetPosition(currentDevice);
        Microphone.End(currentDevice);
        isRecording = false;

        if (position < recordedClip.samples)
        {
            AudioClip newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
            float[] data = new float[position * recordedClip.channels];
            recordedClip.GetData(data, 0);
            newClip.SetData(data, 0);
            recordedClip = newClip;
        }

        doing_convert_to_text_now = true; //���R�[�h���~�߂���͑��M��ԂɂȂ�͂��ŁA���̂Ƃ��̓{�^���𓮂����Ȃ��悤�ɂ���
    }

    public void SaveAsWav()
    {
        if (!recordedClip) return;

        string path = Path.Combine(Application.persistentDataPath, "recorded.wav");
        byte[] wavData = WavUtility.FromAudioClip(recordedClip);
        File.WriteAllBytes(path, wavData);
        Debug.Log("Saved to " + path);
    }

    public byte[] GetWavData()
    {
        if (!recordedClip) return null;

        return WavUtility.FromAudioClip(recordedClip);
    }


    /*
     �{�^����EventTrigger�R���|�[�l���g��ǉ����APointer Down��Pointer Up�C�x���g��ǉ����܂��B
    ���ꂼ��̃C�x���g��AudioRecorder��OnPointerDown�����OnPointerUp�֐����֘A�t���܂��B
    �ȏ�̎菇�ŁA�{�^����������Ă���Ԙ^���𑱂��A�{�^������肪���ꂽ��^�����~���铮�삪�����ł��܂��B
     */
    public void OnRecordButtonDown()
    {
        if (!doing_convert_to_text_now)
        {
            //Debug.Log("OnRecordButtonDown()"); ��Unity�ł̓V�[����ɓ������O�̂��̂���������A���ɃG���[�͂ł��Ɂi�����ォ��H�j�ق��̃I�u�W�F�N�g���珟��ɏ��������E�E�E�IBackButton�����������������ŉ������Ă�false�ɂȂ�񂩂����E�E�E
            GameObject Back_Button = GameObject.Find("Recording_Back_Button");
            //Back_Button.GetComponent<Animator>().speed = 0f;
            Back_Button.GetComponent<Button>().interactable = false; //�ϊ����͖߂�Ȃ��悤�Ɂi�G���[�����I�ɂ���܂悭�Ȃ��C�����邪�E�E�E�j
              StartRecording();
            //gameObject.GetComponent<Button>().interactable = false; //�����Ă��e���ł��Ȃ��悤�Ɂi�����Down��Up�Ȃ̂ł���͋@�\�ɒ��ڊ֌W���Ă��Ȃ��j
        }
        
    }

    public void OnRecordButtonUp()
    {
        if (!doing_convert_to_text_now)
        {
            StopRecording();
            StartCoroutine(Converting_voice_to_text_Animation());
            gameObject.GetComponentInChildren<TMP_Text>().text = "�����ɕϊ����Ă��܂��E�E�E"; //������������߂�����y����Ȃ���
        }
        
        
    }

    

    IEnumerator Converting_voice_to_text_Animation()
    {
        float speed = 30f;
        bool _isGetting_Larger = true;
        float Blur_Max_Size = 80;
        float Blur_Min_Size = 10;


        while (!_is_convert_to_text_finish)
        {
            if(_isGetting_Larger)
            {
                gameObject.GetComponent<Shape>().settings.blur += speed*Time.deltaTime;
            }
            else
            {
                gameObject.GetComponent<Shape>().settings.blur -= speed * Time.deltaTime;
            }

            //�ȉ��̏����ő傫�������菬���������肵�Ă���
            if(gameObject.GetComponent<Shape>().settings.blur > Blur_Max_Size )
            {
                _isGetting_Larger = false;
            }else if(gameObject.GetComponent<Shape>().settings.blur < Blur_Min_Size)
            {
                _isGetting_Larger = true;
            }


            yield return null;
        }
    }

}

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        // Write header
        writer.Write(new char[] { 'R', 'I', 'F', 'F' });
        writer.Write((int)(36 + clip.samples * clip.channels * 2)); // File size - 8
        writer.Write(new char[] { 'W', 'A', 'V', 'E' });
        writer.Write(new char[] { 'f', 'm', 't', ' ' });
        writer.Write(16); // Sub-chunk size (16 for PCM)
        writer.Write((short)1); // Audio format (1 for PCM)
        writer.Write((short)clip.channels);
        writer.Write(clip.frequency);
        writer.Write(clip.frequency * clip.channels * 2); // Byte rate
        writer.Write((short)(clip.channels * 2)); // Block align
        writer.Write((short)16); // Bits per sample

        // Write data
        writer.Write(new char[] { 'd', 'a', 't', 'a' });
        writer.Write(clip.samples * clip.channels * 2); // Data chunk size
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        foreach (float sample in samples)
        {
            writer.Write((short)(sample * 32768));
        }

        byte[] wavData = stream.ToArray();

        writer.Close();
        stream.Close();

        return wavData;
    }
}