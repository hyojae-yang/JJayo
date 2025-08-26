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

    [Header("�ް� �Ǹ� ��� UI")]
    [Tooltip("�ް� �Ǹ� ����� ǥ���� �ؽ�Ʈ")]
    public TextMeshProUGUI eggResultText; // �߰��� ����

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
    /// <param name="moneyChange">�� ��ȭ�� (���� �ŷ�)</param>
    /// <param name="reputationChange">���� ��ȭ��</param>
    /// <param name="message">�� �޽���</param>
    /// <param name="eggsSold">�Ǹ��� �ް� ����</param>
    /// <param name="eggRevenue">�ް� �Ǹŷ� ���� �� �ݾ�</param>
    public void DisplayResult(bool success, int moneyChange, int reputationChange, string message, int eggsSold = 0, int eggRevenue = 0)
    {
        // 1. ���� ����
        titleText.text = success ? "�ŷ� ����!" : "�ŷ� ����";

        // 2. ���� �ŷ� �� ��ȭ ǥ�� (+/- ��ȣ�� �Բ�)
        string moneySign = moneyChange >= 0 ? "+" : "-";
        moneyText.text = $"���� �Ǹ�: {moneySign}{Mathf.Abs(moneyChange)} ���";

        // 3. ���� ��ȭ ǥ�� (+/- ��ȣ�� �Բ�)
        string repSign = reputationChange >= 0 ? "+" : "";
        reputationText.text = $"���� ��ȭ: {repSign}{reputationChange} ��";

        // 4. �� �޽��� ����
        detailMessageText.text = message;

        // 5. �ް� �Ǹ� ��� ǥ�� (���� �߰��� �κ�)
        if (eggResultText != null)
        {
            if (eggsSold > 0)
            {
                eggResultText.text = $"�ް� �Ǹ�: {eggsSold}�� ({eggRevenue} ���)";
            }
            else
            {
                eggResultText.text = "�ް� �Ǹ�: ����";
            }
        }

        // 6. �г� Ȱ��ȭ
        resultPanel.SetActive(true);
        Time.timeScale = 0; // ���â�� ������ ���� �ð� ����
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
        // 2. ���� �г� �ݱ�
        if (traderManager != null && traderManager.traderUIPanel != null)
        {
            traderManager.traderUIPanel.SetActive(false);
        }

        // 3. ���� �ð� �簳
        Time.timeScale = 1;
    }
}