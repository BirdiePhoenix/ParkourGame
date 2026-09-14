using System;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    [SerializeField]private ItemData itemData;
    
    private string _itemName;
    private string _itemDescription;
    private int _increamentValue;
    
    private Collider _collider;
    private void Awake()
    {
        _collider = gameObject.GetComponent<BoxCollider>();
        initialize();
    }

    private void initialize()
    {
        _itemName = itemData.name;
        _itemDescription = itemData.description;
        _increamentValue = itemData.IncreamentValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        List<ItemData> inventoryItems = other.GetComponent<PlayerInventory>()._items;
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].name == _itemName)
            {
                inventoryItems[i].currentValue += _increamentValue;
            } 
        }
            
        
    }
}
