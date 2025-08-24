using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRatioFixer : MonoBehaviour
{
    // 게임의 기준이 되는 세로 시야 크기입니다.
    // 이 값을 조정하여 화면에 보이는 세로 범위를 설정할 수 있습니다.
    public float targetVerticalSize = 10f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        // 화면의 종횡비 (가로/세로)를 계산합니다.
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // 게임의 기준이 되는 종횡비입니다.
        // 예를 들어 1920x1080을 기준으로 한다면 16f / 9f = 1.777f 입니다.
        float targetAspectRatio = 16f / 9f;

        // 카메라의 Orthographic Size를 조정하여 세로 시야를 고정합니다.
        // 화면 비율이 기준보다 넓으면, 가로에 맞춰 세로 크기를 늘려 검은 여백을 만듭니다.
        cam.orthographicSize = targetVerticalSize;
        if (screenAspectRatio > targetAspectRatio)
        {
            float newSize = targetVerticalSize * (targetAspectRatio / screenAspectRatio);
            cam.orthographicSize = newSize;
        }
    }
}