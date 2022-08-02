using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ҡ��
/// </summary>
public class JoyStickMgr : SingleTon<JoyStickMgr>
{
    public ETCJoystick m_joystick;
    public List<ETCButton> m_skillBtn;//���ܰ�ť
    internal GameObject m_joyGO;
    HostPlayer m_target;

    public bool JoyActive
    {
        set
        {
            if (m_joyGO.activeSelf != value)
            {
                m_joyGO.SetActive(value);
            }
        }
    }

    public void SetJoyArg(Camera camera,HostPlayer target)
    {
        m_target = target;
        m_joystick.cameraLookAt = target.m_go.transform;
        m_joystick.cameraTransform = camera.transform;
        SetJoytick();
    }

    private void SetJoytick()
    {
        if (m_joystick && m_target.m_go)
        {
            m_joystick.OnPressLeft.AddListener(() => m_target.JoystickHandlerMoving(m_joystick.axisX.axisValue, m_joystick.axisY.axisValue));
            m_joystick.OnPressRight.AddListener(() => m_target.JoystickHandlerMoving(m_joystick.axisX.axisValue, m_joystick.axisY.axisValue));
            m_joystick.OnPressUp.AddListener(() => m_target.JoystickHandlerMoving(m_joystick.axisX.axisValue, m_joystick.axisY.axisValue));
            m_joystick.OnPressDown.AddListener(() => m_target.JoystickHandlerMoving(m_joystick.axisX.axisValue, m_joystick.axisY.axisValue));
        }

        //TODO ==���ܰ�ť����==>����m_target.UseSkill
        if (m_skillBtn.Count != 0 && m_target.m_go)
        {
            foreach (var item in m_skillBtn)
            {
                item.onPressed.AddListener(() => m_target.JoyButtonHandler(item.name));
            }
        }
    }
}
