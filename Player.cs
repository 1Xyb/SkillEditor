using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
//using LitJson; 
//using Newtonsoft.Json;

public class Player : MonoBehaviour
{
    /*
     * RuntimeAnimatorController:  AnimatorController 的运行时表示。使用此表示可在运行时期间更改 Animator Controller。
      */
    RuntimeAnimatorController controller;
    /*AnimatorOverrideController:    是一种资源，可以扩展现有的AnimatorController，
    替换使用的特定动画但保留其原始结构、参数和逻辑
    */
    public AnimatorOverrideController overrideController;//动画重写
    public Transform effectsparent;//挂载特效的父级
    AudioSource audioSource;
    Animator anim;

    /// <summary>
    /// 技能字典    技能的名字   技能的组件（动画、音效、特效）
    /// </summary>
    public Dictionary<string, List<SkillBase>> skillsList = new Dictionary<string, List<SkillBase>>();

    /// <summary>
    /// 玩家当前的技能组件
    /// </summary>
    public List<SkillBase> currSkillComponets = new List<SkillBase>();

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void InitData()
    {
        overrideController = new AnimatorOverrideController();
        controller = Resources.Load<RuntimeAnimatorController>("PlayerAnmi");//实例化动画控制器并赋值
        overrideController.runtimeAnimatorController = controller;
        anim.runtimeAnimatorController =overrideController;
        //声音组件
        audioSource = gameObject.AddComponent<AudioSource>();
        //找到特效挂载的节点
        effectsparent = transform.Find("effectsparent");
    }

    public void Play()
    {
        foreach (var item in currSkillComponets)
        {
            item.Play();
        }
    }

    private Skill_Anim _Anim;
    private Skill_Audio _Aduio;
    private Skill_Effects _Effect;
    /// <summary>
    /// 设置技能
    /// </summary>
    /// <param name="skillName">技能的名字</param>
    public void SetData(string skillName)
    {
        List<SkillJson> skillList = GameData.Instance.GetSkillsByRoleName("Teddy");//通过角色名字拿到技能

        foreach (var item in skillList)
        {
            if (item.name == skillName)
            {
                currSkillComponets.Clear();//清空当前技能组件
                foreach (var ite in item.skillComponents)
                {
                    foreach (var it in ite.Value)
                    {
                        if (ite.Key.Equals("动画"))
                        {
                            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/GameData/Anim/" + it.componentName + ".anim");
                            if (_Anim == null) _Anim = new Skill_Anim(this);
                            _Anim.SetAnimClip(clip);
                            _Anim.SetTrigger(it.trigger);
                            currSkillComponets.Add(_Anim);//重新添加组件
                        }
                        else if (ite.Key.Equals("音效"))
                        {
                            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/GameData/Audio/" + it.componentName + ".mp3");
                            if (_Aduio == null) _Aduio = new Skill_Audio(this);
                            _Aduio.SetAudioClip(clip);
                            _Aduio.SetTrigger(it.trigger);
                            currSkillComponets.Add(_Aduio);
                        }
                        else if (ite.Key.Equals("特效"))
                        {
                            GameObject clip = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameData/Effect/Skill/" + it.componentName + ".prefab");
                            if (_Effect == null) _Effect = new Skill_Effects(this);
                            _Effect.SetGameClip(clip);
                            _Effect.SetTrigger(it.trigger);
                            currSkillComponets.Add(_Effect);
                        }
                    }
                }
            }
        }
    }

    //实例化玩家
    public static Player Init(string characterName)
    {
        if (characterName != null)
        {
            string str = characterName;
            GameObject obj = Instantiate(Resources.Load<GameObject>(str));
            if (obj != null)
            {
                Player player = obj.GetComponent<Player>();
                player.overrideController = new AnimatorOverrideController();
                player.controller = Resources.Load<RuntimeAnimatorController>("PlayerAnmi");//实例化动画控制器并赋值
                player.overrideController.runtimeAnimatorController = player.controller;
                player.anim.runtimeAnimatorController = player.overrideController;
                //声音组件
                player.audioSource = player.gameObject.AddComponent<AudioSource>();
                //找到特效挂载的节点
                player.effectsparent = player.transform.Find("effectsparent");
                player.gameObject.name = characterName;
                //加载技能
                player.LoadAllSkill();
                return player;
            }
        }
        return null;
    }

    /// <summary>
    /// 读取配置表   加载该玩家的技能配置表
    /// </summary>
    private void LoadAllSkill()
    {
        if (File.Exists(Application.dataPath + "/Config/" + gameObject.name + ".json"))
        {
            string str = File.ReadAllText(Application.dataPath+"/Config/" + gameObject.name + ".json");
            if (str != "")
            {
                List<SkillJson> skills = JsonConvert.DeserializeObject<List<SkillJson>>(str);
                foreach (var item in skills)
                {
                    skillsList.Add(item.name, new List<SkillBase>());
                    foreach (var ite in item.skillComponents)
                    {
                        foreach (var it in ite.Value)
                        {
                            if (ite.Key.Equals("动画"))
                            {
                                //加载
                                //AssetDatabase 用于访问项目里的资源
                                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/GameData/Anim/" + it.componentName + ".anim"); 
                                Skill_Anim _Anim = new Skill_Anim(this);//传player
                                _Anim.SetAnimClip(clip);//切换动画
                                _Anim.SetTrigger(it.trigger);//设置延迟
                                skillsList[item.name].Add(_Anim);//添加到集合
                            }
                            else if (ite.Key.Equals("音效"))
                            {
                                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/GameData/Audio/" + it.componentName + ".mp3");
                                Skill_Audio _Audio = new Skill_Audio(this);
                                _Audio.SetAudioClip(clip);
                                _Audio.SetTrigger(it.trigger);
                                skillsList[item.name].Add(_Audio);
                            }
                            else if (ite.Key.Equals("特效"))
                            {
                                GameObject clip = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameData/Effect/Skill/" + it.componentName + ".prefab");
                                Skill_Effects _Effects = new Skill_Effects(this);
                                _Effects.SetGameClip(clip);
                                _Effects.SetTrigger(it.trigger);
                                skillsList[item.name].Add(_Effects);
                            }
                        }
                    }
                }
            }
           
        }
    }

    /// <summary>
    /// 玩家新增技能
    /// </summary>
    /// <param name="newSkillName"></param>
    /// <returns></returns>
    public List<SkillBase> AddNewSkill(string newSkillName)
    {
        //如果技能名字已存在
        if (skillsList.ContainsKey(newSkillName))
        {
            //返回该名字的技能组件
            return skillsList[newSkillName];
        }
        //如果没有则添加到集合
        skillsList.Add(newSkillName, new List<SkillBase>());
        //返回组件
        return skillsList[newSkillName];
    }
    /// <summary>
    /// 根据技能名字  获取技能组件
    /// </summary>
    /// <param name="skillName"></param>
    /// <returns></returns>
    public List<SkillBase> GetSkill(string skillName)
    {
        if (skillsList.ContainsKey(skillName))
        {
            return skillsList[skillName];//List<skillbase>
        }
        return null;
    }
    /// <summary>
    /// 根据技能名字  删除技能
    /// </summary>
    /// <param name="newSkillName"></param>
    public void RevSkill(string newSkillName)
    {
        if (skillsList.ContainsKey(newSkillName))
        {
            skillsList.Remove(newSkillName);
        }
    }
    /// <summary>
    /// 运行Update
    /// </summary>
    private void Update()
    {
        foreach (var item in currSkillComponets)
        {
            //游戏开始到运行到现在的时间
            item.Update(Time.time);
        }
    }
    private void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 存储技能
    /// </summary>
    public void SaveSkill()
    {
        List<SkillJson> skills = new List<SkillJson>();
        foreach (var item in skillsList)
        {
            SkillJson skillJson = new SkillJson();
            skillJson.name = item.Key;//设置技能名字
            foreach (var ite in item.Value)
            {
                if(ite is Skill_Anim)
                {
                    //如果没有key“动画”
                    if (!skillJson.skillComponents.ContainsKey("动画"))
                    {
                        //添加
                        skillJson.skillComponents.Add("动画", new List<SkillComponentsData>());
                    }
                    skillJson.skillComponents["动画"].Add(new SkillComponentsData(ite.name, ite.trigger));
                }
                else if(ite is Skill_Audio)
                {
                    if (!skillJson.skillComponents.ContainsKey("音效"))
                    {
                        skillJson.skillComponents.Add("音效", new List<SkillComponentsData>());
                    }
                    skillJson.skillComponents["音效"].Add(new SkillComponentsData(ite.name, ite.trigger));
                }
                else if (ite is Skill_Effects)
                {
                    if (!skillJson.skillComponents.ContainsKey("特效"))
                    {
                        skillJson.skillComponents.Add("特效", new List<SkillComponentsData>());
                    }
                    skillJson.skillComponents["特效"].Add(new SkillComponentsData(ite.name, ite.trigger));
                }
            }
            skills.Add(skillJson);
        }
        string str = JsonConvert.SerializeObject(skills);
        File.WriteAllText(Application.dataPath + "/Config/" + gameObject.name + ".json", str);
        Debug.Log("保存成功");
    }
}

public class SkillJson
{
    public string name;//技能的名字
                       //组件的名字(是动画、音效或者特效)
    public Dictionary<string, List<SkillComponentsData>> skillComponents = new Dictionary<string, List<SkillComponentsData>>();
}

public class SkillComponentsData
{
    public string componentName;//组件的名字 (动画、音效、特效的名字)
    public string trigger;//延迟时间
    public SkillComponentsData(string cn, string tri)
    {
        componentName = cn;
        trigger = tri;
    }
}
