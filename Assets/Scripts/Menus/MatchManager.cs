using System.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    public float player1Total = 0, player2Total = 0, collabTotal = 0;
    public int currentRound = 1;
    public RoundSettings[] roundSettings;
    public GameObject[] charPrefabs1, charPrefabs2;
    public Transform p1SpawnPoint, p2SpawnPoint;
    private Vector3 p1SpawnPos, p2SpawnPos;
    private GameObject p1Prefab, p2Prefab;
    public GameObject buyGrowBtn, buyHarvestBtn;
    private float buyGrowPrice = 40f, buyHarvestPrice = 50f;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        roundSettings = new RoundSettings[PlayerPrefs.GetInt("selectedRound")];
        PopulateRoundSettings(6, 2, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("MatchManager") != null && GameObject.Find("MatchManager") != gameObject) Destroy(gameObject);
        if(SceneManager.GetActiveScene().name == "Match_Menu") {
            p1SpawnPos = p1SpawnPoint.position;
            p2SpawnPos = p2SpawnPoint.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Match Over!!
        if(currentRound > roundSettings.Length && SceneManager.GetActiveScene().name == "Match_Menu") {
            DisplayResults();
        }
        if(SceneManager.GetActiveScene().name == "Match_Menu") {
            var canvas = GameObject.Find("Canvas");
            buyGrowBtn = GameObject.Find("BuyGrowBtn");
            buyHarvestBtn = GameObject.Find("BuyHarvestBtn");
            Helper.FindInChildren(canvas, "RoundLabel").GetComponent<Text>().text = "Round " + currentRound;
            if(currentRound > roundSettings.Length) return;
            Helper.FindInChildren(canvas, "TimeLabel").GetComponent<Text>().text = "Time to Grow: " + roundSettings[currentRound - 1].growthTime.ToString("0.00") + " sec";
            Helper.FindInChildren(canvas, "YieldLabel").GetComponent<Text>().text = "Harvest Amount: 1 to " + roundSettings[currentRound - 1].yieldAmount;

            Helper.FindInChildren(canvas, "Player 1").GetComponent<Text>().text = "Player 1: " + string.Format("{0:C2}", player1Total);
            Helper.FindInChildren(canvas, "Player 2").GetComponent<Text>().text = "Player 2: " + string.Format("{0:C2}", player2Total);
            Helper.FindInChildren(canvas, "Collab").GetComponent<Text>().text = "Collab Pool: " + string.Format("{0:C2}", collabTotal);

            if (collabTotal < buyGrowPrice) 
            { 
                buyGrowBtn.GetComponent<Button>().interactable = false;
                buyGrowBtn.GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.1f); 
            } 
            else if(roundSettings[currentRound-1].growthTime < 2.005)
            {
                buyGrowBtn.GetComponent<Button>().interactable = false;
                buyGrowBtn.GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.1f); 
                Helper.FindInChildren(canvas, "TimeLabel").GetComponent<Text>().text = "Time to Grow: " + roundSettings[currentRound - 1].growthTime.ToString("0.00") + " sec (MAX!)";
            }
            else
            {
                buyGrowBtn.GetComponentInChildren<Text>().color = new Color(46f / 255f, 170f / 255f, 58f / 255f);
                buyGrowBtn.GetComponent<Button>().interactable = true;
            }

            if(collabTotal < buyHarvestPrice) {
                buyHarvestBtn.GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.1f); 
                buyHarvestBtn.GetComponent<Button>().interactable = false;
            } else if(roundSettings[currentRound-1].yieldAmount == 3)
            {
                buyHarvestBtn.GetComponentInChildren<Text>().color = new Color(0.1f, 0.1f, 0.1f); 
                buyHarvestBtn.GetComponent<Button>().interactable = false;
                Helper.FindInChildren(canvas, "YieldLabel").GetComponent<Text>().text = "Harvest Amount: 1 to " + roundSettings[currentRound - 1].yieldAmount + " (MAX!)";
            } else
            {
                buyHarvestBtn.GetComponentInChildren<Text>().color = new Color(46f / 255f, 170f / 255f, 58f / 255f);
                buyHarvestBtn.GetComponent<Button>().interactable = true;
            }

            Helper.FindInChildren(buyGrowBtn, "Text").GetComponent<Text>().text = "Speed Up ($" + buyGrowPrice + ")";
            Helper.FindInChildren(buyHarvestBtn, "Text").GetComponent<Text>().text = "Increase Yield ($" + buyHarvestPrice + ")";
        }

        if(SceneManager.GetActiveScene().name == "Game") {
            if(GameObject.Find("Player 1") == null && GameObject.Find("Player 2") == null) {
                GameObject p1 = Instantiate<GameObject>(p1Prefab);
                GameObject p2 = Instantiate<GameObject>(p2Prefab);

                // DO NOT REMOVE THESE LINES.
                p1.name = "Player 1";
                p2.name = "Player 2";

                p2.GetComponent<Player>().ID = 2;
                p2.GetComponent<PlayerInput>().defaultActionMap = "PlayerTwo";

                p1.transform.position = p1SpawnPos;
                p2.transform.position = p2SpawnPos;

                // table bits (im sorry we're hardcoding them)
                GameObject.Find("L1").GetComponent<Table>().InsertItem(new ItemStack("Wheat", "Wheat", 1.00f, 1));
                GameObject.Find("R1").GetComponent<Table>().InsertItem(new ItemStack("Wheat", "Wheat", 1.00f, 1));

                if(Random.Range(0, 2) > 0) {
                    GameObject.Find("L2").GetComponent<Table>().InsertItem(new ItemStack("Sugar", "Sugar", 1.50f, 1));
                    GameObject.Find("R2").GetComponent<Table>().InsertItem(new ItemStack("Egg", "Egg", 1.50f, 1));
                } else {
                    GameObject.Find("R2").GetComponent<Table>().InsertItem(new ItemStack("Sugar", "Sugar", 1.50f, 1));
                    GameObject.Find("L2").GetComponent<Table>().InsertItem(new ItemStack("Egg", "Egg", 1.50f, 1));
                }

                if(Random.Range(0, 2) > 0) {
                    GameObject.Find("L3").GetComponent<Table>().InsertItem(new ItemStack("Blueberry", "Blueberry", 1.00f, 1));
                    GameObject.Find("R3").GetComponent<Table>().InsertItem(new ItemStack("Rhubarb", "Rhubarb", 1.25f, 1));
                } else {
                    GameObject.Find("R3").GetComponent<Table>().InsertItem(new ItemStack("Blueberry", "Blueberry", 1.00f, 1));
                    GameObject.Find("L3").GetComponent<Table>().InsertItem(new ItemStack("Rhubarb", "Rhubarb", 1.25f, 1));
                }

                if(currentRound > 1) {
                    if(Random.Range(0, 2) > 0) {
                        GameObject.Find("L4").GetComponent<Table>().InsertItem(new ItemStack("Banana", "Banana", 1.00f, 1));
                        GameObject.Find("R4").GetComponent<Table>().InsertItem(new ItemStack("Carrot", "Carrot", 1.25f, 1));
                    } else {
                        GameObject.Find("R4").GetComponent<Table>().InsertItem(new ItemStack("Banana", "Banana", 1.00f, 1));
                        GameObject.Find("L4").GetComponent<Table>().InsertItem(new ItemStack("Carrot", "Carrot", 1.25f, 1));
                    }
                }
            }
        }

        if(SceneManager.GetActiveScene().name == "Results") {
            var canvas = GameObject.Find("Canvas");
            Helper.FindInChildren(canvas, "Player 1").GetComponent<Text>().text = "Player 1: " + string.Format("{0:C2}", player1Total);
            Helper.FindInChildren(canvas, "Player 2").GetComponent<Text>().text = "Player 2: " + string.Format("{0:C2}", player2Total);
            Helper.FindInChildren(canvas, "Collab").GetComponent<Text>().text = "Collab Pool: " + string.Format("{0:C2}", collabTotal);

            string playerWhoWon = "_";
            if(player1Total > player2Total) { // p1 win
                playerWhoWon = "1";
            } else if(player1Total == player2Total) { // tie
                playerWhoWon = "1 and 2";
            } else { // p2 win
                playerWhoWon = "2";
            }

            Helper.FindInChildren(canvas, "Text (1)").GetComponent<Text>().text = "Player " + playerWhoWon + " Win" + (player1Total == player2Total ? "" : "s") + "!";

            Color pcolor = new Color(0, 0, 0, 0);
            if(player1Total > player2Total) { // p1 win
                pcolor = new Color(0.8113208f, 0.1492524f, 0.1492524f);
            } else if(player1Total == player2Total) { // tie
                pcolor = new Color(0, 0, 0, 1);
            } else { // p2 win
                pcolor = new Color(0.137104f, 0.248794f, 0.745283f);
            }

            Helper.FindInChildren(canvas, "Text (1)").GetComponent<Text>().color = pcolor;
        }

        if(SceneManager.GetActiveScene().name == "Main_Menu") {
            PopulateRoundSettings(6, 2, 1);
            player1Total = 0;
            player2Total = 0;
            collabTotal = 0;
        }
    }

    void DisplayResults() {
        GameObject.Find("MusicMan").GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene("Results");
    }

    public void AddRoundDetails(float player1Money, float player2Money, float collabScore) {
        currentRound++;
        player1Total += player1Money;
        player2Total += player2Money;
        collabTotal += collabScore;
    }

    public void StartRound()
    {
        GameObject plotPrefab = Resources.Load<GameObject>("Prefabs/Plot");
        plotPrefab.GetComponent<PlotController>().timeToGrow = roundSettings[currentRound - 1].growthTime;
        plotPrefab.GetComponent<PlotController>().maxYield = roundSettings[currentRound - 1].yieldAmount;

        GetCharacters();
        GameObject.Find("MusicMan").GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene("Game");
    }

    public void ResetRounds() {
        roundSettings = new RoundSettings[PlayerPrefs.GetInt("selectedRound")];
    }

    public void GetCharacters()
    {
        int p1SelectedCharacter = PlayerPrefs.GetInt("p1SelectedCharacter");
        int p2SelectedCharacter = PlayerPrefs.GetInt("p2SelectedCharacter");

        p1Prefab = charPrefabs1[p1SelectedCharacter];
        p2Prefab = charPrefabs2[p2SelectedCharacter];
    }

    public void BuyGrow()
    {
        Debug.Log("Buy grow!");
        if (collabTotal >= buyGrowPrice)
        {
            float time = roundSettings[currentRound - 1].growthTime;
            roundSettings[currentRound - 1].growthTime = time - ((time - roundSettings[currentRound - 1].minGrowthTime)/3.0f);
            PopulateRoundSettings(roundSettings[currentRound - 1].growthTime, roundSettings[currentRound - 1].minGrowthTime, roundSettings[currentRound - 1].yieldAmount);
            collabTotal -= buyGrowPrice;
        }
    }

    public void BuyHarvest()
    {
        Debug.Log("Buy Harvet!");
        if (collabTotal >= buyHarvestPrice)
        {
            roundSettings[currentRound - 1].yieldAmount++;
            PopulateRoundSettings(roundSettings[currentRound - 1].growthTime, roundSettings[currentRound - 1].minGrowthTime, roundSettings[currentRound - 1].yieldAmount);
            collabTotal -= buyHarvestPrice;
        }
    }

    private void PopulateRoundSettings(float growthTime, float minGrowth, int yield) {
        for (int i = 0; i < roundSettings.Length; i++)
        {
            roundSettings[i] = new RoundSettings(growthTime, minGrowth, yield);
        }
    }

}

[System.Serializable]
public class RoundSettings {
    public float growthTime = 6;
    public float minGrowthTime = 2;
    public int yieldAmount = 1;
    public RoundSettings(float growthTime, float minGrowthTime, int yieldAmount)
    {
        this.growthTime = growthTime;
        this.minGrowthTime = minGrowthTime;
        this.yieldAmount = yieldAmount;
    }
}
