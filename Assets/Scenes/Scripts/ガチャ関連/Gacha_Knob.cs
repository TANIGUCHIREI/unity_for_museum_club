using UnityEngine;

public class GachaKnob : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _knobScreenPos;
    private Vector2 _initVector;
    private Vector2 _moveVector;

    private float _totalRotation = 0.0f;
    private const float ROTATE_THRESHOLD = 360.0f;
    public bool _isRotateFinish = false;
    public bool Gacha_Rotate_On = false;
    public float N = 12;
    private float divided_angle;
    private float next_sound_angle  ;
    AudioSource audioSource;
    public AudioClip Gacha_sound;
    public AudioClip Gacha_Emit_sound;

    public GameObject Gacha_Event;
    void Start()
    {
        _camera = GameObject.Find("init_camera_duplicate").GetComponent<Camera>();
        divided_angle =  360 / N;
        next_sound_angle = divided_angle;
        audioSource = GetComponent<AudioSource>();

        Gacha_Event = GameObject.Find("Gacha_Event");
    }

    void Update()
    {
        if(_isRotateFinish == false && Gacha_Rotate_On)
        {//まだ回り切っていない時、かつガチャが動いてもいいよってときに動く
            _knobScreenPos = _camera.WorldToScreenPoint(transform.position);  // ここで毎フレーム更新

            // タッチ入力の取得
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    _initVector = touch.position - new Vector2(_knobScreenPos.x, _knobScreenPos.y);
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    _moveVector = touch.position - new Vector2(_knobScreenPos.x, _knobScreenPos.y);
                    RotateKnob();
                }
            }
            // マウス入力の取得
            else if (Input.GetMouseButtonDown(0))
            {
                _initVector = Input.mousePosition - new Vector3(_knobScreenPos.x, _knobScreenPos.y);
            }
            else if (Input.GetMouseButton(0))
            {
                _moveVector = Input.mousePosition - new Vector3(_knobScreenPos.x, _knobScreenPos.y);
                RotateKnob();
            }
        }
        
    }

    void RotateKnob()
    {
        float angleDifference = Vector2.SignedAngle(_initVector, _moveVector);
        if (angleDifference < 0) // 時計回りのみ
        {
            transform.Rotate(Vector3.forward, -angleDifference);
            _totalRotation += -angleDifference;

            if(_totalRotation > next_sound_angle)
            {
                audioSource.PlayOneShot(Gacha_sound);
                next_sound_angle += divided_angle;
            }

            if (_totalRotation >= ROTATE_THRESHOLD)
            {
                _isRotateFinish = true;
                Gacha_Event.GetComponent<Gacha_Controller>()._isGacha_knob_Rotate_finish = true;
                //audioSource.PlayOneShot(Gacha_Emit_sound);
            }
            _initVector = _moveVector;  // 現在のベクトルを初期ベクトルとして更新
        }

        Debug.Log(_totalRotation);
    }
}