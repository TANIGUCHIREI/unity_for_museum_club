using System.Collections;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera cameraComponent;
    public float switchSpeed = 1.0f;

    private bool isSwitching = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSwitching)
        {
            if (cameraComponent.orthographic)
                StartCoroutine(SwitchToPerspective());
            else
                StartCoroutine(SwitchToOrthographic());
        }
    }

    IEnumerator SwitchToOrthographic()
    {
        isSwitching = true;

        while (cameraComponent.fieldOfView > 10f)
        {
            cameraComponent.fieldOfView -= switchSpeed;
            yield return null;
        }

        cameraComponent.orthographic = true;
        float targetOrthographicSize = 5f; // ����͖ړI�̃T�C�Y�ɍ��킹�Ē������Ă�������

        while (cameraComponent.orthographicSize < targetOrthographicSize)
        {
            cameraComponent.orthographicSize += switchSpeed * 0.1f;
            yield return null;
        }

        isSwitching = false;
    }

    IEnumerator SwitchToPerspective()
    {
        isSwitching = true;

        float initialOrthographicSize = cameraComponent.orthographicSize;
        while (cameraComponent.orthographicSize > 0.1f)
        {
            cameraComponent.orthographicSize -= switchSpeed * 0.1f;
            yield return null;
        }

        cameraComponent.orthographic = false;
        cameraComponent.fieldOfView = 10f;

        while (cameraComponent.fieldOfView < 60f) // �ʏ��60�x��FOV�ɖ߂�
        {
            cameraComponent.fieldOfView += switchSpeed;
            yield return null;
        }

        isSwitching = false;
    }
}