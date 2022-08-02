using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public float xMap, yMap;
    public float xoffset, yoffset;

    public Transform player;
                        //怪物类型      ui上的对象
    Dictionary<MonsterType, Transform> monsterdic = new Dictionary<MonsterType, Transform>();

    List<ObjectBase> otherGoPos = new List<ObjectBase>();
    Vector3 playerpos = Vector3.zero;//默认玩家初始位置
    List<Vector3> otherpos = new List<Vector3>();

    private void Awake()
    {
        xMap = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        yMap = gameObject.GetComponent<RectTransform>().sizeDelta.y;
        xoffset = xMap / World.Instance.xlength;//小地图与地图的比值
        yoffset = yMap / World.Instance.ylength;

        //UI初始化
        player = transform.Find("player");
        monsterdic.Add(MonsterType.Gather, transform.Find("gather"));
        monsterdic.Add(MonsterType.Normal, transform.Find("monster"));
        monsterdic.Add(MonsterType.NPC, transform.Find("npc"));
    }

    private void Update()
    {
        //如果世界里怪物的数量！=小地图上怪物的数量
        if (World.Instance.m_insDic.Count != otherGoPos.Count)
        {
            otherGoPos.Clear();
            otherpos.Clear();
            foreach (var item in World.Instance.m_insDic)
            {
                otherGoPos.Add(item.Value);//添加
                otherpos.Add(new Vector3(0, 0, 0));//添加坐标
            }
        }
        //如果世界和小地图上的玩家都存在
        if (player && World.Instance.m_plyer.m_go)
        {
            //玩家在小地图上的位置        玩家世界坐标*小地图/世界地图
            playerpos.Set(World.Instance.m_plyer.m_go.transform.position.x * xoffset, World.Instance.m_plyer.m_go.transform.position.z * yoffset, 0);
            player.localPosition = playerpos;
        }
        //如果小地图上怪物数量大于0
        if (otherGoPos != null && otherGoPos.Count > 0)
        {
            for (int i = 0; i < otherGoPos.Count; i++)
            {
                //设置怪物在小地图上的坐标
                otherpos[i] = new Vector3(otherGoPos[i].m_go.transform.position.x * xoffset, otherGoPos[i].m_go.transform.position.z * yoffset, 0);
                monsterdic[otherGoPos[i].m_type].transform.localPosition = otherpos[i];
            }
        }
    }
}
