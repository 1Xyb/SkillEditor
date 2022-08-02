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
        return "ʹ��";
    }
    public override void Do(int id, Item item)
    {
        base.Do(id, item);
        item.data.cfg.num--;
        string str = JsonConvert.SerializeObject(item.data.list);
        System.IO.File.WriteAllText(Application.dataPath + "/data.json", str);
        MessageManager.Ins.OnDisPatch(MessageID.UseId);//����Ϣ
        Debug.Log("ʹ��");
    }
}

public class Sell : ComponentBase
{
    public override string GetName()
    {
        return "����";
    }
    public override void Do(int id, Item item)
    {
        base.Do(id, item);
        Debug.Log("����");
    }
}
public class Equip : ComponentBase
{
    public override string GetName()
    {
        return "װ��";
    }
    public override void Do(int id, Item item)
    {
        base.Do(id,item);
        Debug.Log("װ��");
    }
}
