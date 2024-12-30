using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[Serializable]
public class DataUnitRTS
{
    public MainUnitRTS mainUnitRTS { get; private set; }

    PlayerRTS playerOwner;

    MainWeaponRTS mainWeapon;
    MainWeaponRTS MainWeapon
    {
        get
        {
            if (mainWeapon == null)
            {
                return mainWeapon = mainUnitRTS.GetComponentInChildren<MainWeaponRTS>();
            }
            else
            {
                return null;
            }
        }
    }

    MainUnitRTS currentEnemy;

    Vector3 currentDestination;

    NavMeshAgent navMeshAgent;

    [SerializeField]
    int maxHp;

    int currentHp;
    int CurrentHp
    {
        get
        {
            return currentHp;
        }
        set
        {
            currentHp = value;
            if (currentHp <= 0)
            {
                Die();
            }
        }
    }

    UnitState currentState = UnitState.Idle;

    public DataUnitRTS(MainUnitRTS mainUnitRTS, PlayerRTS playerOwner)
    {
        this.mainUnitRTS = mainUnitRTS;
        this.playerOwner = playerOwner;

        currentHp = maxHp;
    }

    public void Update()
    {
        SelectOption();
    }

    void SelectOption()
    {
        switch (currentState)
        {
            case UnitState.Idle:
                FindEnemy();
                break;
            case UnitState.Move:
                Move();
                break;
            case UnitState.Attack:
                Attack();
                break;
            case UnitState.AttackDuringMove:
                Move();
                Attack();
                break;
            default:
                break;
        }
    }

    void FindEnemy()
    {

    }

    void Move()
    {
    }

    void Attack()
    {

    }

    void Die()
    {

    }


}

enum UnitState
{
    Idle,
    Move,
    Attack,
    AttackDuringMove,
    Dead
}
