using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class UiOptions : MonoBehaviour
{
    [SerializeField] private Sprite yes, no, mixed;
    [SerializeField] private GameObject attack, hunt;
    [SerializeField] private Color colorYes, colorNo, colorMixed;

    private TeamsConstructor player;
    private int countListSelectedUnits;

    void Start()
    {
        player = Teams.listOfPlayers[0];
        countListSelectedUnits = player.listSelectedUnits.Count;
        UpdateIcons();  // Initial update
    }

    void Update()
    {
        if (countListSelectedUnits != player.listSelectedUnits.Count)
        {
            countListSelectedUnits = player.listSelectedUnits.Count;
            UpdateIcons();
        }
    }

    public void Attack()
    {
        if (player.listSelectedUnits.Count == 0) return;
        ToggleBehavior(unit => unit.attackingBool, isAttacking: true);
    }

    public void Hunt()
    {
        if (player.listSelectedUnits.Count == 0) return;
        ToggleBehavior(unit => unit.huntBool, isAttacking: false);
    }

    private void ToggleBehavior(Func<UnitBehaviour, bool> behaviorSelector, bool isAttacking)
    {
        bool isMixed = false;
        bool firstBool = behaviorSelector(player.listSelectedUnits[0].GetComponent<UnitBehaviour>());

        foreach (var item in player.listSelectedUnits)
        {
            UnitBehaviour unit = item.GetComponent<UnitBehaviour>();
            if (behaviorSelector(unit) != firstBool)
            {
                isMixed = true;
                break;
            }
        }

        bool newValue = isMixed || !firstBool;

        foreach (var item in player.listSelectedUnits)
        {
            UnitBehaviour unit = item.GetComponent<UnitBehaviour>();
            if (isAttacking)
            {
                unit.attackingBool = newValue;
                unit.huntBool = false;
            }
            else
            {
                unit.huntBool = newValue;
                unit.attackingBool = false;
            }
        }

        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (player.listSelectedUnits.Count == 0)
        {
            UpdateIcon(attack, false, false);
            UpdateIcon(hunt, false, false);

            return;
        }
        bool isAttackingMixed = false;
        bool isHuntingMixed = false;
        bool firstAttackingBool = player.listSelectedUnits[0].GetComponent<UnitBehaviour>().attackingBool;
        bool firstHuntingBool = player.listSelectedUnits[0].GetComponent<UnitBehaviour>().huntBool;

        foreach (var item in player.listSelectedUnits)
        {
            UnitBehaviour unit = item.GetComponent<UnitBehaviour>();
            if (unit.attackingBool != firstAttackingBool) isAttackingMixed = true;
            if (unit.huntBool != firstHuntingBool) isHuntingMixed = true;
        }

        UpdateIcon(attack, firstAttackingBool, isAttackingMixed);
        UpdateIcon(hunt, firstHuntingBool, isHuntingMixed);
    }

    private void UpdateIcon(GameObject icon, bool state, bool isMixed)
    {
        Sprite sprite = isMixed ? mixed : (state ? yes : no);
        Color color = isMixed ? colorMixed : (state ? colorYes : colorNo);

        Image statusImage = icon.GetComponentsInChildren<Image>().Last();
        statusImage.sprite = sprite;
        //statusImage.color = color;
    }
}
