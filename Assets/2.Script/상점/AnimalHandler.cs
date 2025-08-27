using UnityEngine;
using System.Collections.Generic;

public class AnimalHandler : MonoBehaviour
{
    public ObjectPool cowObjectPool;
    public ChickenCoop chickenCoop;
    public List<Transform> cowSpawnPoints;

    private void Awake()
    {
        // ObjectPool�� ChickenCoop�� ������ ã�ų� �������� �����ؾ� �մϴ�.
        // ����� ���� ������ �����մϴ�.
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

    // ���� �Ǹ��� �� �ִ��� ���θ� AnimalHandler�� ���� Ȯ���մϴ�.
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
                NotificationManager.Instance.ShowNotification("���Ҹ� �����߽��ϴ�!");
            }
            else
            {
                NotificationManager.Instance.ShowNotification("���Ҹ� ���� �ڸ��� �����ϴ�.");
            }
        }
        else if (animalData.animalType == AnimalType.Chicken)
        {
            chickenCoop.AddChicken();
            NotificationManager.Instance.ShowNotification("���� �����߽��ϴ�.");
        }
    }

    public void Sell(Animal animalToSell, int price)
    {
        GameData gameData = GameManager.Instance.gameData;
        gameData.money += price;
        NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "��(��) " + price + "���� �Ǹ��߽��ϴ�!");

        if (cowObjectPool != null)
        {
            cowObjectPool.ReturnToPool(animalToSell.gameObject);
        }
        else
        {
            Destroy(animalToSell.gameObject);
        }
    }

    // ���� ������ �����ϴ� �޼��带 �߰��մϴ�.
    public void RemoveChicken()
    {
        if (chickenCoop != null)
        {
            chickenCoop.RemoveChicken();
        }
    }
}