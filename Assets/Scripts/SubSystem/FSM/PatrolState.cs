using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//巡逻状态
public class PatrolState : FSMState//实现基类
{
	private List<Transform> path = new List<Transform>();//巡逻的路径点
	private int index = 0;
	private Transform playerTransform;

	/// <summary>
	/// 构造	的时候给状态id
	/// </summary>
	/// <param name="fsm"></param>
	public PatrolState(FSMSystem fsm) : base(fsm)
	{
		stateID = StateID.Patrol;//给定状态id
		Transform pathTransform = GameObject.Find("Path").transform;
		Transform[] children = pathTransform.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child != pathTransform)
			{
				path.Add(child);
			}
		}
		playerTransform = GameObject.Find("Player").transform;//获取玩家
	}

	/// <summary>
	/// 巡逻动作
	/// </summary>
	/// <param name="npc"></param>
	public override void Act(GameObject npc)
	{
		npc.transform.LookAt(path[index].position);//看向
		npc.transform.Translate(Vector3.forward * Time.deltaTime * 3);//移动
		if (Vector3.Distance(npc.transform.position, path[index].position) < 1)
		{
			index++;
			index %= path.Count;//求余  如果巡逻点事最后一个则重新赋值为第一个
		}
	} 

	/// <summary>
	/// 判断条件
	/// </summary>
	/// <param name="npc"></param>
	public override void Reason(GameObject npc)
	{
		//如果敌人与玩家距离小于3
		if (Vector3.Distance(playerTransform.position, npc.transform.position) < 3)
		{
			fsm.PerformTransition(Transition.SeePlayer);//切换状态
		}
	}
}
