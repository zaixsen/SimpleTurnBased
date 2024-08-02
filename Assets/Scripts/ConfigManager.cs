using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum RoleType
{
    Role, Pet
}
public class RoleData
{
    public int id;
    public string name;
    public int Atk;
    public int HP;
    public RoleType roleType;
    public string modelPrefab;
    public float x;
    public float y;
    public float z;
}

public class SkillData
{
    public int id;
    public string name;
    public string skillPath;
    public string skillIcon;
}

public class ConfigManager : Singleton<ConfigManager>
{
    public List<RoleData> roleDatas;
    public List<SkillData> skillDatas;

    public ConfigManager()
    {
        try
        {
            string str = Resources.Load<TextAsset>("roleData").text;
            roleDatas = JsonConvert.DeserializeObject<List<RoleData>>(str);
            str = Resources.Load<TextAsset>("skillData").text;
            skillDatas = JsonConvert.DeserializeObject<List<SkillData>>(str);
        }
        catch (System.Exception)
        {
            Debug.LogError("File Error");
        }
    }


}
