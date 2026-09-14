using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string name;
    public string description;
    public int IncreamentValue;
    public int currentValue = 0;
}