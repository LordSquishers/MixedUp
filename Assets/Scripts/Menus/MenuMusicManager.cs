using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicManager : MonoBehaviour
{
    public AudioClip[] menuMusics;

    void Awake()
    {
        SceneManager.sceneLoaded += this.OnLoadCallback;
    }


    void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == "Main_Menu")
        {
            GameObject.Find("Main Camera").GetComponent<AudioSource>().clip = menuMusics[Random.Range(0, menuMusics.Length)];
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Play();
        }
    }

    private void OnDestroy()
    {
        GetComponent<AudioSource>().Stop();
    }
}
