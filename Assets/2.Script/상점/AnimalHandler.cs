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

    [SerializeField] public ObjectPool cowObjectPool;
    // 수정된 부분: AnimalHandler는 이제 스폰포인트를 관리하지 않습니다.

    private ChickenCoop _chickenCoop;

    public void RegisterChickenCoop(ChickenCoop coop)
    {
        _chickenCoop = coop;
    }

    public bool CanBuy(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            // 수정된 부분: AnimalManager에게 빈 공간이 있는지 직접 확인합니다.
            return AnimalManager.Instance != null && AnimalManager.Instance.GetAvailableCowPosition() != Vector2.zero;
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

    public void Purchase(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            // 수정된 부분: AnimalManager에게 빈 공간을 요청합니다.
            Vector2 spawnPosition = AnimalManager.Instance.GetAvailableCowPosition();

            if (spawnPosition == Vector2.zero)
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
                return;
            }

            GameObject newCowObj = cowObjectPool.GetFromPool();

            if (newCowObj != null)
            {
                newCowObj.transform.position = spawnPosition;
                AnimalManager.Instance.occupiedCowPositions.Add(spawnPosition);

                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");

                Animal newCowComponent = newCowObj.GetComponent<Animal>();
                Production productionComponent = newCowObj.GetComponent<Production>();

                if (newCowComponent != null)
                {
                    newCowComponent.Initialize(animalData);

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