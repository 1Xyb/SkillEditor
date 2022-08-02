using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//׷��״̬
public class ChaseState : FSMState//�̳л���
{
	private Transform playerTransform;//���
	public ChaseState(FSMSystem fsm) : base(fsm)
	{
		stateID = StateID.Chase;//��ֵ״̬ID
		playerTransform = GameObject.Find("Player").transform;
	}

    /// <summary>
    /// ׷������
    /// </summary>
    /// <param name="npc"></param>
    public override void Act(GameObject npc)
    {
        npc.transform.LookAt(playerTransform.position);//���˿������
        npc.transform.Translate(Vector3.forward * 2 * Time.deltaTime);//�����ƶ�
    }

    /// <summary>
    /// �����л�
    /// </summary>
    /// <param name="npc"></param>
    public override void Reason(GameObject npc)
    {
        //׷��ʱ����������6�����л�״̬
        if (Vector3.Distance(playerTransform.position, npc.transform.position) > 6)
        {
            fsm.PerformTransition(Transition.LostPlayer);//�л�Ѳ��״̬
        }
    }
}
