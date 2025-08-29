using UnityEngine;
using System.Collections;
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

    /// <summary>
    /// 늑대를 초기화하고 체력과 공격력을 설정합니다.
    /// </summary>
    public void Initialize(WolfManager manager, float newHealth, float newDamage)
    {
        this.wolfManager = manager;
        this.health = newHealth;
        this.maxHealth = newHealth;
        this.damage = newDamage;
        FindNewTarget();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isReturning = false;

        // 체력바 초기화
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
                    Debug.Log("젖소 타겟이 모두 사라졌습니다. 임무 완수! 화면 밖으로 돌아갑니다.");
                    isReturning = true;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position + (transform.position - Camera.main.transform.position) * 10f, moveSpeed * Time.deltaTime);
            CheckIfOffScreen();
        }
    }

    private void FindNewTarget()
    {
        Animal closestCow = null;
        float closestDistance = Mathf.Infinity;
        Vector3 wolfPosition = transform.position;

        foreach (Animal cow in AnimalManager.Instance.activeAnimals)
        {
            // ★★★ 수정된 부분: 리스트에 null 값이 있으면 스킵하고 다음 항목으로 넘어갑니다. ★★★
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

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        if (health <= 0)
        {
            if (wolfManager != null)
            {
                wolfManager.ReturnWolfToPool(gameObject);
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
                Debug.Log("늑대가 화면 밖으로 도망쳤습니다! 풀로 돌아갑니다.");
                wolfManager.ReturnWolfToPool(gameObject);
            }
        }
    }
}