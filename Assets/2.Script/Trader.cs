using UnityEngine;

// ������ �䱸���� �����͸� ��� Ŭ����
// �� ��ũ��Ʈ�� GameManager�� �����Ͽ� ����մϴ�.
public class Trader : MonoBehaviour
{
    // ������ �䱸�ϴ� ������ ����
    [Tooltip("������ �䱸�ϴ� ������ ����")]
    public int requiredMilkAmount;

    // ������ �䱸�ϴ� ������ �ּ� �ż��� (100�� ����)
    [Tooltip("������ �䱸�ϴ� ������ �ּ� �ż���")]
    public int requiredFreshness;

    // ������ �����ϴ� ���� �Ǹ� �ݾ�
    [Tooltip("������ �����ϴ� ���� �Ǹ� �ݾ�")]
    public int offeredPrice;

    // ������ �����ϴ� ���� (���� �߰�)
    // public int playerReputation;

    // �ʿ��� ���, �� ���� ������ �߰��� �� �ֽ��ϴ�.
    // ��: ���� ���� Ȯ�� ��
}