using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchMenu : MonoBehaviour
{
    void OnClick() {
        GameObject.Find("MatchManager").GetComponent<MatchManager>().StartRound();
    }

}
