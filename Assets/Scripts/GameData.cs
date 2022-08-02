using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameData : SingleTon<GameData>
{
    //该角色的所有技能
    public Dictionary<string, List<SkillJson>> AllRoleSkillList = new Dictionary<string, List<SkillJson>>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="roleName">角色名</param>
    public void InitByRoleName(string roleName)
    {
        //如果该角色技能。JSON文件存在
        if (File.Exists("Assets/Config/"+roleName+".json"))
        {
            //解析
            string str = File.ReadAllText("Assets/Config/" + roleName + ".json");
            List<SkillJson> skills = JsonConvert.DeserializeObject<List<SkillJson>>(str);
            //添加到集合
            AllRoleSkillList.Add(roleName, skills);
        }
    }
    /// <summary>
    /// 获取技能
    /// </summary>
    /// <param name="roleName">角色名字</param>
    /// <returns></returns>
    public List<SkillJson> GetSkillsByRoleName(string roleName)
    {
        if (AllRoleSkillList.ContainsKey(roleName))
        {
            return AllRoleSkillList[roleName];
        }
        return null;
    }

    //                  任务id        任务内容数据
    public Dictionary<int, TaskData> AllTaskDic = new Dictionary<int, TaskData>();
    /// <summary>
    /// 初始化任务
    /// </summary>
    public void InitTaskData()
    {
        TaskData task = new TaskData();
        task.taskId = 1;
        task.taskName = "任务1";
        AllTaskDic.Add(task.taskId, task);
    }

    /// <summary>
    /// 通过任务id获取任务数据
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
