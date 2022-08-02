using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����״̬��
/// </summary>
public class Enemy : MonoBehaviour
{
    //ÿ�����ﶼ��һ��״̬��ϵͳ ʵ�ֹ���״̬�Ķ���
    private FSMSystem fsm;

    void Start()
    {
        InitFSM();
    }

    private void InitFSM()
    {
        fsm = new FSMSystem();

        //ʵ��������״̬
        //Ѳ��״̬
        FSMState patrolState = new PatrolState(fsm);
        patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);//������ң��л���׷��״̬
        //׷��״̬
        FSMState chaseState = new ChaseState(fsm);
        chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);//���������ID

        fsm.AddState(patrolState);//���״̬
        fsm.AddState(chaseState);
    }

    void Update()
    {
        fsm.Update(this.gameObject);
    }
}
