using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemData> _items = new List<ItemData>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        foreach (ItemData item in Resources.LoadAll<ItemData>("Items"))
        {
            _items.Add(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
