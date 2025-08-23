using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI 요소 연결")]
    public Slider milkerGauge;
    public Slider basketGauge;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI dayText;

    [Header("매니저 연결")]
    private PlayerInventory playerInventory;
    private GameManager gameManager;

    void Start()
    {
        playerInventory = PlayerInventory.Instance;
        gameManager = GameManager.Instance;

        // GameManager의 OnMoneyChanged 이벤트에 UpdateMoney 함수를 구독
        gameManager.OnMoneyChanged += UpdateMoney;

        // 착유기 게이지의 최대값을 PlayerInventory의 용량으로 설정
        milkerGauge.maxValue = playerInventory.milkerCapacity;

        // 바구니 게이지의 최대값을 PlayerInventory의 용량으로 설정
        basketGauge.maxValue = playerInventory.basketCapacity;

        // 게임 시작 시 한 번 업데이트
        UpdateMoney(gameManager.CurrentMoney);
    }

    void Update()
    {
        UpdateGauges();
        dayText.text = gameManager.CurrentDate; // 날짜 텍스트는 매 프레임 업데이트
    }

    private void UpdateGauges()
    {
        // 착유기 게이지 업데이트
        milkerGauge.value = playerInventory.currentMilkFreshness.Count;

        // 바구니 게이지 업데이트
        basketGauge.value = playerInventory.currentEggs;
    }

    /// <summary>
    /// 돈 UI를 업데이트하는 함수
    /// </summary>
    /// <param name="newMoney">새로운 돈의 양</param>
    private void UpdateMoney(int newMoney)
    {
        moneyText.text = newMoney.ToString("C0");
    }
}