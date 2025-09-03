using UnityEngine;
using System.Collections.Generic;

public class AnimalHandler : MonoBehaviour
{
    private static AnimalHandler m_instance;
    public static AnimalHandler Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<AnimalHandler>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // **수정된 부분: 인스펙터로 연결된 오브젝트 풀과 스폰 포인트를 추가합니다.**
    [SerializeField] public ObjectPool cowObjectPool;
    [SerializeField] public List<Transform> cowSpawnPoints;

    private ChickenCoop _chickenCoop;

    public void RegisterChickenCoop(ChickenCoop coop)
    {
        _chickenCoop = coop;
    }

    public bool CanBuy(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            return cowSpawnPoints.Count > 0;
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            return _chickenCoop != null;
        }
        return false;
    }

    public bool CanSellChicken()
    {
        return _chickenCoop != null && _chickenCoop.numberOfChickens > 0;
    }

    // 이 메서드는 이제 ShopService에서 직접 호출됩니다.
    public void Purchase(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            if (cowSpawnPoints.Count == 0)
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
                return;
            }

            GameObject newCowObj = cowObjectPool.GetFromPool();

            if (newCowObj != null)
            {
                newCowObj.transform.position = cowSpawnPoints[0].position;
                cowSpawnPoints.RemoveAt(0);

                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");

                Animal newCowComponent = newCowObj.GetComponent<Animal>();
                Production productionComponent = newCowObj.GetComponent<Production>();

                if (newCowComponent != null)
                {
                    // Animal 컴포넌트의 Initialize 메서드 호출
                    newCowComponent.Initialize(animalData);

                    // Production 컴포넌트의 Initialize 메서드 호출
                    if (productionComponent != null && GameManager.Instance != null && GameManager.Instance.pastureUpgradeData != null)
                    {
                        productionComponent.Initialize(GameManager.Instance.CurrentPastureLevel, GameManager.Instance.pastureUpgradeData);
                    }
                    else
                    {
                        Debug.LogError("Production 컴포넌트를 찾을 수 없거나 GameManager 데이터가 유효하지 않습니다.");
                    }

                    if (AnimalManager.Instance != null) AnimalManager.Instance.AddAnimal(newCowComponent);
                }
            }
            else
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 가져올 수 없습니다. 오브젝트 풀을 확인하세요.");
            }
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            if (_chickenCoop != null)
            {
                _chickenCoop.AddChicken();
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("닭을 구매했습니다.");
            }
            else
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("닭장을 찾을 수 없습니다.");
            }
        }
    }

    public void Sell(Animal animalToSell, int price)
    {
        GameData gameData = GameManager.Instance.CurrentGameData;
        gameData.money += price;
        if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + price + "원에 판매했습니다!");

        if (AnimalManager.Instance != null) AnimalManager.Instance.RemoveAnimal(animalToSell);

        if (cowObjectPool != null)
        {
            cowObjectPool.ReturnToPool(animalToSell.gameObject);
        }
        else
        {
            Destroy(animalToSell.gameObject);
        }
    }

    public void RemoveChicken()
    {
        if (_chickenCoop != null)
        {
            _chickenCoop.RemoveChicken();
        }
    }
}