using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillWindow : EditorWindow
{
    Player player;
    List<SkillBase> skillComponents;

    float currSpeed = 1;//当前播放速度
    public void SetInitSkill(List<SkillBase> _skillComponents, Player _player)
    {
        player = _player;
        // player.AnimSpeed = 1;
        currSpeed = 1;
        skillComponents = _skillComponents;
        player.currSkillComponets = skillComponents;
    }

    string[] skillComponent = new string[] { "Null", "动画", "音效", "特效" };
    int skillComponentIndex = 0;
    private void OnGUI()
    {

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("播放"))
        {
            foreach (var item in skillComponents)
            {
                item.Play();
            }
        }
        if (GUILayout.Button("停止"))
        {
            foreach (var item in skillComponents)
            {
                item.Stop();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("速度");
        float speed = EditorGUILayout.Slider(currSpeed, 0, 5);
        if (speed != currSpeed)
        {
            currSpeed = speed;
            Time.timeScale = currSpeed;//设置时间缩放
        }

        GUILayout.BeginHorizontal();
        //绘制下拉框
        skillComponentIndex = EditorGUILayout.Popup(skillComponentIndex, skillComponent);
        if (GUILayout.Button("添加"))
        {
            switch (skillComponentIndex)
            {
                case 1:
                    //添加动画
                    skillComponents.Add(new Skill_Anim(player));
                    break;
                case 2:
                    //添加音效
                    skillComponents.Add(new Skill_Audio(player));
                    break;
                case 3:
                    //添加特效
                    skillComponents.Add(new Skill_Effects(player));
                    break;
            }
        }
        GUILayout.EndHorizontal();

        ScrollViewPos = GUILayout.BeginScrollView(ScrollViewPos, false, true);
        //遍历技能组件
        foreach (var item in skillComponents)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.name);
            if (GUILayout.Button("删除"))
            {
                //移除组件
                skillComponents.Remove(item);
                break;
            }
            GUILayout.EndHorizontal();
            //判断组件类型
            if (item is Skill_Anim)
            {
                //显示动画
                ShowSkill_Anim(item as Skill_Anim);
            }
            else if (item is Skill_Audio)
            {
                //显示音效
                Skill_Audio(item as Skill_Audio);
            }
            else if (item is Skill_Effects)
            {
                //显示特效
                Skill_Effects(item as Skill_Effects);
            }
            GUILayout.Space(0.5f);
        }
        GUILayout.EndScrollView();

        //保存技能按钮
        if (GUILayout.Button("保存"))
        {
            player.SaveSkill();
        }
    }
    Vector2 ScrollViewPos = new Vector2(0, 0);
    //_Anim
    void ShowSkill_Anim(Skill_Anim _Anim)
    {
        _Anim.trigger = EditorGUILayout.TextField(_Anim.trigger);//延迟时间
        //ObjectField   生成一个可接受任何对象类型的字段
        AnimationClip animClip = EditorGUILayout.ObjectField(_Anim.animClip, typeof(AnimationClip), false) as AnimationClip;
        //如果默认的和选择的片段不一致
        if (_Anim.animClip != animClip)
        {
            //设置动画片段
            _Anim.SetAnimClip(animClip);
        }
    }

    void Skill_Audio(Skill_Audio _Audio)
    {
        _Audio.trigger = EditorGUILayout.TextField(_Audio.trigger);//延迟时间
        //生成一个可接受对象类型的字段
        AudioClip audioClip = EditorGUILayout.ObjectField(_Audio.audioClip, typeof(AudioClip), false) as AudioClip;
        if (_Audio.audioClip != audioClip)
        {
            _Audio.SetAudioClip(audioClip);
        }
    }

    void Skill_Effects(Skill_Effects _Effects)
    {
        _Effects.trigger = EditorGUILayout.TextField(_Effects.trigger);//延迟时间
        //生成一个可接受对象类型的字段
        GameObject gameClip = EditorGUILayout.ObjectField(_Effects.gameClip, typeof(GameObject), false) as GameObject;
        if (_Effects.gameClip != gameClip)
        {
            _Effects.SetGameClip(gameClip);
        }
    }
}
