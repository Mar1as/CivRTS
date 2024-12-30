using UnityEngine;

public class MainWeaponRTS : MonoBehaviour
{
    [SerializeField]
    DataWeaponRTS dataWeaponRTS;
    void Start()
    {
        Setup();
    }

    void Update()
    {
        dataWeaponRTS.Update();
    }

    void Setup()
    {
        dataWeaponRTS = new DataWeaponRTS(this);
    }
}
