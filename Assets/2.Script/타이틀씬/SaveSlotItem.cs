using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SaveSlotItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI slotNameText;
    public TextMeshProUGUI lastSavedDateText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI reputationText;

    // 인스펙터에서 직접 연결할 수 있도록 public으로 변경
    public Button slotButton;

    public void Setup(string name, string date, int money, int reputation)
    {
        slotNameText.text = name;
        lastSavedDateText.text = date;
        moneyText.text = $"{money} 원";
        reputationText.text = $"{reputation} 명성";
    }

    public void SetupEmptySlot(string name)
    {
        slotNameText.text = name;
        lastSavedDateText.text = "새 게임";
        moneyText.text = "-";
        reputationText.text = "-";
    }

    public void AddListener(UnityAction action)
    {
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(action);
        }
        else
        {
            Debug.LogError("오류: SaveSlotItem에 버튼 컴포넌트가 연결되지 않았습니다. 인스펙터에서 Button 변수를 연결해주세요.");
        }
    }
}