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
    /// �ļ���
    /// </summary>
    List<string> m_folderList = new List<string>();
    /// <summary>
    /// Ԥ��������
    /// </summary>
    List<string> m_characterList = new List<string>();
    /// <summary>
    /// ���ļ����洢Ԥ����
    /// </summary>
    Dictionary<string, List<string>> m_folderPrefabs = new Dictionary<string, List<string>>();
    /// <summary>
    /// �洢  �����µļ��ܵ�����
    /// </summary>
    string newSkillName = string.Empty;
    /// <summary>
    /// �������鴰��
    /// </summary>
    SkillWindow skillWindow;

    [MenuItem("Tools/���ܱ༭��")]
    static void Init()
    {
        //ֻ�����е�ʱ��Ż��
        if (Application.isPlaying)
        {
            SkillEditorWindow window = EditorWindow.GetWindow<SkillEditorWindow>("SkillEditor");
            if (window != null)
            {
                window.Show();//�򿪼��ܱ༭����
            }
        }
    }
    private void OnEnable()
    {
        DoSearchFolder();
        DoSearchCharacter();
    }

    /// <summary>
    /// �������е�Ԥ����
    /// </summary>
    private void DoSearchCharacter()
    {
        //��ȡ��·���ļ����������ԡ�.prefab��Ϊ��׺���ļ�
        string[] files = Directory.GetFiles(GetCharacterPath(), "*.prefab", SearchOption.AllDirectories);
        m_characterList.Clear();
        foreach (var item in files)
        {
            m_characterList.Add(Path.GetFileNameWithoutExtension(item));//ȥ����׺�����Ԥ��������
        }
        m_characterList.Sort();//����
        m_characterList.Insert(0, "Null");
        m_player.characterList.AddRange(m_characterList);//�Ӽ��ϵ����ʼ���
    }

    /// <summary>
    /// ���������ļ���
    /// </summary>
    private void DoSearchFolder()
    {
        m_folderList.Clear();
        m_folderList.Add("All");
        string[] folders = Directory.GetDirectories(GetCharacterPath());//��ȡ��·���µ������ļ���
        foreach (var item in folders)
        {
            m_folderList.Add(Path.GetFileName(item));//��ȡ�ļ��е�����
        }
    }
    string GetCharacterPath()
    {
        return Application.dataPath + "/GameData/Model";
    }
    private void OnGUI()
    {
        //����һ���������ڣ��±꣬�������飩�ļ���
        int folderIndex = EditorGUILayout.Popup(m_player._folderIndex, m_folderList.ToArray());
        //���������ٿ�ʼɸѡ
        if (folderIndex != m_player._folderIndex)
        {
            m_player._folderIndex = folderIndex;
            m_player._characterIndex = -1;
            string folderName = m_folderList[m_player._folderIndex];//�õ���ǰѡ����ļ�����
            List<string> list;
            if (folderName.Equals("All"))
            {
                list = m_characterList;
            }
            else
            {
                //����ֵ���û�д��ļ�����keyֵ  ��new��valueֵ
                if(!m_folderPrefabs.TryGetValue(folderName,out list))
                {
                    list = new List<string>();
                    //��ȡ���ļ������µ�����Ԥ����
                    string[] files = Directory.GetFiles(GetCharacterPath() + "/" + folderName, "*.prefab", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        list.Add(Path.GetFileNameWithoutExtension(item));//��Ԥ����������ӵ�����
                    }
                    m_folderPrefabs[folderName] = list;//��ֵ
                }
            }
            //���Ԥ���弯�ϸ���ѡ����ļ��еĲ�ͬʱ���ڱ仯������Ҫ��պ�������ӽ�ȥ
            m_player.characterList.Clear();
            m_player.characterList.AddRange(list);
        }
        //���Ƶ������ڣ���Ԥ����
        int characterIndex = EditorGUILayout.Popup(m_player._characterIndex, m_player.characterList.ToArray());
        if (characterIndex != m_player._characterIndex)
        {
            m_player._characterIndex = characterIndex;
            if (m_player.characterName != m_player.characterList[m_player._characterIndex])
            {
                //ˢ��    ���¸�ֵ
                m_player.characterName = m_player.characterList[m_player._characterIndex];
                //�����ɫ����Ϊ��
                if (!string.IsNullOrEmpty(m_player.characterName))
                {
                    //��ǰ�н�ɫ
                    if (m_player.player != null)
                    {
                        //ɾ��
                        Destroy(m_player.player.gameObject);
                    }
                    //���ݽ�ɫ����    ��������ѡ��Ľ�ɫ
                    m_player.player = Player.Init(m_player.characterName);//�������
                }
            }
        }
        //�����¼�������
        newSkillName = GUILayout.TextField(newSkillName);
        //�������ܰ�ť
        if (GUILayout.Button("�����¼���"))
        {
            //�жϼ������ֺ�����Ƿ�Ϊ��
            if (!string.IsNullOrEmpty(newSkillName) && m_player.player != null)
            {
                //�õ��ü��ܵļ������
                List<SkillBase> skills = m_player.player.AddNewSkill(newSkillName);
                //�򿪼��ܱ༭����
                OpenSkillWindow(newSkillName,skills);
            }
        }

        //�����Ҳ�Ϊ��   ��ʾscrollview  ��ӵ�еļ����б�
        if (m_player.player != null)
        {
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, false, true);
            foreach (var item in m_player.player.skillsList)
            {
                GUILayout.BeginHorizontal();
                //�ü��ܵ���������ť������
                if (GUILayout.Button(item.Key))
                {
                    //��ȡ�����ּ��ܵ����
                    List<SkillBase> skillComponents = m_player.player.GetSkill(item.Key);
                    foreach (var ite in skillComponents)
                    {
                        ite.Init();//��ʼ��    �������ֵ
                    }
                    //�򿪼��ܱ༭����
                    OpenSkillWindow(item.Key, item.Value);
                }
                GUILayoutOption[] option = new GUILayoutOption[]
                {
                    GUILayout.Width(60),
                    GUILayout.Height(20)
                };
                //ɾ������
                if (GUILayout.Button("ɾ��",option))
                {
                    m_player.player.RevSkill(item.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            //����
            if (GUILayout.Button("����"))
            {
                m_player.player.SaveSkill();
            }
        }
    }

    Vector2 scrollViewPos = new Vector2(0, 0);
    /// <summary>
    /// �򿪼��ܱ༭����
    /// </summary>
    /// <param name="newSkillName">�¼��ܵ�����</param>
    /// <param name="skillComponents">��Ӧ�ļ������</param>
    private void OpenSkillWindow(string newSkillName,List<SkillBase>skillComponents)
    {
        if (skillComponents != null)
        {
            if (skillWindow == null)
            {
                skillWindow = EditorWindow.GetWindow<SkillWindow>("");
            }
            //���ô��ڵ�����
            skillWindow.titleContent = new GUIContent(newSkillName);
            //�������  ���
            skillWindow.SetInitSkill(skillComponents, m_player.player);
            //չʾ����
            skillWindow.Show();
            //ˢ��
            skillWindow.Repaint();
        }
    }
}
