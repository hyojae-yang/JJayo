using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Wolf : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("UI")]
    public Slider healthBarSlider;

    [Header("Wolf Stats")]
    private float health;
    private float maxHealth;
    private float damage;

    [Header("Targeting")]
    public float moveSpeed = 3f;
    private Transform targetCow;
    private float minDistanceToTarget = 1.5f;

    public float attackRate = 1f;
    private float nextAttackTime = 0f;

    public WolfManager wolfManager;
    public bool isReturning = false;

    public void Initialize(WolfManager manager, float newHealth, float newDamage)
    {
        this.wolfManager = manager;
        this.health = newHealth;
        this.maxHealth = newHealth;
        this.damage = newDamage;
        FindNewTarget();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isReturning = false;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = health;
        }

        Debug.Log($"새로운 늑대 소환! 체력: {health}, 공격력: {damage}");
    }

    void Update()
    {
        CheckIfOffScreen();

        if (!isReturning)
        {
            if (targetCow != null && targetCow.gameObject.activeInHierarchy)
            {
                Vector3 direction = targetCow.position - transform.position;
                transform.position = Vector3.MoveTowards(transform.position, targetCow.position, moveSpeed * Time.deltaTime);

                if (spriteRenderer != null)
                {
                    if (direction.x < 0) spriteRenderer.flipX = false;
                    else spriteRenderer.flipX = true;
                }

                if (Vector3.Distance(transform.position, targetCow.position) < minDistanceToTarget && Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackRate;
                }
            }
            else
            {
                FindNewTarget();

                if (targetCow == null)
                {
                    isReturning = true;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position + (transform.position - Camera.main.transform.position) * 10f, moveSpeed * Time.deltaTime);
        }
    }

    private void FindNewTarget()
    {
        Animal closestCow = null;
        float closestDistance = Mathf.Infinity;
        Vector3 wolfPosition = transform.position;

        List<Animal> activeCows = wolfManager.GetActiveCows();

        foreach (Animal cow in activeCows)
        {
            if (cow == null)
            {
                continue;
            }

            float distance = Vector3.Distance(wolfPosition, cow.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCow = cow;
            }
        }

        if (closestCow != null)
        {
            targetCow = closestCow.transform;
        }
        else
        {
            targetCow = null;
        }
    }

    void Attack()
    {
        Animal animal = targetCow.GetComponent<Animal>();
        if (animal != null)
        {
            animal.TakeDamage(damage, this.gameObject);
        }
    }

    // 총에 맞았을 때 호출되는 메서드
    public void TakeDamage(float amount)
    {
        health -= amount;

        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        if (health <= 0)
        { // ★★★ 이 줄을 추가합니다. ★★★
            GameManager.Instance.CurrentGameData.totalWolvesKilled++;
            SoundManager.Instance.PlaySFX(SFXType.Wolf_Die);
            if (wolfManager != null)
            {
                wolfManager.ReturnWolfToPool(gameObject);
            }
        }
    }

    // 늑대가 젖소를 성공적으로 처치했을 때 호출됩니다.
    public void OnKillTarget()
    {
        isReturning = true;
        targetCow = null;
    }

    // ★★★ 추가된 부분: 늑대 클릭 시 총알 데미지 처리 ★★★
    void OnMouseDown()
    {
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Gun)
        {
            if (GameManager.Instance.CurrentGameData.bulletCount > 0)
            {
                GameManager.Instance.CurrentGameData.bulletCount -= 1;
                SoundManager.Instance.PlaySFX(SFXType.Gun_Shot);
                TakeDamage(PlayerInventory.Instance.GunDamage);

            }
            else
            {
                NotificationManager.Instance.ShowNotification("총알이 부족합니다!");
            }
        }
    }

    private void CheckIfOffScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.x < -0.1f || screenPoint.x > 1.1f || screenPoint.y < -0.1f || screenPoint.y > 1.1f)
        {
            if (wolfManager != null)
            {
                wolfManager.ReturnWolfToPool(gameObject);
            }
        }
    }
}