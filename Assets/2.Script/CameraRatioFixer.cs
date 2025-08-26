using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRatioFixer : MonoBehaviour
{
    // 게임의 기준이 되는 가로 시야 크기입니다.
    // 이 값을 조정하여 화면에 보이는 가로 범위를 설정할 수 있습니다.
    public float targetHorizontalSize = 15f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    // 화면 비율이 변경될 때마다 호출하면 좋습니다.
    void AdjustCamera()
    {
        // 화면의 종횡비 (가로/세로)를 계산합니다.
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // 카메라의 Orthographic Size를 조정하여 가로 시야를 고정합니다.
        // 공식: OrthographicSize = (타겟 가로 크기 / 2) / 현재 종횡비
        cam.orthographicSize = targetHorizontalSize / (2f * screenAspectRatio);
    }
}