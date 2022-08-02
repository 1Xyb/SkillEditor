using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ECSMain : MonoBehaviour
{
    public Transform content;
    public Button bag, shop;
    UIItem iItem;
    // Start is called before the first frame update
    void Start()
    {
        bag.onClick.AddListener(() =>
        {
            Clear();
            for (int i = 0; i < 9; i++)
            {
                iItem = new UIItem("item", content);
                iItem.SetItem(i, Item.CreateEntityByID(i, ShowType.bag));
            }
        });
        shop.onClick.AddListener(() =>
        {
            Clear();
            for (int i = 0; i < 9; i++)
            {
                iItem = new UIItem("item", content);
                iItem.SetItem(i, Item.CreateEntityByID(i, ShowType.shop));
            }
        });
    }

    

    public void Clear()
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
