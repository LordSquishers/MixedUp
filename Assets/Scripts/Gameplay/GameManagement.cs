using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManagement : MonoBehaviour
{
    private Dictionary<Player, float> playerMoney;
    private float collabMoney = 0;
    private GameObject empty;

    public float timeInRound;

    public float timeRemaining;

    void Start()
    {
        playerMoney = new Dictionary<Player, float>();
        timeRemaining = timeInRound;
    }

    void FixedUpdate() {
        if(timeRemaining < 0) {
            float p1M = 0, p2M = 0;
            foreach(KeyValuePair<Player, float> entry in playerMoney) {
                if(entry.Key.ID == 1) p1M = entry.Value;
                if(entry.Key.ID == 2) p2M = entry.Value;
            }
            GameObject.Find("MatchManager").GetComponent<MatchManager>().AddRoundDetails(p1M, p2M, collabMoney);
            GameObject.Find("UI ELEMENTS").GetComponents<AudioSource>()[1].Stop();
            SceneManager.LoadScene("Match_Menu");
        }

        timeRemaining -= Time.deltaTime;
    }

    public string GetTimeRemaining() {
        return ((int)timeRemaining).ToString();
    }

    public void PollMoney() {
        foreach(KeyValuePair<Player, float> entry in playerMoney) {
            UnityEngine.Debug.Log(entry.Key + " " + entry.Value);
        }
    }

    public void AddIndividualMoney(Player player, float amount)
    {
        if (!playerMoney.ContainsKey(player))
        {
            playerMoney.Add(player, amount);
        }
        else 
        {
            playerMoney[player] += amount;
        }
    }

    public void AddCollabMoney(float amount) {
        collabMoney += amount;
    }

    public void DistributeMoney(Player p1, Player p2, float amount) {
        AddIndividualMoney(p1, amount / 2f);
        AddIndividualMoney(p2, amount / 2f);
    }

    public float GetMoney(int id)
    {
        float sum = 0;
        if (id == -1)
        {
            sum = 0;
            foreach(KeyValuePair<Player, float> entry in playerMoney) 
            {
                sum += entry.Value;
            }
        }
        foreach(KeyValuePair<Player, float> entry in playerMoney) 
        {
                if(entry.Key.ID == id) sum = entry.Value;
        }
        return sum;
    }

}
