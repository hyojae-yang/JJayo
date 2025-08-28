using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    // ������ ��ȭ �� ��
    public int money;
    public int reputation;

    // �ð� �� ��¥
    public int day;
    public int month;
    public int year;

    // ���귮
    public int dailyMilkProduced;
    public int dailyEggsProduced;
    public int milkCount;
    public float milkAverageFreshness;
    public int eggCount;

    // ��� �� ���׷��̵� ����
    public int pastureLevel;
    public bool hasGun;
    public int gunLevel;
    public int basketLevel;
    public int milkerLevel;
    public int bulletCount;

    public EquipmentType currentEquipment;

    // �ڡڡ� �ǹ� ���� ���� (List�� ����)
    public List<string> ownedBuildingIds;

    // �ڡڡ� ��� ���� ���� (List �߰�) �ڡڡ�
    public List<string> ownedEquipmentIds;

    // NPC(����) ���� ������
    public int traderRequiredMilkAmount;
    public int traderRequiredFreshness;
    public int traderOfferedPrice;
    public float traderCurrentEggPrice;

    public GameData()
    {
        // ��� ������ �ʱⰪ�� ��Ȯ�� �����մϴ�.
        this.money = 50000;
        this.reputation = 0;

        this.pastureLevel = 0;

        this.day = 1;
        this.month = 1;
        this.year = 1;

        this.dailyMilkProduced = 0;
        this.dailyEggsProduced = 0;
        this.milkCount = 0;
        this.milkAverageFreshness = 0f;
        this.eggCount = 0;

        this.hasGun = false;
        this.gunLevel = 0;
        this.basketLevel = 1;
        this.milkerLevel = 1;
        this.bulletCount = 0;

        // �ڡڡ� �ǹ� ���� ����Ʈ �ʱ�ȭ
        this.ownedBuildingIds = new List<string>();

        // �ڡڡ� ��� ���� ����Ʈ �ʱ�ȭ �ڡڡ�
        this.ownedEquipmentIds = new List<string>();

        this.traderRequiredMilkAmount = 0;
        this.traderRequiredFreshness = 0;
        this.traderOfferedPrice = 0;
        this.traderCurrentEggPrice = 0;
    }

    /// <summary>
    /// Ư�� �ǹ��� �����ϰ� �ִ��� Ȯ���մϴ�.
    /// </summary>
    public bool IsBuildingOwned(string buildingId)
    {
        return ownedBuildingIds.Contains(buildingId);
    }
}