using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapSys : UIbase
{
    public MapControl m_map;

    public override void DoCreate(string path)
    {
        base.DoCreate(path);
        Transform map = m_go.transform.Find("minimap/map");
        m_map = map.gameObject.AddComponent<MapControl>();//代码添加组件
    }

    public override void DoShow(bool v)
    {
        base.DoShow(v);
    }

    public override void Destory()
    {
        GameObject.Destroy(m_map);//删除脚本
        base.Destory();
    }
}
