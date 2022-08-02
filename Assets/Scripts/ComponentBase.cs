using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public abstract class ComponentBase 
{
    public virtual void Do(int id, Item item) { }
    public abstract string GetName();
}
public class Use : ComponentBase
{
    public override string GetName()
    {
        return "使用";
    }
    public override void Do(int id, Item item)
    {
        base.Do(id, item);
        item.data.cfg.num--;
        string str = JsonConvert.SerializeObject(item.data.list);
        System.IO.File.WriteAllText(Application.dataPath + "/data.json", str);
        MessageManager.Ins.OnDisPatch(MessageID.UseId);//发消息
        Debug.Log("使用");
    }
}

public class Sell : ComponentBase
{
    public override string GetName()
    {
        return "出售";
    }
    public override void Do(int id, Item item)
    {
        base.Do(id, item);
        Debug.Log("出售");
    }
}
public class Equip : ComponentBase
{
    public override string GetName()
    {
        return "装备";
    }
    public override void Do(int id, Item item)
    {
        base.Do(id,item);
        Debug.Log("装备");
    }
}
