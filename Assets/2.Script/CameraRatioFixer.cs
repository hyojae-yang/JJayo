using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRatioFixer : MonoBehaviour
{
    // ������ ������ �Ǵ� ���� �þ� ũ���Դϴ�.
    // �� ���� �����Ͽ� ȭ�鿡 ���̴� ���� ������ ������ �� �ֽ��ϴ�.
    public float targetHorizontalSize = 15f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    // ȭ�� ������ ����� ������ ȣ���ϸ� �����ϴ�.
    void AdjustCamera()
    {
        // ȭ���� ��Ⱦ�� (����/����)�� ����մϴ�.
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // ī�޶��� Orthographic Size�� �����Ͽ� ���� �þ߸� �����մϴ�.
        // ����: OrthographicSize = (Ÿ�� ���� ũ�� / 2) / ���� ��Ⱦ��
        cam.orthographicSize = targetHorizontalSize / (2f * screenAspectRatio);
    }
}