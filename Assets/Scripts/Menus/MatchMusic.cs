using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchMusic : MonoBehaviour
{
    public AudioClip[] matchMenuMusics;

    void Awake()
    {
        SceneManager.sceneLoaded += this.OnLoadCallback;
    }


    void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == "Match_Menu")
        {
            GameObject.Find("MusicMan").GetComponent<AudioSource>().clip = matchMenuMusics[Random.Range(0, matchMenuMusics.Length)];
            GameObject.Find("MusicMan").GetComponent<AudioSource>().Play();
        }
    }

    private void OnDestroy()
    {
        GetComponent<AudioSource>().Stop();
    }
}
