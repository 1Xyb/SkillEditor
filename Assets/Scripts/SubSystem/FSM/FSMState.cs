using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// </summary>
public enum Transition
{
    NullTransition=-1,
    SeePlayer,
    LostPlayer,//�������
    Max=99
}

/// <summary>
/// ״̬ID
/// </summary>
public enum StateID
{
    NullStateID=-1,
    Patrol,//Ѳ��
    Chase,//׷��
    Max=99
}

/// <summary>
/// ״̬����
/// </summary>
public abstract class FSMState 
{
    protected StateID stateID;//״̬ID
    public StateID ID { get { return stateID; } } //�Լ���װ������
    //������   ��ǰ״̬����ͨ����Щ����������Щ״̬
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
    //״̬�������״̬��ϵͳ
    protected FSMSystem fsm;

    public FSMState(FSMSystem fsm)
    {
        this.fsm = fsm;
    }

    /// <summary>
    /// ��
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="id"></param>
    public void AddTransition(Transition trans,StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.Log("������NullTransition");return;
        }
        if (id == StateID.NullStateID)
        {
            Debug.LogError("������NullStateID"); return;
        }
        if (map.ContainsKey(trans))
        {
            Debug.LogError("���ת��������ʱ��" + trans + "�Ѿ�������map��"); return;
        }
        map.Add(trans, id);//���
    }

    /// <summary>
    /// ɾ
    /// </summary>
    /// <param name="trans"></param>
    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("������NullTransition"); return;
        }
        if (map.ContainsKey(trans) == false)
        {
            Debug.LogError("ɾ��ת��������ʱ��" + trans + "��������map��"); return;
        }
        map.Remove(trans);
    }

    /// <summary>
    /// ��
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public StateID GetOutputState(Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    //����ǰҡ��ҡ
    public virtual void DoBeforeEntering() { }//����״̬Ǯ
    public virtual void DoAfterLeaving() { }//�뿪״̬��
    //��������ʵ�ֺ�������       ��д��ʱ��ʵ��
    public abstract void Act(GameObject npc);//��ǰ״̬�Ķ���
    public abstract void Reason(GameObject npc);//��ǰ״̬���������
}
