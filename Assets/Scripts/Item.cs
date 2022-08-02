using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShowType
{
    nullType=-1,
    bag,
    shop,
    Max=99
}
public enum ComponentType
{
    nullType=-1,
    use,
    sell,
    equip,
    Max=99
}
public class Item
{
    public ItemData data;
    public ShowType show;
    public Dictionary<ComponentType, ComponentBase> dic = new Dictionary<ComponentType, ComponentBase>();
    public static Item CreateEntityByID(int id,ShowType type)
    {
        Item item = new Item();
        
        item.CreateByID(id, type);
        return item;
    }

    private void CreateByID(int id, ShowType type)
    {
        data = new ItemData(id,type);
        InJectAction(type);
    }

    private void InJectAction(ShowType type)
    {
        show = type;
        if (data.cfg != null)
        {
            if (data.cfg.isequip == 1)
            {
                Equip equip = new Equip();
                dic.Add(ComponentType.equip, equip);
            }
            if (data.cfg.isuse == 1)
            {
                Use use = new Use();
                dic.Add(ComponentType.use, use);
            }
            if (data.cfg.issell == 1)
            {
                Sell sell = new Sell();
                dic.Add(ComponentType.sell, sell);
            }
        }
      
    }
}
