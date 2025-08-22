using UnityEngine;

// ����Ƽ �޴��� �� ��ũ���ͺ� ������Ʈ�� ������ �� �ִ� �޴��� �߰��մϴ�.
[CreateAssetMenu(fileName = "ChickenCoopData", menuName = "Jjayo/ChickenCoop Data")]
public class ChickenCoopData : ScriptableObject
{
    [Header("���� ����")]
    public string coopName;

    [Tooltip("�� �� ������ �� 1���� ���� �� �ɸ��� �ð�(��).")]
    public float eggProductionInterval = 10f; // �⺻�� 10��

    [Tooltip("���� �ǹ� ���� ���.")]
    public int purchaseCost = 5000;
}