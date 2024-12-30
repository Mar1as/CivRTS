using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField]
    public static int endScore = 500;
    bool endGame = false;

    float TimeMax = 1;
    float TimeCurrent = 0;

    [SerializeField]
    int multiplierScore = 1;
    [SerializeField]
    int multiplierMoney = 1;
    [SerializeField]
    int gainMoneyFromZone = 1;
    [SerializeField]
    int gainMoney = 1;

    [SerializeField]
    GameObject endText;
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GameObject left, right, money;
    [SerializeField] Image leftIcon, rightIcon;

    SliderConstructor leftSlider, rightSlider;
    private void Start()
    {
        leftSlider = new SliderConstructor(left);
        rightSlider = new SliderConstructor(right);
    }
    private void Update()
    {
        Timeing();
        Graphics();
        EndGame();

    }

    public void Timeing()
    {
        if (TimeCurrent >= TimeMax)
        {
            TimeCurrent = 0;
            TeamsForLoop();
        }
        else
        {
            TimeCurrent += Time.deltaTime;
        }

    }
    void TeamsForLoop()
    {
        for (int i = 0; i < Teams.listOfPlayers.Count; i++)
        {
            AddScore(Teams.listOfPlayers[i]);
            AddMoney(Teams.listOfPlayers[i]);
        }
    }
    void AddScore(TeamsConstructor player)
    {
        player.score += player.controledZones * multiplierScore;
        EndScore(player);
    }
    void AddMoney(TeamsConstructor player)
    {
        player.money += gainMoneyFromZone * multiplierMoney * player.controledZones;
        player.money += gainMoney * multiplierMoney;
    }
    void EndScore(TeamsConstructor player)
    {
        if (player.score >= endScore && endGame == false)
        {
            endGame = true; //aby se to nevolalo poøád dokola

            
            
            GameObject finalPanel = Instantiate(endText, canvas.transform);
            TextMeshProUGUI textMeshPro = finalPanel.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log(textMeshPro.gameObject.name);
            textMeshPro.text = $"{player.tag} WON\nPress Enter";
            Image img = finalPanel.GetComponentInChildren<Image>();
            Debug.Log(img.gameObject.name);
            img.sprite = player.playerFaction.flag;
            img.color = player.teamColor;
        }
    }

    void EndGame()
    {
        if (endGame == true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Teams.listOfPlayers.Clear();

                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

                foreach (GameObject obj in allObjects)
                {
                    Destroy(obj);
                }

                Destroy(this.gameObject);

                SceneManager.LoadScene(0);//Menu
            }
        }
    }

    void Graphics()
    {
        leftSlider.ChangeSlider(Teams.listOfPlayers[0].score, endScore);
        rightSlider.ChangeSlider(Teams.listOfPlayers[1].score, endScore);
        money.GetComponent<TextMeshProUGUI>().text = Teams.listOfPlayers[0].money.ToString();

        leftIcon.sprite = Teams.listOfPlayers[0].playerFaction.flag;
        rightIcon.sprite = Teams.listOfPlayers[1].playerFaction.flag;
    }
}
