[System.Serializable]
public class SaveData
{
    // GameManager에서 저장할 데이터
    public int currentMoney;
    public int reputation;
    public int gameDay;
    public int gameMonth;
    public int gameYear;

    // PlayerInventory에서 저장할 데이터 (우유)
    public System.Collections.Generic.List<MilkData> milkInventory;

    // Warehouse에서 저장할 데이터 (달걀)
    public int eggCount;
}

[System.Serializable]
public class MilkData
{
    public float freshness;
    // 우유에 추가할 다른 데이터가 있다면 여기에 public 변수로 추가하세요.
}