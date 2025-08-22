using UnityEngine;

// 유니티 메뉴에 이 스크립터블 오브젝트를 생성할 수 있는 메뉴를 추가합니다.
[CreateAssetMenu(fileName = "ChickenCoopData", menuName = "Jjayo/ChickenCoop Data")]
public class ChickenCoopData : ScriptableObject
{
    [Header("닭장 정보")]
    public string coopName;

    [Tooltip("닭 한 마리가 알 1개를 낳는 데 걸리는 시간(초).")]
    public float eggProductionInterval = 10f; // 기본값 10초

    [Tooltip("닭장 건물 구매 비용.")]
    public int purchaseCost = 5000;
}