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
    }

    void AddPlayers()
    {
        CivGameManagerSingleton.Instance.players = new Player[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            CivGameManagerSingleton.Instance.players[i] = new Player(factions[i]);
        }
    }
}
