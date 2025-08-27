using UnityEngine;
using System.Collections.Generic;

public class BuildingHandler : MonoBehaviour
{
    // BuildingHandler�� ���� ���� ����Ʈ�� �����մϴ�.
    [SerializeField] public List<Transform> buildingSpawnPoints;

    // �� Ŭ������ ���� Ư�� �ǹ� ������ ���� �ʿ䰡 �����ϴ�.
    // private ChickenCoop chickenCoop;
    // ...

    private void Awake()
    {
        // ������ �ʿ��� ������Ʈ���� ã�ƿ� �����մϴ�.
        // �ǹ����� FindObjectOfType�� ȣ���� �ʿ䰡 �������ϴ�.
    }

    /// <summary>
    /// �ǹ� ���� ���� ���θ� Ȯ���մϴ�.
    /// �� �ڵ鷯�� ���� ��ġ�� ������ �ִ��� ���θ� Ȯ���մϴ�.
    /// </summary>
    public bool CanBuy()
    {
        return buildingSpawnPoints.Count > 0;
    }

    /// <summary>
    /// �ǹ��� �����ϰ� ��ġ�մϴ�.
    /// </summary>
    /// <param name="buildingData">�����Ϸ��� �ǹ��� ������ �����Դϴ�.</param>
    public void Purchase(BuildingData buildingData)
    {
        // buildingSpawnPoints ����Ʈ�� ��ȿ�� �˻�
        if (buildingSpawnPoints.Count == 0)
        {
            NotificationManager.Instance.ShowNotification("�� �̻� �ǹ��� ��ġ�� ������ �����ϴ�!");
            return;
        }

        // buildingData�� ��ȿ���� Ȯ��
        if (buildingData == null || buildingData.buildingPrefab == null)
        {
            Debug.LogError("�����Ϸ��� �ǹ� ������ �Ǵ� �������� ��ȿ���� �ʽ��ϴ�.");
            return;
        }

        // ���� �����ϰ� �ν��Ͻ�ȭ ������ �����մϴ�.
        Instantiate(buildingData.buildingPrefab, buildingSpawnPoints[0].position, Quaternion.identity);
        buildingSpawnPoints.RemoveAt(0);

        // �ǹ� ���� �� GameData�� ���� ������ �߰��մϴ�.
        GameData gameData = GameManager.Instance.gameData;
        if (gameData != null)
        {
            // GameData�� List�� �ǹ��� ID�� �߰��մϴ�.
            gameData.ownedBuildingIds.Add(buildingData.buildingId);
        }

        NotificationManager.Instance.ShowNotification(buildingData.buildingName + "��(��) �����߽��ϴ�. ���忡 ��ġ�Ǿ����ϴ�!");
    }
}