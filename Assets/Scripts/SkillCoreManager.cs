
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SkillCoreManager : MonoSingleton<SkillCoreManager>
{
    Vector3 startPos;
    Role role;
    GameObject skill;
    List<Role> TargetRoles;
    private void Start()
    {
        TargetRoles = new List<Role>();
    }
    /// <summary>
    /// 释放技能    
    /// </summary>
    /// <param name="releaseRole">释放技能的角色</param>
    /// <param name="targetRoles">目标集合</param>
    /// <param name="skillData">技能数据</param>
    /// <param name="action">技能结束回调事件</param>
    public void ReleaseSkill(Role releaseRole, List<Role> targetRoles, SkillData skillData, Action<PlayableDirector> action = null)
    {
        //随机一个目标 敌方攻击默认为最前排血量最少的宠物或角色
        Role targetRole = targetRoles[UnityEngine.Random.Range(0, targetRoles.Count)];
        //排序
        targetRoles.Sort((x, y) =>
        {
            float dis1 = Vector3.Distance(releaseRole.transform.localPosition, x.transform.localPosition);
            float dis2 = Vector3.Distance(releaseRole.transform.localPosition, y.transform.localPosition);
            if (dis1 > dis2)
            {
                return 1;
            }
            else if (dis1 < dis2)
            {
                return -1;
            }
            else
            {
                if (x.Hp > y.Hp)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        });

        startPos = releaseRole.transform.position;
        skill = Instantiate(Resources.Load<GameObject>(skillData.skillPath));
        role = releaseRole;
        PlayableDirector playableDirector = skill.GetComponent<PlayableDirector>();
        //绑定第一个动画
        TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
        TrackAsset aniTrack = timelineAsset.GetOutputTrack(0);
        playableDirector.SetGenericBinding(aniTrack, releaseRole.GetComponent<Animator>());

        if (skillData.id == 1)
        {
            //概率 暴击击飞动画
            int trackIndex = UnityEngine.Random.Range(0, 10) < 2 ? 1 : 2;

            TrackAsset harmTrack = timelineAsset.GetOutputTrack(trackIndex);

            playableDirector.SetGenericBinding(harmTrack, targetRole.GetComponent<Animator>());
            releaseRole.transform.position = targetRole.transform.localPosition + targetRole.transform.forward;
            if (trackIndex == 2)
            {
                targetRole.SetHp(9999);
                TargetRoles.Add(targetRole);
            }
            else
            {
                targetRole.SetHp(releaseRole.Atk);
            }
        }
        else if (skillData.id == 2)
        {
            for (int i = 0; i < targetRoles.Count; i++)
            {
                TrackAsset harmTrack = timelineAsset.GetOutputTrack(i + 1);
                playableDirector.SetGenericBinding(harmTrack, targetRoles[i].GetComponent<Animator>());
                targetRoles[i].SetHp(9999);
            }

            TargetRoles = targetRoles;
        }

        playableDirector.stopped += PlayableDirector_stopped;

        if (action != null)
        {
            playableDirector.stopped += action;
        }

        Debug.Log(releaseRole.name + "对" + targetRole.name + "释放" + skillData.name);
    }

    private void PlayableDirector_stopped(PlayableDirector obj)
    {
        role.transform.position = startPos;

        for (int i = TargetRoles.Count - 1; i >= 0; i--)
        {
            if (TargetRoles[i].Hp <= 0)
            {
                Debug.Log(TargetRoles[i].name + "死亡");
                DestroyImmediate(TargetRoles[i].gameObject);
                GameManager.Instance.RemoveRole(TargetRoles[i]);
            }
        }
        TargetRoles.Clear();

        DestroyImmediate(skill);
    }
}
