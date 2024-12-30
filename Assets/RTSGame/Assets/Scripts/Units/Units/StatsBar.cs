using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar
{
    GameObject prefab; //Canvas
    Image image;

    UnitStats unitStats;
    Camera camera;

    int curHp { get { return unitStats.hp; } set { } }
    int maxHp { get { return unitStats.maxHp; } set { } }

    public StatsBar(UnitStats us)
    {
        unitStats = us;
        curHp = us.hp;
        maxHp = us.maxHp;
        prefab = us.prefab;
        image = us.image;

        camera = Camera.main;
        SetColor();

        float timeMax = 1;
        float timeCurrent = timeMax;
    }

    public void Update()
    {
        if (prefab != null)
        {
            LookAtCamera();
        }
    }

    void EnableOrDisable(bool enable)
    {
        prefab.SetActive(enable);
    }

    public void UpdateHealthBar()
    {
        if (image != null)
        {
            float fillAmount = (float)curHp / (float)maxHp;

            EnableOrDisable(true);

            image.fillAmount = fillAmount;
        }

    }

    void LookAtCamera()
    {
        prefab.transform.rotation = Quaternion.LookRotation(prefab.transform.position - camera.transform.position);
    }

    void SetColor()
    {
        if (image != null)
        {
            image.color = unitStats.teamColor;
        }
    }

    public void UpateText(string textos)
    {
        TextMeshProUGUI text = prefab.GetComponentInChildren<TextMeshProUGUI>();
        if (!ReferenceEquals(text, null))
        {
            text.text = textos;
        }
    }

}
