using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollision
{
    public float x, y, r;
    public CircleCollision(float x,float y,float r)
    {
        this.x = x;
        this.y = y;
        this.r = r;
    }

    //ˢ������
    public void RefreshPos(float x,float y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// �ж���Բ�Ƿ��ཻ
    /// </summary>
    public static bool CircleCollisionMsg(CircleCollision circle1,CircleCollision circle2)
    {
        bool result = false;
        float dis = Vector2.Distance(new Vector2(circle1.x, circle1.y), new Vector2(circle2.x, circle2.y));
        result = dis <= (circle1.r + circle2.r);//�ж�Բ�ľ�����뾶֮��
        return result;
    }
}

