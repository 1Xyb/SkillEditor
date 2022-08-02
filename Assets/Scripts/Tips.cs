using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips
{
    static GameObject tip;
    public static Button btn;
    public static Transform content;
    static Tips T;
    private Item item;
    public static Tips Get()
    {
        if (T == null)
        {
            T = new Tips();
            tip = GameObject.Instantiate(Resources.Load<GameObject>("tips"), GameObject.Find("Canvas").transform, false);
            btn = tip.transform.Find("close").GetComponent<Button>();
            content = tip.transform.Find("Scroll View/Viewport/Content");
            btn.onClick.AddListener(() =>
            {
                foreach (Transform item in content)
                {
                    GameObject.Destroy(item.gameObject);
                }
                tip.SetActive(false);
            });
        }
        return T;
    }

    internal void Show(Item item)
    {
        tip.SetActive(true);
        this.item = item;
        foreach (var v in item.dic)
        {
            Button btn = GameObject.Instantiate(Resources.Load<Button>("Button"), content, false);
            Text t = btn.GetComponentInChildren<Text>();
            t.text = v.Value.GetName();
            btn.onClick.AddListener(() =>
            {
                v.Value.Do(item.data.cfg.id,item);
            });
        }
    }
}
