// Contributor(s): Nathan More
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;

    public bool lizChosen;
    public bool jayChosen;
    public bool twoPlayers;
    public CharacterChoices keyboardChoice = CharacterChoices.None;

    [SerializeField] private GameObject lizPrefab;
    [SerializeField] private GameObject jayPrefab;
    [SerializeField] GameObject[] spawnPoints;

    private PlayerInputManager _playerInputManager;

    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlayerManager>();
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

        _playerInputManager = GetComponent<PlayerInputManager>();

        lizChosen = false;
        jayChosen = false;
        twoPlayers = false;
    }

    public void Update()
    {
        CheckStartGame();
        
    }

    public void CheckStartGame()
    {
        if (_playerInputManager.playerCount == 1)
        {
            if (lizChosen || jayChosen)
            {
                _playerInputManager.gameObject.SetActive(false);
                GameManagerScript.Instance.NewGame();
                //StartCoroutine(CheckForSpawn(1.0f));
                //CheckForSpawn();
            }
        }
        else if (_playerInputManager.playerCount == 2)
        {
            twoPlayers = true;

            if (lizChosen && jayChosen)
            {
                _playerInputManager.gameObject.SetActive(false);
                GameManagerScript.Instance.StartGame();
            }
        }
    }

    public void ChoseCharacter(CharacterChoices character, bool isKeyboard)
    {
        if (isKeyboard)
        {
            keyboardChoice = character;
        }
        
        if (character == CharacterChoices.Liz)
        {
            lizChosen = true;
        }
        else if (character == CharacterChoices.Jay)
        {
            jayChosen = true;
        }
    }

    public void CheckForSpawn()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        if (spawnPoints != null && spawnPoints.Length > 1)
        {
            SpawnCharacters(spawnPoints[0].transform.position, spawnPoints[1].transform.position);
        }
        else
        {
            Debug.Log("Couldn't find spawn points");

            SpawnCharacters(new Vector3(0, 0, 0), new Vector3(-2, 2, 0));
        }
    }

    public void SpawnCharacters(Vector3 spawn1, Vector3 spawn2)
    {
        if (keyboardChoice == CharacterChoices.Liz)
        {
            //lizPrefab.GetComponent<PlayerInput>().defaultControlScheme = "Keyboard";
            GameObject player1 = Instantiate(lizPrefab, spawn1, Quaternion.identity);
            player1.GetComponent<PlayerInput>().SwitchCurrentControlScheme("Keyboard");
            player1.GetComponent<PlayerInput>().ActivateInput();
        }
        else if (keyboardChoice == CharacterChoices.Jay)
        {
            jayPrefab.GetComponent<PlayerInput>().defaultControlScheme = "Keyboard";
            Instantiate(jayPrefab, spawn1, Quaternion.identity);
        }

        if (twoPlayers == true)
        {
            if (keyboardChoice == CharacterChoices.Liz)
            {
                jayPrefab.GetComponent<PlayerInput>().defaultControlScheme = "Controller";
                Instantiate(jayPrefab, spawn2, Quaternion.identity);
            }
            else if (keyboardChoice == CharacterChoices.Jay)
            {
                lizPrefab.GetComponent<PlayerInput>().defaultControlScheme = "Controller";
                Instantiate(lizPrefab, spawn2, Quaternion.identity);
            }
        }
    }

    //public IEnumerator CheckForSpawn(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);

    //    spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
    //    if (spawnPoints != null && spawnPoints.Length > 1)
    //    {
    //        SpawnCharacters(spawnPoints[0].transform.position, spawnPoints[1].transform.position);
    //    }
    //    else
    //    {
    //        Debug.Log("Couldn't find spawn points");
    //    }
    //}
}
