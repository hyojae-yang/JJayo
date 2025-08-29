using UnityEngine;
using System.Collections.Generic;

public class AnimalHandler : MonoBehaviour
{
    public ObjectPool cowObjectPool;
    public List<Transform> cowSpawnPoints;

    private ChickenCoop _chickenCoop;

    private void Awake()
    {
        // AnimalManager�ʹ� ���� �������� �ʽ��ϴ�.
        // AnimalManager�� AnimalHandler�� ȣ���� ���� ���˴ϴ�.
    }

    public void RegisterChickenCoop(ChickenCoop coop)
    {
        _chickenCoop = coop;
        Debug.Log("���� �ν��Ͻ��� AnimalHandler�� ���������� ��ϵǾ����ϴ�.");
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
                NotificationManager.Instance.ShowNotification("���Ҹ� �����߽��ϴ�!");

                // �ڡڡ� ������ �κ�: Cow ��ũ��Ʈ�� ���� ������ �� AnimalManager�� �����մϴ�. �ڡڡ�
                Cow newCowComponent = newCowObj.GetComponent<Cow>();

                if (newCowComponent != null)
                {
                    AnimalManager.Instance.AddAnimal(newCowComponent);
                }
            }
            else
            {
                NotificationManager.Instance.ShowNotification("���Ҹ� ���� �ڸ��� �����ϴ�.");
            }
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            if (_chickenCoop != null)
            {
                _chickenCoop.AddChicken();
                NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�.");
            }
            else
            {
                NotificationManager.Instance.ShowNotification("������ ã�� �� �����ϴ�.");
            }
        }
    }

    public void Sell(Animal animalToSell, int price)
    {
        GameData gameData = GameManager.Instance.gameData;
        gameData.money += price;
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "��(��) " + price + "���� �Ǹ��߽��ϴ�!");

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