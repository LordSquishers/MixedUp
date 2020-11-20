using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public Text p1MonLabel, p2MonLabel, collabLabel, timeRem;
    public GameManagement gameManager;
    public MatchManager matchManager;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            GameObject.Find("MatchManager");
        }
    }
    void Update()
    {
        timeRem.text = gameManager.GetTimeRemaining();
        p1MonLabel.text = "$" + gameManager.GetMoney(1).ToString("0.00");
        p2MonLabel.text = "$" + gameManager.GetMoney(2).ToString("0.00");
        collabLabel.text = "Collab $" + Merchant.collabMoney.ToString("0.00");
    }
}
