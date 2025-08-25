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

    // ★★★ 이 부분을 추가해주세요 ★★★
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        // 필요에 따라 오브젝트를 초기 상태로 리셋하는 로직을 추가할 수 있습니다.
        // 예를 들어, obj.transform.position = Vector3.one * 9999; 같은 코드를 추가할 수 있습니다.
    }
}