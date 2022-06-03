using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class GameOver : MonoBehaviour
{
    private LevelLoader levelLoader;
    private void Start()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>(); 
    }
    public void BackToStart()
    {
        if(levelLoader)
        {
            levelLoader.LoadLevel("Start Menu"); 
        }
        else
        {
            Debug.LogError("Level Loader not found in Game Over Script"); 
        }
    }
}
