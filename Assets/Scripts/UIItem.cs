using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class UIItem
{
    public Dictionary<ShowType, Action> dic;
    public Item item;
    Text t,num;
    Button btn;
    Image ima;
    public UIItem(string name, Transform content)
    {
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(name),content,false);
        btn = go.GetComponent<Button>();
        t = go.transform.Find("Text").GetComponent<Text>();
        num = go.transform.Find("num").GetComponent<Text>();
        ima = go.transform.Find("icon").GetComponent<Image>();

        dic = new Dictionary<ShowType, Action>()
        {
            {ShowType.bag,()=>{Tips.Get().Show(item); } },
            {ShowType.shop,()=>{Tips.Get().Show(item); } }
        };
        btn.onClick.AddListener(() =>
        {
            dic[item.show].Invoke();
        });

        MessageManager.Ins.OnAddListen(MessageID.UseId, UseMsg);
    }

    private void UseMsg(object obj)
    {
        SetItem(0, item);
    }


    public void SetItem(int id, Item item)
    {
        this.item = item;
        if (item.data.cfg != null)
        {
            t.text = item.data.cfg.name;
            num.text = item.data.cfg.num.ToString();
            ima.sprite = Resources.Load<Sprite>(item.data.cfg.icon);
        }
        else
        {
            t.text = "";
            num.text = "";
        }
    }
}
