using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    [SerializeField] private float TransitionTime = 1f;
    private bool isdone = false;
    //loads the scene with the name you give it if it exists in the build
    public void LoadLevel(string SceneName)
    {
        if (SceneName != null)
        {
            StartCoroutine(LevelTransition(SceneName));
        }
        else
        {
            Debug.LogError("Level loader was called but no Scene Name was given");
        }
    }

    IEnumerator LevelTransition(string SceneName)
    {
        transition.SetTrigger("Start");
        
        yield return new WaitForSeconds(TransitionTime);
        
        SceneManager.LoadScene(SceneName);
        
    }

    public void toggleDone()
    {
        isdone = !isdone;
    }

    public bool checkDone()
    {
        return isdone;
    }
}
