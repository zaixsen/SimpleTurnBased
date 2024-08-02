
using UnityEngine;
using UnityEngine.UI;

public class Role : MonoBehaviour
{
    RoleData roleData;
    public RoleType roleType;
    public float Atk;
    public float Hp;
    public bool IsMyRole;
    public Slider hpSlider;
    public void InitData(RoleData data, bool isMyRole)
    {
        IsMyRole = isMyRole;
        roleData = data;
        name = data.name;
        roleType = data.roleType;
        Atk = data.Atk;
        Hp = data.HP;
        if (IsMyRole)
        {
            transform.position = new Vector3(-data.x, data.y, data.z);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(data.x, data.y, data.z);
            transform.eulerAngles = new Vector3(0, -90, 0);
        }

        hpSlider = UIManager.Instance.GetRoleHp();
        hpSlider.maxValue = Hp;
        hpSlider.value = Hp;
    }

    public void SetHp(float hp)
    {
        Hp -= hp;
        hpSlider.value = Hp;
        if (hpSlider.value <= 0)
        {
            Destroy(hpSlider.gameObject);
        }
        UIManager.Instance.SetHpDown(hp.ToString(), transform.position);
        Debug.Log(name + "受到" + hp + "点攻击" + " 当前剩余血量:" + Hp);
    }

    private void Update()
    {
        if (hpSlider == null) return;
        hpSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
    }
}
