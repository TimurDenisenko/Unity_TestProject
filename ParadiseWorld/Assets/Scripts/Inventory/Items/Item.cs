using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
[Serializable]
public abstract class Item : ScriptableObject
{
    public string Name;
    public bool IsStackable;
    public Sprite Icon;
    public GameObject ItemObject;
}
