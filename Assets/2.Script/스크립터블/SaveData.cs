[System.Serializable]
public class SaveData
{
    // GameManager���� ������ ������
    public int currentMoney;
    public int reputation;
    public int gameDay;
    public int gameMonth;
    public int gameYear;

    // PlayerInventory���� ������ ������ (����)
    public System.Collections.Generic.List<MilkData> milkInventory;

    // Warehouse���� ������ ������ (�ް�)
    public int eggCount;
}

[System.Serializable]
public class MilkData
{
    public float freshness;
    // ������ �߰��� �ٸ� �����Ͱ� �ִٸ� ���⿡ public ������ �߰��ϼ���.
}