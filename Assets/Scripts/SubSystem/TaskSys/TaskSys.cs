using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskSys : UIbase
{
    private Text taskText;
    private Button acceptBtn;
    public override void DoCreate(string path)
    {
        base.DoCreate(path);

    }
    public override void DoShow(bool active)
    {
        base.DoShow(active);
        taskText = m_go.transform.Find("TaskText").GetComponent<Text>();
        acceptBtn = m_go.transform.Find("AcceptButton").GetComponent<Button>();

        taskText.text = GameData.Instance.GetTaskDataById(1).taskName;//任务名字

        //点击接受按钮
        acceptBtn.onClick.AddListener(() =>
        {
            //交互服务器
            Notification notify = new Notification();
            notify.Refresh("AcceptTask", 1);//任务id
            MsgCenter.Instance.SendMsg("ServerMsg", notify);
        });

    }
}
