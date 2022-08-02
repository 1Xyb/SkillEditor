using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager:Singleton<MessageManager>
{
    Dictionary<int, Action<object>> msgdic = new Dictionary<int, Action<object>>();//容器
    /// <summary>
    /// 监听消息
    /// </summary>
    /// <param name="id">消息号</param>
    /// <param name="action">具体事件</param>
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
    /// 移除消息
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
            Debug.Log("消息" + id + "未注册");
        }
    }
}
