using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] p1Characters, p2Characters;
    public int p1SelectedCharacter = 0, p2SelectedCharacter = 0;
    public Text p1Label, p2Label;
    public int selectedRound = 5;
    public Text roundLabel;
    public int mixerDelete = 1;
    public Text mixerLabel;

    void Update()
    {
        p1Label.text = p1Characters[p1SelectedCharacter].name.Substring(2);
        p2Label.text = p2Characters[p2SelectedCharacter].name.Substring(2);
        roundLabel.text = ""+selectedRound;
        mixerLabel.text = ""+(mixerDelete == 1 ? "YES":"NO");
    }

    public void P1NextCharacter()
    {
        p1Characters[p1SelectedCharacter].SetActive(false);
        p1SelectedCharacter = (p1SelectedCharacter + 1) % p1Characters.Length;;
        p1Characters[p1SelectedCharacter].SetActive(true);
    }

    public void P1PreviousChracter()
    {
        p1Characters[p1SelectedCharacter].SetActive(false);
        p1SelectedCharacter--;
        if (p1SelectedCharacter < 0)
        {
            p1SelectedCharacter += p1Characters.Length;;
        }
        p1Characters[p1SelectedCharacter].SetActive(true);
    }   

     public void P2NextCharacter()
    {
        p2Characters[p2SelectedCharacter].SetActive(false);
        p2SelectedCharacter = (p2SelectedCharacter + 1) % p2Characters.Length;;
        p2Characters[p2SelectedCharacter].SetActive(true);
    }

    public void P2PreviousChracter()
    {
        p2Characters[p2SelectedCharacter].SetActive(false);
        p2SelectedCharacter--;
        if (p2SelectedCharacter < 0)
        {
            p2SelectedCharacter += p2Characters.Length;;
        }
        p2Characters[p2SelectedCharacter].SetActive(true);
    }

    public void RoundIncrease()
    {
        if (selectedRound < 10)
        {
            selectedRound++;
        }
    }

    public void RoundDecrease()
    {
        if (selectedRound > 3)
        {
            selectedRound--;
        }
    }
    public void mixerDelOn()
    {
        mixerDelete = 1;
    }

    public void mixerDelOff()
    {
        mixerDelete = 0;
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("p1SelectedCharacter", p1SelectedCharacter);
        PlayerPrefs.SetInt("p2SelectedCharacter", p2SelectedCharacter);
        PlayerPrefs.SetInt("selectedRound", selectedRound);
        PlayerPrefs.SetInt("mixerDelete", mixerDelete);

        GameObject.Find("Main Camera").GetComponent<AudioSource>().Stop();
        if(GameObject.Find("MatchManager") != null) {
            GameObject.Find("MatchManager").GetComponent<MatchManager>().ResetRounds();
        }
        SceneManager.LoadScene("Match_Menu");
    }
}
