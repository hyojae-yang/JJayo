using UnityEngine;
using System.Collections.Generic;

// ��� ���׷��̵� �������� �⺻ Ŭ����
public abstract class UpgradeData : ScriptableObject
{
    // �� ���׷��̵� �����Ͱ� �����ؾ� �� ���� �޼���
    public abstract int GetUpgradePrice(int currentLevel);
    public abstract int GetMaxLevel();
}