// Contributor(s): Nathan More
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayerController _player1;
    [SerializeField] private PlayerController _player2;
    [SerializeField] private CharacterData lizData;
    [SerializeField] private CharacterData jayData;
    [SerializeField] private GameObject _gameOverPanel;

    private void Start()
    {
        if (GameManagerScript.Instance.IsMultiplayer == true)
        {
            SetupMultiplayerCharacters();
        }
    }

    public void SetupMultiplayerCharacters()
    {
        _player2.gameObject.SetActive(true);

        if (GameManagerScript.Instance.Player1Character == CharacterChoices.Jay)
        {
            _player1.CharacterData = jayData;
            _player2.CharacterData = lizData;
        }
        else
        {
            _player1.CharacterData = lizData;
            _player2.CharacterData = jayData;
        }
        _player1.SetupCharacter();
        _player2.SetupCharacter();
    }


    private void Update()
    {
        if(GameManagerScript.Instance.IsMultiplayer == true)
        {
            if (_player1.checkDown() && _player2.checkDown())
            {
                gameOver();
            }
        }
        else if(_player1.checkDown())
        {
            gameOver();
        }
    }
    public void gameOver()
    {
        _gameOverPanel.SetActive(true);
        if (GameManagerScript.Instance.IsMultiplayer == true)
        {
            _player1.setPaused(true);
            _player2.setPaused(true);
            _player1.gameObject.SetActive(false);
            _player2.gameObject.SetActive(false);
        }
        else if (_player1)
        {
            _player1.setPaused(true);
            _player1.gameObject.SetActive(false);
        }
    }
}
