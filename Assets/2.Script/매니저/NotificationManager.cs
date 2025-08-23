using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    public TextMeshProUGUI notificationText;
    public float displayDuration = 3f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowNotification(string message)
    {
        // 알림창이 현재 활성화 중이라면 기존 코루틴을 중지
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // 텍스트 업데이트 후 알림창을 보이게 함
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        // 일정 시간 후 알림을 숨기는 코루틴 시작
        hideCoroutine = StartCoroutine(HideAfterDelay(displayDuration));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationText.gameObject.SetActive(false);
    }
}