using UnityEngine;

public class WolfSummoner : MonoBehaviour
{
    // 인스펙터에 WolfManager를 연결할 변수
    public WolfManager wolfManager;

    void Update()
    {
        // 'W' 키를 눌렀을 때 늑대를 소환합니다.
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (wolfManager != null)
            {
                wolfManager.SpawnWolf();
                Debug.Log("개발자 소환 코드: 늑대를 즉시 소환합니다!");
            }
            else
            {
                Debug.LogError("Wolf Manager가 할당되지 않았습니다. 인스펙터에서 연결해주세요.");
            }
        }
    }
}