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
            GameObject obj = Instantiate(prefab, transform); // 부모를 ObjectPool 오브젝트로 설정
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

        // 풀에 여유가 없으면 풀 사이즈를 늘려 새로 생성
        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(true);
        pool.Add(newObj);
        return newObj;
    }
}