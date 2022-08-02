using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public float xMap, yMap;
    public float xoffset, yoffset;

    public Transform player;
                        //��������      ui�ϵĶ���
    Dictionary<MonsterType, Transform> monsterdic = new Dictionary<MonsterType, Transform>();

    List<ObjectBase> otherGoPos = new List<ObjectBase>();
    Vector3 playerpos = Vector3.zero;//Ĭ����ҳ�ʼλ��
    List<Vector3> otherpos = new List<Vector3>();

    private void Awake()
    {
        xMap = gameObject.GetComponent<RectTransform>().sizeDelta.x;
        yMap = gameObject.GetComponent<RectTransform>().sizeDelta.y;
        xoffset = xMap / World.Instance.xlength;//С��ͼ���ͼ�ı�ֵ
        yoffset = yMap / World.Instance.ylength;

        //UI��ʼ��
        player = transform.Find("player");
        monsterdic.Add(MonsterType.Gather, transform.Find("gather"));
        monsterdic.Add(MonsterType.Normal, transform.Find("monster"));
        monsterdic.Add(MonsterType.NPC, transform.Find("npc"));
    }

    private void Update()
    {
        //�������������������=С��ͼ�Ϲ��������
        if (World.Instance.m_insDic.Count != otherGoPos.Count)
        {
            otherGoPos.Clear();
            otherpos.Clear();
            foreach (var item in World.Instance.m_insDic)
            {
                otherGoPos.Add(item.Value);//���
                otherpos.Add(new Vector3(0, 0, 0));//�������
            }
        }
        //��������С��ͼ�ϵ���Ҷ�����
        if (player && World.Instance.m_plyer.m_go)
        {
            //�����С��ͼ�ϵ�λ��        �����������*С��ͼ/�����ͼ
            playerpos.Set(World.Instance.m_plyer.m_go.transform.position.x * xoffset, World.Instance.m_plyer.m_go.transform.position.z * yoffset, 0);
            player.localPosition = playerpos;
        }
        //���С��ͼ�Ϲ�����������0
        if (otherGoPos != null && otherGoPos.Count > 0)
        {
            for (int i = 0; i < otherGoPos.Count; i++)
            {
                //���ù�����С��ͼ�ϵ�����
                otherpos[i] = new Vector3(otherGoPos[i].m_go.transform.position.x * xoffset, otherGoPos[i].m_go.transform.position.z * yoffset, 0);
                monsterdic[otherGoPos[i].m_type].transform.localPosition = otherpos[i];
            }
        }
    }
}
