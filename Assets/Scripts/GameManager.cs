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

    float countDownTimer;  //�غϿ�ʼ��ʱ
    float countDownTimerMax;  //��ʱ���ֵ
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
    /// ��ʼ�غ�
    /// </summary>
    private void BeginTrunBased()
    {
        countDownTimer = countDownTimerMax;

        //��������
        currentRole = allRoles[0];

        StartCoroutine(StartFight());

    }
    /// <summary>
    /// Э�̿�ʼս��
    /// </summary>
    /// <returns></returns>
    IEnumerator StartFight()
    {
        //ʱ�䵽�� �����չ�
        SkillData skillData = ConfigManager.Instance.skillDatas[0];
        //Ĭ���չ�    ʱ�䵽�˲ſ��Գ���
        yield return new WaitUntil(() => countDownTimer <= 0);

        //ѡ���ܸ���Ϊ����
        if (selectedSkillData != null)
        {
            skillData = selectedSkillData;
            Debug.Log("ѡ����");
        }
        Debug.Log("��ʼЭ��");
        ReleaseSkill(skillData);

        allRoles.RemoveAt(0);

        selectedSkillData = null;
    }
    /// <summary>
    /// �ͷż���
    /// </summary>
    /// <param name="skillData"></param>
    public void ReleaseSkill(SkillData skillData)
    {
        
        List<Role> targetRole = myRoles.Contains(currentRole) ? enemyRoles : myRoles;

        SkillCoreManager.Instance.ReleaseSkill(currentRole, targetRole, skillData, (playableDir) =>
        {
            bool isOver = CheckGameOver();

            if (!isOver)
                BeginTrunBased();  //Timeline ���� ������һ�غ�
        });
    }
    /// <summary>
    /// �����Ϸ�Ƿ����
    /// </summary>
    /// <returns></returns>
    public bool CheckGameOver()
    {

        if (myRoles.Count == 0)
        {
            Debug.Log("ʧ��");
            UIManager.Instance.SetGameoverText("ʧ��");
            StopCoroutine(StartFight());
            return true;
        }

        if (enemyRoles.Count == 0)
        {
            Debug.Log("ʤ��");
            UIManager.Instance.SetGameoverText("ʤ��");
            StopCoroutine(StartFight());
            return true;
        }

        //�������ҽ�����һ�غ�
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
    /// ���صз��ҷ�ģ��
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
    /// ��ɫ�����Ƴ�
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
        Debug.Log("ɾ��");

    }
    /// <summary>
    /// ����б� ˫���������
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
