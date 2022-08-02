using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticCircleCheck : MonoBehaviour
{
    public float m_checkRangeActive = 2f;//在2的范围内开始启动检测
    public float m_triggerRange = 0.2f;//触发半径
    public GameObject m_target;//目标物体
    public Action<bool> m_call;//委托
    public bool m_isTrigger = true;

    CircleCollision circle1,circle2;
    // Start is called before the first frame update
    void Start()
    {
        circle1 = new CircleCollision(transform.position.x, transform.position.z, m_triggerRange);
        circle2 = new CircleCollision(0, 0, m_triggerRange);//圆2默认是0,0位置
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target)
        {
            if (Vector3.Distance(transform.position, m_target.transform.position) <= m_checkRangeActive)
            {
                circle2.RefreshPos(m_target.transform.position.x, m_target.transform.position.z);
                if (CircleCollision.CircleCollisionMsg(circle1, circle2))//如果两个圆相交了
                {
                    if (m_isTrigger)
                    {
                        m_call(true);//走事件回调
                        m_isTrigger = false;//仅仅执行一次
                    }
                }
                else
                {
                    //如果两圆没有相交
                    if (!m_isTrigger)
                    {
                        m_isTrigger = true;
                    }
                }
            }
        }
    }
}
