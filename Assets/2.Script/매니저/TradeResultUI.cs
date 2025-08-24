using UnityEngine;
using TMPro;

public class TradeResultUI : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static TradeResultUI Instance { get; private set; }

    [Header("Dependencies")]
    [Tooltip("����� ǥ���� UI �г� ��ü")]
    public GameObject resultPanel;
    [Tooltip("��� ���� �ؽ�Ʈ (����/����)")]
    public TextMeshProUGUI titleText;
    [Tooltip("�� ��ȭ�� �ؽ�Ʈ")]
    public TextMeshProUGUI moneyText;
    [Tooltip("���� ��ȭ�� �ؽ�Ʈ")]
    public TextMeshProUGUI reputationText;
    [Tooltip("�� �޽��� �ؽ�Ʈ")]
    public TextMeshProUGUI detailMessageText;

    // ���� �Ŵ��� ��ũ��Ʈ�� �����Ͽ� �г��� ���� �� �ְ� �մϴ�.
    [Tooltip("���� �Ŵ��� ��ũ��Ʈ")]
    public TraderManager traderManager;

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

        if (resultPanel != null)
        {
            // ���� �ÿ��� ��Ȱ��ȭ ���·� �Ӵϴ�.
            resultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// TraderManager�κ��� ����� �޾� UI�� ǥ���մϴ�.
    /// </summary>
    /// <param name="success">�ŷ� ���� ����</param>
    /// <param name="moneyChange">�� ��ȭ��</param>
    /// <param name="reputationChange">���� ��ȭ��</param>
    /// <param name="message">�� �޽���</param>
    public void DisplayResult(bool success, int moneyChange, int reputationChange, string message)
    {
        // 1. ���� ����
        titleText.text = success ? "�ŷ� ����!" : "�ŷ� ����";

        // 2. �� ��ȭ ǥ�� (+/- ��ȣ�� �Բ�)
        string moneySign = moneyChange >= 0 ? "+" : "-";
        moneyText.text = $"ȹ�� �ڱ�: {moneySign}{Mathf.Abs(moneyChange)} ���";

        // 3. ���� ��ȭ ǥ�� (+/- ��ȣ�� �Բ�)
        string repSign = reputationChange >= 0 ? "+" : "";
        reputationText.text = $"���� ��ȭ: {repSign}{reputationChange} ��";

        // 4. �� �޽��� ����
        detailMessageText.text = message;

        // 5. �г� Ȱ��ȭ
        resultPanel.SetActive(true);
    }

    /// <summary>
    /// UI�� 'Ȯ��' ��ư Ŭ�� �� ȣ��˴ϴ�.
    /// </summary>
    public void OnConfirmButtonClicked()
    {
        // 1. ��� �г� ��Ȱ��ȭ
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        // 3. ���� �г� �ݱ�
        if (traderManager != null && traderManager.traderUIPanel != null)
        {
            traderManager.traderUIPanel.SetActive(false);
        }

        // 2. ���� �ð� �簳
        Time.timeScale = 1;

        
        
    }
}