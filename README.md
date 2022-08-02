# SkillEditor
技能编辑器+种怪+基于ecs实现的背包+object基类及其子类的综合案例
 1、技能编辑部分：SkillEditorWindow、SkillWindow、Skill_Anim、Skill_Audio、Skill_Effects、SkillBase、Player 
 2、种怪部分：MonsterEditor、MonsterCfg 3、bject基类及其子类部分：种类基类（ObjectBase）存储GameObject、玩家基类（PlayerObj）{1、HostPlayer自己控制的玩家2、OtherPlayer其他玩家}、
 NPC种类（NPCObj）、 MonsterObj{1、Normal 2、Gather 3、Follow}、基础数据类：ObjectInfo
 4、GameInit.cs是入口 
 5、UI管理和fsm在SunSystem文件夹下 
 6、圆形范围检测CircleCollision.cs、StaticCircleCheck.cs
