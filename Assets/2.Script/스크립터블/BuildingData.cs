using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Tycoon Game/Building Data")]
public class BuildingData : ScriptableObject
{
    // �ǹ��� �ĺ��� ���� ID (�ڵ忡�� �ǹ��� ã�� �� ���˴ϴ�)
    public string buildingId;

    // ������ ǥ�õ� �ǹ��� �̸�
    public string buildingName;

    // �ǹ��� ���� ����
    public int buildingPrice;

    // ������ ǥ�õ� �ǹ��� ������
    public Sprite buildingIcon;

    // ������ ���ӿ� ��ġ�� �ǹ��� ������
    public GameObject buildingPrefab;
}