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

    public ObjectPool cowObjectPool;
    public List<Transform> cowSpawnPoints;

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

    public void Purchase(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            GameObject newCowObj = cowObjectPool.GetFromPool();

            if (newCowObj != null)
            {
                newCowObj.transform.position = cowSpawnPoints[0].position;
                cowSpawnPoints.RemoveAt(0);
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");

                Cow newCowComponent = newCowObj.GetComponent<Cow>();

                if (newCowComponent != null)
                {
                    if (AnimalManager.Instance != null) AnimalManager.Instance.AddAnimal(newCowComponent);
                }
            }
            else
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
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