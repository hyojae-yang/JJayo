using UnityEngine;

[CreateAssetMenu(fileName = "New Product", menuName = "Tycoon Game/Product Data")]
public class ProductData : ScriptableObject
{
    public string productName;
    public Sprite productIcon;
    public int productValue;
}