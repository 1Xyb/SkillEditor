using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ѳ��״̬
public class PatrolState : FSMState//ʵ�ֻ���
{
	private List<Transform> path = new List<Transform>();//Ѳ�ߵ�·����
	private int index = 0;
	private Transform playerTransform;

	/// <summary>
	/// ����	��ʱ���״̬id
	/// </summary>
	/// <param name="fsm"></param>
	public PatrolState(FSMSystem fsm) : base(fsm)
	{
		stateID = StateID.Patrol;//����״̬id
		Transform pathTransform = GameObject.Find("Path").transform;
		Transform[] children = pathTransform.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child != pathTransform)
			{
				path.Add(child);
			}
		}
		playerTransform = GameObject.Find("Player").transform;//��ȡ���
	}

	/// <summary>
	/// Ѳ�߶���
	/// </summary>
	/// <param name="npc"></param>
	public override void Act(GameObject npc)
	{
		npc.transform.LookAt(path[index].position);//����
		npc.transform.Translate(Vector3.forward * Time.deltaTime * 3);//�ƶ�
		if (Vector3.Distance(npc.transform.position, path[index].position) < 1)
		{
			index++;
			index %= path.Count;//����  ���Ѳ�ߵ������һ�������¸�ֵΪ��һ��
		}
	} 

	/// <summary>
	/// �ж�����
	/// </summary>
	/// <param name="npc"></param>
	public override void Reason(GameObject npc)
	{
		//�����������Ҿ���С��3
		if (Vector3.Distance(playerTransform.position, npc.transform.position) < 3)
		{
			fsm.PerformTransition(Transition.SeePlayer);//�л�״̬
		}
	}
}
