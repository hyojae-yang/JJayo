using UnityEngine;

public class WolfSummoner : MonoBehaviour
{
    // �ν����Ϳ� WolfManager�� ������ ����
    public WolfManager wolfManager;

    void Update()
    {
        // 'W' Ű�� ������ �� ���븦 ��ȯ�մϴ�.
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (wolfManager != null)
            {
                wolfManager.SpawnWolf();
                Debug.Log("������ ��ȯ �ڵ�: ���븦 ��� ��ȯ�մϴ�!");
            }
            else
            {
                Debug.LogError("Wolf Manager�� �Ҵ���� �ʾҽ��ϴ�. �ν����Ϳ��� �������ּ���.");
            }
        }
    }
}