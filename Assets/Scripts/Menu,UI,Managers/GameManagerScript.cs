// Contributor(s): Nathan More
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private static GameManagerScript _instance;

    [SerializeField] private FloatAsset _playerMaxHealth; // MaxHealth stored in data so it can be changes across game from one spot
    [SerializeField] private FloatAsset _player1Health;
    [SerializeField] private FloatAsset _player2Health;

    private LevelLoader levelLoader;

    public CharacterChoices Player1Character { get; set; }
    public bool IsMultiplayer { get; set; }

    public static GameManagerScript Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManagerScript>();
            }

            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            Destroy(this);
        }

    }

    private void Start()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
    }

    // In future this will check for save. If there is one, it will load it. If not, then it calls new game.
    public void StartGame()
    {
        NewGame();
    }

    public void NewGame()
    {
        _player1Health.Value = _playerMaxHealth.Value;
        _player2Health.Value = _playerMaxHealth.Value;

        if (levelLoader)
        {
            levelLoader.LoadLevel("Level 1");
        }
        else
        {
            SceneManager.LoadScene("Level 1");
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
