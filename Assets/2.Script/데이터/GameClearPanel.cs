using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI totalPlayTimeText;
    public TextMeshProUGUI totalMoneyEarnedText;
    public TextMeshProUGUI finalReputationText;
    public TextMeshProUGUI milkSoldText;
    public TextMeshProUGUI eggsSoldText;
    public TextMeshProUGUI cowsPurchasedText;
    public TextMeshProUGUI chickensPurchasedText;
    public TextMeshProUGUI cowsSoldText;
    public TextMeshProUGUI chickensSoldText;
    public TextMeshProUGUI wolvesKilledText;
    public TextMeshProUGUI cowsEatenText;

    // 이 메서드는 GameManager에서 게임이 끝났을 때 호출합니다.
    public void UpdateStatsUI()
    {
        GameData gameData = GameManager.Instance.CurrentGameData;

        // 총 플레이 시간 (초를 분과 초로 변환)
        int minutes = Mathf.FloorToInt(gameData.totalPlayTime / 60);
        int seconds = Mathf.FloorToInt(gameData.totalPlayTime % 60);
        totalPlayTimeText.text = $"총 플레이 시간: {minutes}분 {seconds}초";

        // 그 외 통계
        totalMoneyEarnedText.text = $"총 벌어들인 돈: {gameData.totalMoneyEarned.ToString()}원";
        finalReputationText.text = $"최종 명성도: {gameData.reputation.ToString()}";
        milkSoldText.text = $"판매한 우유 개수: {gameData.totalMilkSold.ToString()}개";
        eggsSoldText.text = $"판매한 달걀 개수: {gameData.totalEggsSold.ToString()}개";
        cowsPurchasedText.text = $"구매한 젖소 마릿수: {gameData.totalCowsPurchased.ToString()}마리";
        chickensPurchasedText.text = $"구매한 닭 마릿수: {gameData.totalChickensPurchased.ToString()}마리";
        cowsSoldText.text = $"판매한 젖소 마릿수: {gameData.totalCowsSold.ToString()}마리";
        chickensSoldText.text = $"판매한 닭 마릿수: {gameData.totalChickensSold.ToString()}마리";
        wolvesKilledText.text = $"잡은 늑대 마릿수: {gameData.totalWolvesKilled.ToString()}마리";
        cowsEatenText.text = $"먹힌 젖소 마릿수: {gameData.totalCowsEaten.ToString()}마리";
    }
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}