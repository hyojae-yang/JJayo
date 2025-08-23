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
        // �˸�â�� ���� Ȱ��ȭ ���̶�� ���� �ڷ�ƾ�� ����
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // �ؽ�Ʈ ������Ʈ �� �˸�â�� ���̰� ��
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        // ���� �ð� �� �˸��� ����� �ڷ�ƾ ����
        hideCoroutine = StartCoroutine(HideAfterDelay(displayDuration));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationText.gameObject.SetActive(false);
    }
}