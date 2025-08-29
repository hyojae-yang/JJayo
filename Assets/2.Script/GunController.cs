using UnityEngine;

public class GunController : MonoBehaviour
{
    void Update()
    {
        if (EquipmentManager.Instance.GetCurrentEquipment() == EquipmentType.Gun)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // �ڡڡ� ������ �κ�: GameData���� �Ѿ� ������ ���� Ȯ���մϴ�. �ڡڡ�
                if (GameManager.Instance.gameData.bulletCount > 0)
                {
                    Shoot();
                    // �ڡڡ� ������ �κ�: GameData���� �Ѿ��� ���� �Ҹ��ŵ�ϴ�. �ڡڡ�
                    GameManager.Instance.gameData.bulletCount -= 1;
                }
                else
                {
                    NotificationManager.Instance.ShowNotification("�Ѿ��� �����մϴ�!");
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
                // �ѱ� ������ ���� GameData���� �����;� �մϴ�.
                // PlayerInventory�� ���� ��ũ��Ʈ���� �� �̻� �Ѿ��� �������� �ʽ��ϴ�.
                wolfTarget.TakeDamage(GameManager.Instance.gameData.gunDamage);
                NotificationManager.Instance.ShowNotification("���븦 ������ϴ�!");
                return;
            }

            if (animalTarget != null && animalTarget.animalData != null)
            {
                animalTarget.TakeDamage(GameManager.Instance.gameData.gunDamage, gameObject);
                NotificationManager.Instance.ShowNotification("�ҽ��� ���Ҹ� ��ٴ�!");
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("����� ���� �����ϴ�!");
        }
    }
}