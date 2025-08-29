using UnityEngine;

public class GunController : MonoBehaviour
{
    void Update()
    {
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Gun)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // ★★★ 수정된 부분: GameData에서 총알 개수를 직접 확인합니다. ★★★
                if (GameManager.Instance.gameData.bulletCount > 0)
                {
                    Shoot();
                    // ★★★ 수정된 부분: GameData에서 총알을 직접 소모시킵니다. ★★★
                    GameManager.Instance.gameData.bulletCount -= 1;
                }
                else
                {
                    NotificationManager.Instance.ShowNotification("총알이 부족합니다!");
                }
            }
        }
    }

    void Shoot()
    {
        Vector2 mousePosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition2D);

        if (hitCollider != null)
        {
            Wolf wolfTarget = hitCollider.GetComponent<Wolf>();
            Animal animalTarget = hitCollider.GetComponent<Animal>();

            if (wolfTarget != null)
            {
                // 총기 데미지 값도 GameData에서 가져와야 합니다.
                // PlayerInventory는 현재 스크립트에서 더 이상 총알을 관리하지 않습니다.
                wolfTarget.TakeDamage(GameManager.Instance.gameData.gunDamage);
                NotificationManager.Instance.ShowNotification("늑대를 맞췄습니다!");
                return;
            }

            if (animalTarget != null && animalTarget.animalData != null)
            {
                animalTarget.TakeDamage(GameManager.Instance.gameData.gunDamage, gameObject);
                NotificationManager.Instance.ShowNotification("불쌍한 젖소를 쏘다니!");
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("허공에 총을 쐈습니다!");
        }
    }
}