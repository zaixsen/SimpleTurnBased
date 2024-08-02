
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
    /// �ͷż���    
    /// </summary>
    /// <param name="releaseRole">�ͷż��ܵĽ�ɫ</param>
    /// <param name="targetRoles">Ŀ�꼯��</param>
    /// <param name="skillData">��������</param>
    /// <param name="action">���ܽ����ص��¼�</param>
    public void ReleaseSkill(Role releaseRole, List<Role> targetRoles, SkillData skillData, Action<PlayableDirector> action = null)
    {
        //���һ��Ŀ�� �з�����Ĭ��Ϊ��ǰ��Ѫ�����ٵĳ�����ɫ
        Role targetRole = targetRoles[UnityEngine.Random.Range(0, targetRoles.Count)];
        //����
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
        //�󶨵�һ������
        TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
        TrackAsset aniTrack = timelineAsset.GetOutputTrack(0);
        playableDirector.SetGenericBinding(aniTrack, releaseRole.GetComponent<Animator>());

        if (skillData.id == 1)
        {
            //���� �������ɶ���
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

        Debug.Log(releaseRole.name + "��" + targetRole.name + "�ͷ�" + skillData.name);
    }

    private void PlayableDirector_stopped(PlayableDirector obj)
    {
        role.transform.position = startPos;

        for (int i = TargetRoles.Count - 1; i >= 0; i--)
        {
            if (TargetRoles[i].Hp <= 0)
            {
                Debug.Log(TargetRoles[i].name + "����");
                DestroyImmediate(TargetRoles[i].gameObject);
                GameManager.Instance.RemoveRole(TargetRoles[i]);
            }
        }
        TargetRoles.Clear();

        DestroyImmediate(skill);
    }
}
