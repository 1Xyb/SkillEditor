using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    public List<ETCButton> Attack;
    public ETCJoystick Joystick;
    public GameObject uiRoot;
    public GameObject[] DontDestory;


    private void Start()
    {
        for (int i = 0; i < DontDestory.Length; i++)
        {
            GameObject.DontDestroyOnLoad(DontDestory[i]);
        }
        GameSceneUtils.LoadSceneAsync("Lobby", () =>
        {
            JoyStickMgr.Instance.m_joyGO = DontDestory[0];
            JoyStickMgr.Instance.m_joystick = Joystick;
            JoyStickMgr.Instance.m_skillBtn = Attack;

            //�������ݽ���                                Ĭ�Ͻ�ɫ
            GameData.Instance.InitByRoleName("Teddy");
            //�������ñ���� C��
            GameData.Instance.InitTaskData();

            World.Instance.Init();
        });
    }
}
