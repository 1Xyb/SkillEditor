using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//追击状态
public class ChaseState : FSMState//继承基类
{
	private Transform playerTransform;//玩家
	public ChaseState(FSMSystem fsm) : base(fsm)
	{
		stateID = StateID.Chase;//赋值状态ID
		playerTransform = GameObject.Find("Player").transform;
	}

    /// <summary>
    /// 追击动作
    /// </summary>
    /// <param name="npc"></param>
    public override void Act(GameObject npc)
    {
        npc.transform.LookAt(playerTransform.position);//敌人看向玩家
        npc.transform.Translate(Vector3.forward * 2 * Time.deltaTime);//敌人移动
    }

    /// <summary>
    /// 条件切换
    /// </summary>
    /// <param name="npc"></param>
    public override void Reason(GameObject npc)
    {
        //追赶时如果距离大于6，则切换状态
        if (Vector3.Distance(playerTransform.position, npc.transform.position) > 6)
        {
            fsm.PerformTransition(Transition.LostPlayer);//切换巡逻状态
        }
    }
}
