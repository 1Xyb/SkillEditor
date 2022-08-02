using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager:Singleton<MessageManager>
{
    Dictionary<int, Action<object>> msgdic = new Dictionary<int, Action<object>>();//����
    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="id">��Ϣ��</param>
    /// <param name="action">�����¼�</param>
    public void OnAddListen(int id, Action<object> action)
    {
        if (msgdic.ContainsKey(id))
        {
            msgdic[id] += action;
        }
        else
        {
            msgdic.Add(id, action);
        }
    }
    /// <summary>
    /// �Ƴ���Ϣ
    /// </summary>
    /// <param name="id"></param>
    /// <param name="action"></param>
    public void OnRemoveListen(int id, Action<object> action)
    {
        if (msgdic.ContainsKey(id))
        {
            msgdic[id] -= action;
            if (msgdic[id] == null)
            {
                msgdic.Remove(id);
            }
        }
    }

    public void OnDisPatch(int id,params object[] arr)
    {
        if (msgdic.ContainsKey(id))
        {
            msgdic[id](arr);
        }
        else
        {
            Debug.Log("��Ϣ" + id + "δע��");
        }
    }
}
