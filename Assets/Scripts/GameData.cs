using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameData : SingleTon<GameData>
{
    //�ý�ɫ�����м���
    public Dictionary<string, List<SkillJson>> AllRoleSkillList = new Dictionary<string, List<SkillJson>>();

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="roleName">��ɫ��</param>
    public void InitByRoleName(string roleName)
    {
        //����ý�ɫ���ܡ�JSON�ļ�����
        if (File.Exists("Assets/Config/"+roleName+".json"))
        {
            //����
            string str = File.ReadAllText("Assets/Config/" + roleName + ".json");
            List<SkillJson> skills = JsonConvert.DeserializeObject<List<SkillJson>>(str);
            //��ӵ�����
            AllRoleSkillList.Add(roleName, skills);
        }
    }
    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <param name="roleName">��ɫ����</param>
    /// <returns></returns>
    public List<SkillJson> GetSkillsByRoleName(string roleName)
    {
        if (AllRoleSkillList.ContainsKey(roleName))
        {
            return AllRoleSkillList[roleName];
        }
        return null;
    }

    //                  ����id        ������������
    public Dictionary<int, TaskData> AllTaskDic = new Dictionary<int, TaskData>();
    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void InitTaskData()
    {
        TaskData task = new TaskData();
        task.taskId = 1;
        task.taskName = "����1";
        AllTaskDic.Add(task.taskId, task);
    }

    /// <summary>
    /// ͨ������id��ȡ��������
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public TaskData GetTaskDataById(int taskId)
    {
        if (AllTaskDic.ContainsKey(taskId))
        {
            return AllTaskDic[taskId];
        }
        return null;
    }
}

public class TaskData
{
    public int taskId;
    public string taskName;
    //public List<TaskAction> taskActions;
}
