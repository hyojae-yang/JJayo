using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize;

    private List<GameObject> pool;

    void Awake()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform); // �θ� ObjectPool ������Ʈ�� ����
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetFromPool()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Ǯ�� ������ ������ Ǯ ����� �÷� ���� ����
        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(true);
        pool.Add(newObj);
        return newObj;
    }

    // �ڡڡ� �� �κ��� �߰����ּ��� �ڡڡ�
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        // �ʿ信 ���� ������Ʈ�� �ʱ� ���·� �����ϴ� ������ �߰��� �� �ֽ��ϴ�.
        // ���� ���, obj.transform.position = Vector3.one * 9999; ���� �ڵ带 �߰��� �� �ֽ��ϴ�.
    }
}