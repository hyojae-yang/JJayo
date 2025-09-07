using UnityEngine;
using System.Collections;

public class Watchtower : MonoBehaviour
{
    [Header("Dependencies")]
    public WatchtowerData watchtowerData;
    private PlayerInventory playerInventory;
    private GameManager gameManager;
    private NotificationManager notificationManager;

    [Header("Logic")]
    private float fireCooldownTimer;
    private Transform targetWolf;

    // ★★★ 수정된 부분: 이벤트 구독 및 해제 로직 추가 ★★★
    void OnEnable()
    {
        Animal.OnCowAttackedByWolf += SetTargetWolf;
    }

    void OnDisable()
    {
        Animal.OnCowAttackedByWolf -= SetTargetWolf;
    }

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;
        notificationManager = NotificationManager.Instance;
    }

    void Update()
    {
        // ★★★ 변경된 로직: 타겟이 있을 때만 공격합니다. ★★★
        if (targetWolf != null && targetWolf.gameObject.activeInHierarchy)
        {
            AttackTarget();
        }
        else
        {
            targetWolf = null;
        }
    }

    // ★★★ 추가된 메서드: 늑대 공격 이벤트가 발생했을 때 호출됩니다. ★★★
    void SetTargetWolf(Transform wolfTransform)
    {
        // 이미 타겟이 있다면 새로운 타겟을 설정하지 않습니다. (최초 공격 늑대에게 집중)
        if (targetWolf == null)
        {
            targetWolf = wolfTransform;
            notificationManager.ShowNotification("감시탑이 늑대 공격을 감지했습니다!");
        }
    }

    void AttackTarget()
    {
        fireCooldownTimer -= Time.deltaTime;

        if (fireCooldownTimer <= 0)
        {
            if (gameManager.CurrentGameData.bulletCount > 0)
            {
                // 총알 소모
                gameManager.CurrentGameData.bulletCount -= watchtowerData.bulletConsumption;

                // 늑대에게 데미지 적용
                Wolf wolfComponent = targetWolf.GetComponent<Wolf>();
                if (wolfComponent != null)
                {
                    wolfComponent.TakeDamage(playerInventory.GunDamage);
                    SoundManager.Instance.PlaySFX(SFXType.Gun_Shot);
                    NotificationManager.Instance.ShowNotification("감시탑이 늑대를 공격했습니다! 남은 총알: " + gameManager.CurrentGameData.bulletCount);
                }

                // 쿨다운 리셋
                fireCooldownTimer = 1f / watchtowerData.fireRate;
            }
            else
            {
                // 총알 부족 알림
                notificationManager.ShowNotification("감시탑에 총알이 부족합니다!");
                fireCooldownTimer = 3f; // 총알이 없으면 3초 후에 다시 확인
            }
        }
    }
}