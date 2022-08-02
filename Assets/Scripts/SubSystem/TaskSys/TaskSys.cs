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

        taskText.text = GameData.Instance.GetTaskDataById(1).taskName;//��������

        //������ܰ�ť
        acceptBtn.onClick.AddListener(() =>
        {
            //����������
            Notification notify = new Notification();
            notify.Refresh("AcceptTask", 1);//����id
            MsgCenter.Instance.SendMsg("ServerMsg", notify);
        });

    }
}
