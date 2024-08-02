using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownHpItem : MonoBehaviour
{
    public Text hpText;

    public void SetText(string hp, Vector3 pos)
    {
        hpText.text = hp;
        transform.position = pos;
        gameObject.SetActive(true);
        Destroy(hpText, 1);
    }

    private void Update()
    {
        transform.position -= Vector3.down * Time.deltaTime * 60;
    }
}
