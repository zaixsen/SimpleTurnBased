using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    List<RoleData> roles;

    public Transform roleParent;
    public SkillData selectedSkillData;

    List<Role> allRoles;
    List<Role> enemyRoles;
    List<Role> myRoles;

    float countDownTimer;  //回合开始计时
    float countDownTimerMax;  //计时最大值
    Role currentRole;

    private void Start()
    {
        roles = ConfigManager.Instance.roleDatas;
        allRoles = new List<Role>();
        enemyRoles = new List<Role>();
        myRoles = new List<Role>();
        InitData();
        countDownTimer = 0;
        countDownTimerMax = 3;
        FillAllList();
        BeginTrunBased();
    }

    private void Update()
    {
        if (countDownTimer <= 0)
        {
            UIManager.Instance.SetCountDown("");
            return;
        }
        countDownTimer -= Time.deltaTime;
        UIManager.Instance.SetCountDown(Mathf.CeilToInt(countDownTimer).ToString());

    }

    /// <summary>
    /// 开始回合
    /// </summary>
    private void BeginTrunBased()
    {
        countDownTimer = countDownTimerMax;

        //攻击的人
        currentRole = allRoles[0];

        StartCoroutine(StartFight());

    }
    /// <summary>
    /// 协程开始战斗
    /// </summary>
    /// <returns></returns>
    IEnumerator StartFight()
    {
        //时间到了 就是普攻
        SkillData skillData = ConfigManager.Instance.skillDatas[0];
        //默认普攻    时间到了才可以出手
        yield return new WaitUntil(() => countDownTimer <= 0);

        //选择技能更新为技能
        if (selectedSkillData != null)
        {
            skillData = selectedSkillData;
            Debug.Log("选择技能");
        }
        Debug.Log("开始协程");
        ReleaseSkill(skillData);

        allRoles.RemoveAt(0);

        selectedSkillData = null;
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skillData"></param>
    public void ReleaseSkill(SkillData skillData)
    {
        
        List<Role> targetRole = myRoles.Contains(currentRole) ? enemyRoles : myRoles;

        SkillCoreManager.Instance.ReleaseSkill(currentRole, targetRole, skillData, (playableDir) =>
        {
            bool isOver = CheckGameOver();

            if (!isOver)
                BeginTrunBased();  //Timeline 结束 进行下一回合
        });
    }
    /// <summary>
    /// 检测游戏是否结束
    /// </summary>
    /// <returns></returns>
    public bool CheckGameOver()
    {

        if (myRoles.Count == 0)
        {
            Debug.Log("失败");
            UIManager.Instance.SetGameoverText("失败");
            StopCoroutine(StartFight());
            return true;
        }

        if (enemyRoles.Count == 0)
        {
            Debug.Log("胜利");
            UIManager.Instance.SetGameoverText("胜利");
            StopCoroutine(StartFight());
            return true;
        }

        //如果有玩家进行下一回合
        for (int i = 0; i < myRoles.Count; i++)
        {
            if (myRoles[i].roleType == RoleType.Role)
            {
                return false;
            }
        }

        for (int i = 0; i < enemyRoles.Count; i++)
        {
            if (enemyRoles[i].roleType == RoleType.Role)
            {
                return false;
            }
        }

        return false;
    }
    /// <summary>
    /// 加载敌方我方模型
    /// </summary>
    public void InitData()
    {
        for (int i = 0; i < roles.Count; i++)
        {
            var go = Instantiate(Resources.Load<GameObject>(roles[i].modelPrefab), roleParent);
            var role = go.AddComponent<Role>();
            role.InitData(roles[i], true);
            myRoles.Add(role);
        }
        for (int i = 0; i < roles.Count; i++)
        {
            var go = Instantiate(Resources.Load<GameObject>(roles[i].modelPrefab), roleParent);
            var role = go.AddComponent<Role>();
            role.InitData(roles[i], false);
            enemyRoles.Add(role);
        }
    }
    /// <summary>
    /// 角色死亡移除
    /// </summary>
    /// <param name="role"></param>
    public void RemoveRole(Role role)
    {
        if (role.IsMyRole)
        {
            myRoles.Remove(role);
        }
        else
        {
            enemyRoles.Remove(role);
        }

        for (int i = 0; i < allRoles.Count; i++)
        {
            if (allRoles[i] == null)
            {
                allRoles.RemoveAt(i);
            }
        }
        Debug.Log("删除");

    }
    /// <summary>
    /// 填充列表 双方出手完毕
    /// </summary>
    public void FillAllList()
    {
        allRoles.Clear();
        for (int i = 0; i < myRoles.Count; i++)
            allRoles.Add(myRoles[i]);

        for (int i = 0; i < enemyRoles.Count; i++)
            allRoles.Add(enemyRoles[i]);
    }
}
