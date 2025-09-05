using UnityEngine;
using TMPro;
using System;
using System.Collections;
using Unity.VisualScripting;

public class MonthlyReviewManager : MonoBehaviour
{
    public static MonthlyReviewManager Instance { get; private set; }

    [Header("UI Dependencies")]
    public GameObject reviewPanelUI;
    public TMP_Text playerReputationText;
    public TMP_Text playerMoneyText;
    public TMP_Text requiredReputationText;
    public TMP_Text requiredMoneyText;
    public TMP_Text resultMessageText;

    [Header("Review Settings")]
    public int baseRequiredReputation = 100;
    public int baseRequiredMoney = 10000;
    public int rewardReputation = 50;
    public int rewardMoney = 3000;

    private int totalMonthsPassed = 0;

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

    private void OnDestroy()
    {
        // 오브젝트가 파괴되기 직전에 이벤트 구독을 확실하게 해지합니다.
        // 이것이 MissingReferenceException을 방지하는 핵심입니다.
        if (TimeManager.Instance != null && !TimeManager.Instance.IsDestroyed())
        {
            TimeManager.Instance.OnMonthChanged -= OnMonthReview;
        }
    }

    public void Initialize()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMonthChanged += OnMonthReview;
        }
        else
        {
            Debug.LogError("MonthlyReviewManager: TimeManager 인스턴스를 찾을 수 없습니다. 초기화 실패.");
        }
    }

    private void OnMonthReview()
    {
        // 오브젝트가 파괴되었는지 다시 한번 확인하여 안전성을 확보합니다.
        if (this == null)
        {
            return;
        }

        totalMonthsPassed++;
        StartCoroutine(PerformReview());
    }

    private IEnumerator PerformReview()
    {
        // 게임 일시 정지
        Time.timeScale = 0f;
        GameManager.Instance.IsMenuOn = true;

        int currentRequiredReputation = baseRequiredReputation * (totalMonthsPassed);
        int currentRequiredMoney = baseRequiredMoney * (totalMonthsPassed);

        int currentReputation = GameManager.Instance.CurrentGameData.reputation;
        int currentMoney = MoneyManager.Instance.CurrentMoney;

        reviewPanelUI.SetActive(true);
        playerReputationText.text = $"현재 명성도: {currentReputation}";
        playerMoneyText.text = $"현재 보유 돈: {currentMoney}G";
        requiredReputationText.text = $"요구 명성도: {currentRequiredReputation}";
        requiredMoneyText.text = $"요구 돈: {currentRequiredMoney}G";

        string resultMessage;
        if (currentReputation >= currentRequiredReputation && currentMoney >= currentRequiredMoney)
        {
            GameManager.Instance.ChangeReputation(rewardReputation);
            MoneyManager.Instance.AddMoney(rewardMoney);
            resultMessage = "축하합니다! 이번 달 목표를 달성했습니다!\n명성도와 보너스 돈을 획득합니다.";
        }
        else
        {
            GameManager.Instance.ChangeReputation(-rewardReputation);
            resultMessage = "아쉽지만, 이번 달 목표를 달성하지 못했습니다.\n명성도가 감소합니다.";
        }
        resultMessageText.text = resultMessage;

        // UI 업데이트 후 한 프레임 대기
        yield return null;
    }

    public void OnContinueButtonClicked()
    {
        reviewPanelUI.SetActive(false);
        GameManager.Instance.IsMenuOn = false;
    }
}