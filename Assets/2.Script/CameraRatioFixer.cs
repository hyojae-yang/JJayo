using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRatioFixer : MonoBehaviour
{
    // 게임의 기준이 되는 세로 시야 크기입니다.
    // 이 값은 Orthographic Size 값과 동일합니다.
    public float targetVerticalSize = 15f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        // Orthographic Size를 targetVerticalSize의 절반으로 설정하여 세로 시야를 고정합니다.
        // 유니티의 Orthographic Size는 화면의 절반 높이를 의미합니다.
        cam.orthographicSize = targetVerticalSize / 2f;
    }
}