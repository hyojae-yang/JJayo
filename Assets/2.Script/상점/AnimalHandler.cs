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

    // ★★★ 추가된 구조체: AnimalData와 ObjectPool을 묶어서 관리합니다. ★★★
    [System.Serializable]
    public struct AnimalPoolPair
    {
        public AnimalData animalData;
        public ObjectPool objectPool;
    }

    // ★★★ 수정된 변수: 여러 젖소 풀을 관리할 수 있는 리스트입니다. ★★★
    public List<AnimalPoolPair> animalPools;

    private ChickenCoop _chickenCoop;

    public void RegisterChickenCoop(ChickenCoop coop)
    {
        _chickenCoop = coop;
    }

    public bool CanBuy(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
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
        // ★★★ 수정된 부분: ChickenCoop 대신 GameData의 chickenCount를 확인합니다. ★★★
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameData == null) return false;
        return GameManager.Instance.CurrentGameData.chickenCount > 0;
    }

    public void Purchase(AnimalData animalData)
    {
        if (animalData.animalType == AnimalType.Cow)
        {
            Vector2 spawnPosition = AnimalManager.Instance.GetAvailableCowPosition();

            if (spawnPosition == Vector2.zero)
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("젖소를 놓을 자리가 없습니다.");
                return;
            }

            // ★★★ 수정된 로직: 구매하려는 젖소의 종류에 맞는 풀을 찾습니다. ★★★
            ObjectPool targetPool = null;
            foreach (var pair in animalPools)
            {
                if (pair.animalData.animalId == animalData.animalId)
                {
                    targetPool = pair.objectPool;
                    break;
                }
            }

            if (targetPool == null)
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("해당 젖소의 오브젝트 풀이 없습니다.");
                return;
            }

            GameObject newCowObj = targetPool.GetFromPool();

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

                    if (productionComponent != null)
                    {
                        productionComponent.Initialize(animalData);
                    }
                    else
                    {
                        Debug.LogError("Production 컴포넌트를 찾을 수 없습니다.");
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
            // ★★★ 수정된 부분: ChickenCoop에 직접 요청하는 대신 GameData를 수정합니다. ★★★
            if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
            {
                GameManager.Instance.CurrentGameData.chickenCount++;
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("닭을 구매했습니다.");
            }
            else
            {
                if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification("게임 데이터를 찾을 수 없습니다.");
            }
        }
    }

    public void Sell(Animal animalToSell, int price)
    {
        GameData gameData = GameManager.Instance.CurrentGameData;
        gameData.money += price;
        if (NotificationManager.Instance != null) NotificationManager.Instance.ShowNotification(animalToSell.animalData.animalName + "을(를) " + price + "원에 판매했습니다!");

        if (AnimalManager.Instance != null) AnimalManager.Instance.RemoveAnimal(animalToSell);

        // ★★★ 수정된 부분: 이제 cowObjectPool 대신 해당 젖소의 풀을 찾아 반환해야 합니다. ★★★
        ObjectPool targetPool = null;
        foreach (var pair in animalPools)
        {
            if (pair.animalData.animalId == animalToSell.animalData.animalId)
            {
                targetPool = pair.objectPool;
                break;
            }
        }

        if (targetPool != null)
        {
            targetPool.ReturnToPool(animalToSell.gameObject);
        }
        else
        {
            Destroy(animalToSell.gameObject);
        }
    }

    public void RemoveChicken()
    {
        // ★★★ 수정된 부분: ChickenCoop에 직접 요청하는 대신 GameData를 수정합니다. ★★★
        if (GameManager.Instance != null && GameManager.Instance.CurrentGameData != null)
        {
            if (GameManager.Instance.CurrentGameData.chickenCount > 0)
            {
                GameManager.Instance.CurrentGameData.chickenCount--;
            }
        }
    }
}