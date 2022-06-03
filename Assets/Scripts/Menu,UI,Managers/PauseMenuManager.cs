using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    private PlayerController PC;
    private PlayerController PC2;
    private bool CanPause = true;
    // Start is called before the first frame update
    void awake()
    {
        //levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if(GameObject.FindGameObjectWithTag("Player2"))
        {
            PC2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController>();
        }
    }

    public void setCanPause(bool pause)
    {
        CanPause = pause;
    }

    public void RestartLevel() {
        Debug.Log("Restart Level");
            Time.timeScale = 1;
        if (levelLoader) {
            levelLoader.LoadLevel(SceneManager.GetActiveScene().name);
        } else {
            Debug.LogError("Level Loader not found in Pause Menu Script");
        }
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Main Menu");
        if(levelLoader) 
        {
            Time.timeScale = 1;
            levelLoader.LoadLevel("Start Menu");
        }
        else
        {
            Debug.LogError("Level Loader not found in Pause Menu Script");
        }
    }

    public void quit()
    {
        Debug.Log("Quit");
        AkSoundEngine.PostEvent("Game_Quit", gameObject);
        Application.Quit();
    }

    public void Resume()
    {
        Debug.Log("Resume");
        AkSoundEngine.PostEvent("Game_Unpause", gameObject);
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
        if (PC)
        {
            PC.setPaused(false);
        }
        if (PC2)
        {
            PC2.setPaused(false);
        }
    }

    public void pause()
    {
        if(CanPause)
        {
            AkSoundEngine.PostEvent("Game_Pause", gameObject);
            Time.timeScale = 0;
            this.gameObject.SetActive(true);
            if (PC)
            {
                PC.setPaused(true);
            }
            if (PC2)
            {
                PC2.setPaused(true);
            }
        }
        
    }

}
