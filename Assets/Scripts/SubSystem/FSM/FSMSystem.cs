using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ״̬��ϵͳ
/// </summary>
public class FSMSystem
{
    /// <summary>
    /// ��
    /// </summary>
    private Dictionary<StateID, FSMState> states = new Dictionary<StateID, FSMState>();

    private StateID currentStateID;
    private FSMState currentState;

    public void Update(GameObject npc)
    {
        currentState.Act(npc);//�滻currentStateʵ�ֶ����л�
        currentState.Reason(npc);
    }

    /// <summary>
    /// ��
    /// </summary>
    /// <param name="s"></param>
    public void AddState(FSMState s)
    {
        if (s == null)
        {
            Debug.Log("FSMState����Ϊ��");return;
        }
        if (currentState == null)
        {
            currentState = s;//Ĭ��״̬
            currentStateID = s.ID;
        }
        if (states.ContainsKey(s.ID))
        {
            Debug.LogError("״̬" + s.ID + "�Ѿ����ڣ��޷��ظ����"); return;
        }
        states.Add(s.ID, s);
    }

    /// <summary>
    /// ɾ
    /// </summary>
    /// <param name="id"></param>
    public void DeleteState(StateID id)
    {
        if (id == StateID.NullStateID)
        {
            Debug.LogError("�޷�ɾ����״̬"); return;
        }
        if (states.ContainsKey(id) == false)
        {
            Debug.LogError("�޷�ɾ�������ڵ�״̬��" + id); return;
        }
        states.Remove(id);
    }

    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <param name="trans">����</param>
    public void PerformTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("�޷�ִ�пյ�ת������"); return;
        }
        StateID id = currentState.GetOutputState(trans);//�õ�״̬ID
        if (id == StateID.NullStateID)
        {
            Debug.LogWarning("��ǰ״̬" + currentStateID + "�޷�����ת������" + trans + "����ת��"); return;
        }
        if (states.ContainsKey(id) == false)
        {
            Debug.LogError("��״̬�����治����״̬" + id + "���޷�����״̬ת����"); return;
        }
        FSMState state = states[id];
        currentState.DoAfterLeaving();//��ҡ
        currentState = state;//�滻״̬
        currentStateID = id;
        currentState.DoBeforeEntering();//ǰҡ
    }
}
