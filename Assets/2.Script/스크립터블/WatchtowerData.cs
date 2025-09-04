using UnityEngine;

[CreateAssetMenu(fileName = "New Watchtower Data", menuName = "Tycoon Game/Watchtower Data")]
public class WatchtowerData : ScriptableObject
{
    [Header("Watchtower Stats")]
    public float fireRate = 1f; // 초당 공격 횟수
    public int bulletConsumption = 1; // 1회 공격 시 소모되는 총알 수
}