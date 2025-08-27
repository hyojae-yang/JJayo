using UnityEngine;
using System.Collections.Generic;

public class AnimalHandler : MonoBehaviour
{
    public ObjectPool cowObjectPool;
    public ChickenCoop chickenCoop;
    public List<Transform> cowSpawnPoints;

    private void Awake()
    {
        // ObjectPool과 ChickenCoop은 씬에서 찾거나 수동으로 연결해야 합니다.
        // 현재는 수동 연결을 가정합니다.
    }

    public bool CanBuy(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            return cowSpawnPoints.Count > 0;
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            return chickenCoop != null;
        }
        return false;
    }

    // 닭을 판매할 수 있는지 여부를 AnimalHandler가 직접 확인합니다.
    public bool CanSellChicken()
    {
        return chickenCoop != null && chickenCoop.numberOfChickens > 0;
    }

    public void Purchase(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            GameObject newCow = cowObjectPool.GetFromPool();
            if (newCow != null)
            {
                newCow.transform.position = cowSpawnPoints[0].position;
                cowSpawnPoints.RemoveAt(0);
                NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");
            }
            else
            {
                NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
            }
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            chickenCoop.AddChicken();
            NotificationManager.Instance.ShowNotification("닭을 구매했습니다.");
        }
    }

    public void Sell(Animal animalToSell, int price)
    {
        GameData gameData = GameManager.Instance.gameData;
        gameData.money += price;
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + price + "원에 판매했습니다!");

        if (cowObjectPool != null)
        {
            cowObjectPool.ReturnToPool(animalToSell.gameObject);
        }
        else
        {
            Destroy(animalToSell.gameObject);
        }
    }

    // 닭을 실제로 제거하는 메서드를 추가합니다.
    public void RemoveChicken()
    {
        if (chickenCoop != null)
        {
            chickenCoop.RemoveChicken();
        }
    }
}