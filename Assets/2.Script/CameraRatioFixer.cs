using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRatioFixer : MonoBehaviour
{
    // ������ ������ �Ǵ� ���� �þ� ũ���Դϴ�.
    // �� ���� �����Ͽ� ȭ�鿡 ���̴� ���� ������ ������ �� �ֽ��ϴ�.
    public float targetVerticalSize = 10f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        // ȭ���� ��Ⱦ�� (����/����)�� ����մϴ�.
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // ������ ������ �Ǵ� ��Ⱦ���Դϴ�.
        // ���� ��� 1920x1080�� �������� �Ѵٸ� 16f / 9f = 1.777f �Դϴ�.
        float targetAspectRatio = 16f / 9f;

        // ī�޶��� Orthographic Size�� �����Ͽ� ���� �þ߸� �����մϴ�.
        // ȭ�� ������ ���غ��� ������, ���ο� ���� ���� ũ�⸦ �÷� ���� ������ ����ϴ�.
        cam.orthographicSize = targetVerticalSize;
        if (screenAspectRatio > targetAspectRatio)
        {
            float newSize = targetVerticalSize * (targetAspectRatio / screenAspectRatio);
            cam.orthographicSize = newSize;
        }
    }
}