using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Anim : SkillBase
{
    Player player;

    Animator anim;

    public AnimationClip animClip;
    AnimatorOverrideController controller;
    //构造    
    public Skill_Anim(Player _player)
    {
        player = _player;
        anim = player.gameObject.GetComponent<Animator>();
        controller = player.overrideController;
    }

    public override void Init()
    {
        controller["Attack1"] = animClip;
    }

    /// <summary>
    /// 设置动画片段
    /// </summary>
    /// <param name="_animClip">选中的片段</param>
    public void SetAnimClip(AnimationClip _animClip)
    {
        animClip = _animClip;
        name = animClip.name;//设置名字
        controller["Attack1"] = animClip;//动画片段更新
    }

    public override void Play()
    {
        base.Play();
        starttime = Time.time;
        isBegin = true;
    }

    public void Begin()
    {
        //StopPlayback  停止动画器播放模式，停止播放后，化身恢复为游戏逻辑控制
        anim.StopPlayback();
        //AnimatorStateInfo 动画状态信息
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //name 是否匹配状态机中激活状态的名称？
        if (stateInfo.IsName("Idle1"))
        {
            //触发播放
            anim.SetTrigger("Play");
        }

    }
    /// <summary>
    /// 计算延迟时间
    /// </summary>
    /// <param name="times"></param>
    public override void Update(float times)
    {
        if ( isBegin && (times - starttime) > float.Parse(trigger))
        {
            isBegin = false;
            Begin();
        }
    }

    public override void Stop()
    {
        base.Stop();
        //将动画器设置为播放模式
        anim.StartPlayback();
    }
}
