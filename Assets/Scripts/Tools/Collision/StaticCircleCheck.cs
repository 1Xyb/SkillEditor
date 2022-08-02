using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticCircleCheck : MonoBehaviour
{
    public float m_checkRangeActive = 2f;//��2�ķ�Χ�ڿ�ʼ�������
    public float m_triggerRange = 0.2f;//�����뾶
    public GameObject m_target;//Ŀ������
    public Action<bool> m_call;//ί��
    public bool m_isTrigger = true;

    CircleCollision circle1,circle2;
    // Start is called before the first frame update
    void Start()
    {
        circle1 = new CircleCollision(transform.position.x, transform.position.z, m_triggerRange);
        circle2 = new CircleCollision(0, 0, m_triggerRange);//Բ2Ĭ����0,0λ��
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target)
        {
            if (Vector3.Distance(transform.position, m_target.transform.position) <= m_checkRangeActive)
            {
                circle2.RefreshPos(m_target.transform.position.x, m_target.transform.position.z);
                if (CircleCollision.CircleCollisionMsg(circle1, circle2))//�������Բ�ཻ��
                {
                    if (m_isTrigger)
                    {
                        m_call(true);//���¼��ص�
                        m_isTrigger = false;//����ִ��һ��
                    }
                }
                else
                {
                    //�����Բû���ཻ
                    if (!m_isTrigger)
                    {
                        m_isTrigger = true;
                    }
                }
            }
        }
    }
}
