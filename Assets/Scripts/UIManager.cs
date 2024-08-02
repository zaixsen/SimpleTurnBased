using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public Text countDownText;
    public Text gameoverText;
    public Button skillButtonTemp;
    public DownHpItem downHpTemp;
    public Button againButton;
    public Slider hpTemp;
    public GameObject gameoverUI;
    List<Button> skillButtons;

    private void Start()
    {
        skillButtons = new List<Button>();
        InitSkillIcon(ConfigManager.Instance.skillDatas);
        skillButtonTemp.gameObject.SetActive(false);
        againButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
            gameoverUI.gameObject.SetActive(false);
        });
    }
    public void SetGameoverText(string gameover)
    {
        gameoverUI.SetActive(true);
        gameoverText.text = gameover;
    }
    public Slider GetRoleHp()
    {
        Slider hp = Instantiate(hpTemp, hpTemp.transform.parent);
        hp.gameObject.SetActive(true);
        return hp;
    }

    public void SetHpDown(string hp, Vector3 pos)
    {
        DownHpItem down = Instantiate(downHpTemp, downHpTemp.transform.parent);
        pos = Camera.main.WorldToScreenPoint(pos);
        down.SetText(hp, pos);
    }

    public void SetCountDown(string timer)
    {
        countDownText.text = timer;
    }

    public void InitSkillIcon(List<SkillData> skillDatas)
    {
        for (int i = 0; i < skillDatas.Count; i++)
        {
            int index = i;
            Button sklBtn = Instantiate(skillButtonTemp, skillButtonTemp.transform.parent);
            sklBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.selectedSkillData = skillDatas[index];
            });
            skillButtons.Add(sklBtn);
        }
    }

}
