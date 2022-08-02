using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物状态类
/// </summary>
public class Enemy : MonoBehaviour
{
    //每个怪物都有一个状态机系统 实现怪物状态的定制
    private FSMSystem fsm;

    void Start()
    {
        InitFSM();
    }

    private void InitFSM()
    {
        fsm = new FSMSystem();

        //实例化两个状态
        //巡逻状态
        FSMState patrolState = new PatrolState(fsm);
        patrolState.AddTransition(Transition.SeePlayer, StateID.Chase);//看到玩家，切换到追击状态
        //追赶状态
        FSMState chaseState = new ChaseState(fsm);
        chaseState.AddTransition(Transition.LostPlayer, StateID.Patrol);//添加条件、ID

        fsm.AddState(patrolState);//添加状态
        fsm.AddState(chaseState);
    }

    void Update()
    {
        fsm.Update(this.gameObject);
    }
}
