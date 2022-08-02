using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataCfg
{
    public int id, isuse, issell, isequip,num;
    public string showType, icon, name;
}
public class ItemData
{
    public List<DataCfg> list = new List<DataCfg>();
    public List<DataCfg> Baglist = new List<DataCfg>();
    public List<DataCfg> Shoplist = new List<DataCfg>();
    public DataCfg cfg;
    public ItemData(int id,ShowType type)
    {
        string str = System.IO.File.ReadAllText(Application.dataPath + "/data.json");
        list = JsonConvert.DeserializeObject<List<DataCfg>>(str);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].showType == "bag")
            {
                Baglist.Add(list[i]);
            }
            else
            {
                Shoplist.Add(list[i]);
            }
        }
        if (type == ShowType.bag&&id<Baglist.Count)
        {
           cfg= Baglist[id];
        }
        else if (type == ShowType.shop&&id<Shoplist.Count)
        {
            cfg = Shoplist[id];
        }
    }
}
