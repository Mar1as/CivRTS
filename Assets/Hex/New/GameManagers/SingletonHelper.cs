using UnityEngine;

public class SingletonHelper : MonoBehaviour
{
    [SerializeField]
    int numberOfPlayers = 2;
    [SerializeField]
    FactionsInCiv[] factions;
    private void Awake()
    {
        AddPlayers();
        Debug.Log("COŽE?");
    }

    void AddPlayers()
    {
        if (CivGameManagerSingleton.Instance.allFactions != null)
        {
            Debug.Log("CivGameManagerSingleton is not null");
            return;
        }
        CivGameManagerSingleton.Instance.players = new Player[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Debug.Log("LOL2 " + i);

            if (i == 0)
            {
                CivGameManagerSingleton.Instance.players[i] = new Player(i, factions[i], false);
            }
            else CivGameManagerSingleton.Instance.players[i] = new Player(i, factions[i], true);

            Debug.Log("LOL" + i);
        }

        CivGameManagerSingleton.Instance.allFactions = factions;
    }
}
