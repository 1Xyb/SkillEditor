using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class SkillEditorWindow : EditorWindow
{
    class PlayerEditor
    {
        public int _characterIndex = 0;
        public int _folderIndex = 0;
        public string characterName = string.Empty;
        public string folderName = string.Empty;
        public string characterFilter = string.Empty;
        public List<string> characterList = new List<string>();
        public Player player = null;
    }

    PlayerEditor m_player = new PlayerEditor();

    /// <summary>
    /// 文件名
    /// </summary>
    List<string> m_folderList = new List<string>();
    /// <summary>
    /// 预制体名字
    /// </summary>
    List<string> m_characterList = new List<string>();
    /// <summary>
    /// 按文件名存储预制体
    /// </summary>
    Dictionary<string, List<string>> m_folderPrefabs = new Dictionary<string, List<string>>();
    /// <summary>
    /// 存储  创建新的技能的名字
    /// </summary>
    string newSkillName = string.Empty;
    /// <summary>
    /// 技能详情窗口
    /// </summary>
    SkillWindow skillWindow;

    [MenuItem("Tools/技能编辑器")]
    static void Init()
    {
        //只有运行的时候才会打开
        if (Application.isPlaying)
        {
            SkillEditorWindow window = EditorWindow.GetWindow<SkillEditorWindow>("SkillEditor");
            if (window != null)
            {
                window.Show();//打开技能编辑窗口
            }
        }
    }
    private void OnEnable()
    {
        DoSearchFolder();
        DoSearchCharacter();
    }

    /// <summary>
    /// 索引所有的预制体
    /// </summary>
    private void DoSearchCharacter()
    {
        //获取该路径文件夹下所有以“.prefab”为后缀的文件
        string[] files = Directory.GetFiles(GetCharacterPath(), "*.prefab", SearchOption.AllDirectories);
        m_characterList.Clear();
        foreach (var item in files)
        {
            m_characterList.Add(Path.GetFileNameWithoutExtension(item));//去掉后缀名后的预制体名字
        }
        m_characterList.Sort();//排序
        m_characterList.Insert(0, "Null");
        m_player.characterList.AddRange(m_characterList);//从集合的最后开始添加
    }

    /// <summary>
    /// 索引所有文件夹
    /// </summary>
    private void DoSearchFolder()
    {
        m_folderList.Clear();
        m_folderList.Add("All");
        string[] folders = Directory.GetDirectories(GetCharacterPath());//获取该路径下的所有文件夹
        foreach (var item in folders)
        {
            m_folderList.Add(Path.GetFileName(item));//获取文件夹的名字
        }
    }
    string GetCharacterPath()
    {
        return Application.dataPath + "/GameData/Model";
    }
    private void OnGUI()
    {
        //绘制一个弹出窗口（下标，内容数组）文件夹
        int folderIndex = EditorGUILayout.Popup(m_player._folderIndex, m_folderList.ToArray());
        //如果不相等再开始筛选
        if (folderIndex != m_player._folderIndex)
        {
            m_player._folderIndex = folderIndex;
            m_player._characterIndex = -1;
            string folderName = m_folderList[m_player._folderIndex];//得到当前选择的文件夹名
            List<string> list;
            if (folderName.Equals("All"))
            {
                list = m_characterList;
            }
            else
            {
                //如果字典里没有此文件名的key值  就new出value值
                if(!m_folderPrefabs.TryGetValue(folderName,out list))
                {
                    list = new List<string>();
                    //获取该文件夹名下的所有预制体
                    string[] files = Directory.GetFiles(GetCharacterPath() + "/" + folderName, "*.prefab", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        list.Add(Path.GetFileNameWithoutExtension(item));//把预制体名字添加到集合
                    }
                    m_folderPrefabs[folderName] = list;//绑定值
                }
            }
            //玩家预制体集合根据选择的文件夹的不同时刻在变化，所以要清空后重新添加进去
            m_player.characterList.Clear();
            m_player.characterList.AddRange(list);
        }
        //绘制弹出窗口（）预制体
        int characterIndex = EditorGUILayout.Popup(m_player._characterIndex, m_player.characterList.ToArray());
        if (characterIndex != m_player._characterIndex)
        {
            m_player._characterIndex = characterIndex;
            if (m_player.characterName != m_player.characterList[m_player._characterIndex])
            {
                //刷新    重新赋值
                m_player.characterName = m_player.characterList[m_player._characterIndex];
                //如果角色名不为空
                if (!string.IsNullOrEmpty(m_player.characterName))
                {
                    //当前有角色
                    if (m_player.player != null)
                    {
                        //删除
                        Destroy(m_player.player.gameObject);
                    }
                    //根据角色名字    重新生成选择的角色
                    m_player.player = Player.Init(m_player.characterName);//生成玩家
                }
            }
        }
        //绘制新技能名字
        newSkillName = GUILayout.TextField(newSkillName);
        //创建技能按钮
        if (GUILayout.Button("创建新技能"))
        {
            //判断技能名字和玩家是否为空
            if (!string.IsNullOrEmpty(newSkillName) && m_player.player != null)
            {
                //拿到该技能的技能组件
                List<SkillBase> skills = m_player.player.AddNewSkill(newSkillName);
                //打开技能编辑窗口
                OpenSkillWindow(newSkillName,skills);
            }
        }

        //如果玩家不为空   显示scrollview  已拥有的技能列表
        if (m_player.player != null)
        {
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, false, true);
            foreach (var item in m_player.player.skillsList)
            {
                GUILayout.BeginHorizontal();
                //用技能的名字做按钮的名字
                if (GUILayout.Button(item.Key))
                {
                    //获取改名字技能的组件
                    List<SkillBase> skillComponents = m_player.player.GetSkill(item.Key);
                    foreach (var ite in skillComponents)
                    {
                        ite.Init();//初始化    对组件赋值
                    }
                    //打开技能编辑窗口
                    OpenSkillWindow(item.Key, item.Value);
                }
                GUILayoutOption[] option = new GUILayoutOption[]
                {
                    GUILayout.Width(60),
                    GUILayout.Height(20)
                };
                //删除技能
                if (GUILayout.Button("删除",option))
                {
                    m_player.player.RevSkill(item.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            //保存
            if (GUILayout.Button("保存"))
            {
                m_player.player.SaveSkill();
            }
        }
    }

    Vector2 scrollViewPos = new Vector2(0, 0);
    /// <summary>
    /// 打开技能编辑窗口
    /// </summary>
    /// <param name="newSkillName">新技能的名字</param>
    /// <param name="skillComponents">对应的技能组件</param>
    private void OpenSkillWindow(string newSkillName,List<SkillBase>skillComponents)
    {
        if (skillComponents != null)
        {
            if (skillWindow == null)
            {
                skillWindow = EditorWindow.GetWindow<SkillWindow>("");
            }
            //设置窗口的名字
            skillWindow.titleContent = new GUIContent(newSkillName);
            //技能组件  玩家
            skillWindow.SetInitSkill(skillComponents, m_player.player);
            //展示窗口
            skillWindow.Show();
            //刷新
            skillWindow.Repaint();
        }
    }
}
