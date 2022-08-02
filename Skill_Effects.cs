using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Effects : SkillBase
{
    public GameObject gameClip;
    Player player;

    ParticleSystem particleSystem;//粒子系统

    GameObject obj;
    public Skill_Effects(Player _player)
    {
        player = _player;

    }
    /// <summary>
    /// 切换特效
    /// </summary>
    /// <param name="_audioClip"></param>
    public void SetGameClip(GameObject _audioClip)
    {
        gameClip = _audioClip;
        if (gameClip.GetComponent<ParticleSystem>())
        {
            obj = GameObject.Instantiate(gameClip, player.effectsparent);
            particleSystem = obj.GetComponent<ParticleSystem>();
            particleSystem.Stop();//先暂停特效
        }
        name = _audioClip.name;
    }

    public override void Play()
    {
        base.Play();
        starttime = Time.time;
        isBegin = true;

    }
    public override void Update(float times)
    {
        if (isBegin && (times - starttime) > float.Parse(trigger))
        {
            isBegin = false;
            Begin();
        }
    }

    private void Begin()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }
    public override void Init()
    {
        if (gameClip.GetComponent<ParticleSystem>())
        {
            particleSystem = obj.GetComponent<ParticleSystem>();
            particleSystem.Stop();
        }
    }
    public override void Stop()
    {
        base.Stop();
        if (particleSystem != null)
            particleSystem.Stop();
    }
}
