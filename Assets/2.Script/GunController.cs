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
                        NotificationManager.Instance.ShowNotification("�Ѿ��� �����մϴ�!");
                    }
                }
            }
        }
    }

    void Shoot()
    {
        // ���콺�� ��ũ�� ��ǥ�� 2D ���� ��ǥ�� ��ȯ�մϴ�.
        Vector2 mousePosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �ڡڡ� Physics2D.Raycast ��� Physics2D.OverlapPoint�� ����մϴ�. �ڡڡ�
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition2D);

        // ���� �浹�ߴ��� Ȯ���մϴ�.
        if (hitCollider != null)
        {
            Wolf wolfTarget = hitCollider.GetComponent<Wolf>();
            Animal animalTarget = hitCollider.GetComponent<Animal>();

            // �ڡڡ� ������ �κ�: ���뿡�� ���ظ� �ݴϴ�. �ڡڡ�
            if (wolfTarget != null)
            {
                wolfTarget.TakeDamage(PlayerInventory.Instance.GunDamage);
                NotificationManager.Instance.ShowNotification("���븦 ������ϴ�!");
                return; // ���븦 �������� �� �̻� �˻��� �ʿ䰡 �����ϴ�.
            }

            // �ڡڡ� ������ �κ�: ���ҿ��� ���ظ� �ݴϴ�. �ڡڡ�
            if (animalTarget != null && animalTarget.animalData != null)
            {
                // �ڡڡ� Animal.TakeDamage() �޼��尡 �����Ǿ����Ƿ� �Ű������� �߰��մϴ�. �ڡڡ�
                animalTarget.TakeDamage(PlayerInventory.Instance.GunDamage, gameObject);
                NotificationManager.Instance.ShowNotification("�ҽ��� ���Ҹ� ��ٴ�!");
            }
        }
        else
        {
            // �ƹ��͵� ������ �ʾ��� ���� �ǵ��
            NotificationManager.Instance.ShowNotification("����� ���� �����ϴ�!");
        }
    }
}