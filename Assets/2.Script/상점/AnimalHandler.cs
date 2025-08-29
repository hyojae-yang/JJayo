using UnityEngine;
using System.Collections.Generic;

public class AnimalHandler : MonoBehaviour
{
    public ObjectPool cowObjectPool;
    public List<Transform> cowSpawnPoints;

    private ChickenCoop _chickenCoop;

    private void Awake()
    {
        // AnimalManager와는 직접 연결하지 않습니다.
        // AnimalManager는 AnimalHandler가 호출할 때만 사용됩니다.
    }

    public void RegisterChickenCoop(ChickenCoop coop)
    {
        _chickenCoop = coop;
        Debug.Log("닭장 인스턴스가 AnimalHandler에 성공적으로 등록되었습니다.");
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
                NotificationManager.Instance.ShowNotification("젖소를 구매했습니다!");

                // ★★★ 수정된 부분: Cow 스크립트를 직접 가져온 후 AnimalManager에 전달합니다. ★★★
                Cow newCowComponent = newCowObj.GetComponent<Cow>();

                if (newCowComponent != null)
                {
                    AnimalManager.Instance.AddAnimal(newCowComponent);
                }
            }
            else
            {
                NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
            }
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            if (_chickenCoop != null)
            {
                _chickenCoop.AddChicken();
                NotificationManager.Instance.ShowNotification("닭을 구매했습니다.");
            }
            else
            {
                NotificationManager.Instance.ShowNotification("닭장을 찾을 수 없습니다.");
            }
        }
    }

    public void Sell(Animal animalToSell, int price)
    {
        GameData gameData = GameManager.Instance.gameData;
        gameData.money += price;
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + price + "원에 판매했습니다!");

        AnimalManager.Instance.RemoveAnimal(animalToSell);

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