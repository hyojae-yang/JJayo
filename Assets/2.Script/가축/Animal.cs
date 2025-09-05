using UnityEngine;
using System.Collections.Generic;
using static GameManager;

public class Animal : MonoBehaviour
{
    // ★★★ 추가된 코드: 늑대 공격 이벤트를 위한 이벤트 선언 ★★★
    public static event System.Action<Transform> OnCowAttackedByWolf;

    private float health;
    public AnimalData animalData;
    private Production production;
    private AnimalUI animalUI;

    // ★★★ 추가된 코드: 우유 과잉 생산으로 인한 사망 로직 변수 ★★★
    private bool isFullAndDying = false;
    private float fullTimer = 0f;
    [SerializeField] private float overproductionDeathTime = 5f; // 에디터에서 설정 가능

    // ★★★ 추가된 코드: 빈 젖소 클릭으로 인한 사망 로직 변수 ★★★
    private int emptyClickCount = 0;
    [SerializeField] private int maxEmptyClicks = 3; // 에디터에서 설정 가능

    public void Initialize(AnimalData data)
    {
        this.animalData = data;
        health = this.animalData.maxHealth;
        production = GetComponent<Production>();
        animalUI = GetComponent<AnimalUI>();

        if (production != null && this.animalData != null)
        {
            production.productionTime = this.animalData.productionInterval;
            production.productionMax = this.animalData.maxProductionCount;
        }
    }

    void Update()
    {
        if (production != null && animalUI != null)
        {
            animalUI.UpdateProductionGauge(production.currentProductionCount, production.productionMax);
        }

        // ★★★ 추가된 코드: 우유 과잉 생산 로직 ★★★
        if (production != null && production.currentProductionCount >= production.productionMax)
        {
            if (!isFullAndDying)
            {
                isFullAndDying = true;
                fullTimer = 0f; // 타이머 시작
                NotificationManager.Instance.ShowNotification($"{animalData.animalName}의 우유가 가득 찼습니다. 관리가 필요합니다!");
            }
            fullTimer += Time.deltaTime;
            if (fullTimer >= overproductionDeathTime)
            {
                Die(); // 과잉 생산으로 인한 사망
            }
        }
        else
        {
            // 우유를 짜내어 보관량이 줄어들면 사망 카운터 리셋
            isFullAndDying = false;
            fullTimer = 0f;
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.IsMenuOn) return;

        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Milker)
        {
            // ★★★ 수정된 코드: 우유량이 1 이상일 때의 로직 ★★★
            if (production.currentProductionCount > 0)
            {
                if (production != null)
                {
                    int milkToCollect = Mathf.Min(production.currentProductionCount, PlayerInventory.Instance.MilkingYield);
                    int collectedCount = PlayerInventory.Instance.AddMilk(milkToCollect, production.currentFreshness);
                    production.currentProductionCount -= collectedCount;

                    if (collectedCount > 0)
                    {
                        NotificationManager.Instance.ShowNotification($"{animalData.animalName}의 우유를 수거했습니다.");
                        // 우유를 수거했으므로 빈 클릭 카운터 리셋
                        emptyClickCount = 0;
                    }
                }
            }
            // ★★★ 추가된 코드: 우유량이 0일 때의 로직 ★★★
            else
            {
                emptyClickCount++;
                NotificationManager.Instance.ShowNotification("우유가 부족합니다.");
                if (emptyClickCount >= maxEmptyClicks)
                {
                    Die(); // 빈 클릭 횟수 초과로 인한 사망
                }
            }
        }
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        if (attacker != null && attacker.CompareTag("Wolf"))
        {
            OnCowAttackedByWolf?.Invoke(attacker.transform);
            health -= amount;

            if (health <= 0)
            {
                Die(attacker);
            }
        }
    }

    private void Die(GameObject lastHitter = null)
    {
        // ★★★ 추가된 코드: 플레이어가 죽였을 때 통계 증가 ★★★
        if (lastHitter == null)
        {
            GameManager.Instance.CurrentGameData.totalCowsKilledByPlayer++;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SFXType.Cow_Die);
        }

        NotificationManager.Instance.ShowNotification($"{animalData.animalName}이(가) 죽었습니다.");

        if (AnimalManager.Instance != null)
        {
            AnimalManager.Instance.RemoveAnimal(this);
        }

        if (lastHitter != null)
        {
            Wolf wolfComponent = lastHitter.GetComponent<Wolf>();
            if (wolfComponent != null)
            {
                wolfComponent.OnKillTarget();
                GameManager.Instance.CurrentGameData.totalCowsEaten++;
            }
        }

        if (transform.parent != null && transform.parent.GetComponent<ObjectPool>() != null)
        {
            transform.parent.GetComponent<ObjectPool>().ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}