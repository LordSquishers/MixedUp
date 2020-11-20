using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HarvestClick : MonoBehaviour
{
    public void OnClick() {
        if(SceneManager.GetActiveScene().name == "Match_Menu") {
            GameObject.Find("MatchManager").GetComponent<MatchManager>().BuyHarvest();
        }
    }
}
