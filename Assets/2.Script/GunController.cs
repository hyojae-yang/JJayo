using UnityEngine;

public class GunController : MonoBehaviour
{
    private float fireRate => PlayerInventory.Instance.GunFireRate;
    private float nextFireTime = 0f;

    void Update()
    {
        if (EquipmentManager.Instance.currentEquipment == EquipmentType.Gun)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextFireTime)
                {
                    if (PlayerInventory.Instance.currentBullets > 0)
                    {
                        Shoot();
                        PlayerInventory.Instance.AddBullets(-1);
                        nextFireTime = Time.time + fireRate;
                    }
                    else
                    {
                        NotificationManager.Instance.ShowNotification("총알이 부족합니다!");
                    }
                }
            }
        }
    }

    void Shoot()
    {
        // 마우스의 스크린 좌표를 2D 월드 좌표로 변환합니다.
        Vector2 mousePosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ★★★ Physics2D.Raycast 대신 Physics2D.OverlapPoint를 사용합니다. ★★★
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition2D);

        // 무언가 충돌했는지 확인합니다.
        if (hitCollider != null)
        {
            Wolf wolfTarget = hitCollider.GetComponent<Wolf>();
            Animal animalTarget = hitCollider.GetComponent<Animal>();

            // ★★★ 수정된 부분: 늑대에게 피해를 줍니다. ★★★
            if (wolfTarget != null)
            {
                wolfTarget.TakeDamage(PlayerInventory.Instance.GunDamage);
                NotificationManager.Instance.ShowNotification("늑대를 맞췄습니다!");
                return; // 늑대를 맞췄으니 더 이상 검사할 필요가 없습니다.
            }

            // ★★★ 수정된 부분: 젖소에게 피해를 줍니다. ★★★
            if (animalTarget != null && animalTarget.animalData != null)
            {
                // ★★★ Animal.TakeDamage() 메서드가 수정되었으므로 매개변수를 추가합니다. ★★★
                animalTarget.TakeDamage(PlayerInventory.Instance.GunDamage, gameObject);
                NotificationManager.Instance.ShowNotification("불쌍한 젖소를 쏘다니!");
            }
        }
        else
        {
            // 아무것도 맞추지 않았을 때의 피드백
            NotificationManager.Instance.ShowNotification("허공에 총을 쐈습니다!");
        }
    }
}