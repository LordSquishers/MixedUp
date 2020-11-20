using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioClip[] inGameMusics;

    void Awake()
    {
        SceneManager.sceneLoaded += this.OnLoadCallback;
    }


    void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == "Game")
        {
            GameObject.Find("UI ELEMENTS").GetComponents<AudioSource>()[1].clip = inGameMusics[Random.Range(0, inGameMusics.Length)];
            GameObject.Find("UI ELEMENTS").GetComponents<AudioSource>()[1].Play();
        }
    }
}
