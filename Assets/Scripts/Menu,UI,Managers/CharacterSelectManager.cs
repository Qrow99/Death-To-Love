using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public bool LizChosen { get; set; }
    public bool JayChosen { get; set; }
    //public CharacterChoices Player1Choice { get; set; }


    private void Awake()
    {
        LizChosen = false;
        JayChosen = false;
        //Player1Choice = CharacterChoices.None;
    }

    public void ChoseCharacter(CharacterChoices character, bool isPlayer1)
    {
        if (isPlayer1)
        {
            //Player1Choice = character;
            GameManagerScript.Instance.Player1Character = character;
        }

        if (character == CharacterChoices.Liz)
        {
            LizChosen = true;
        }
        else if (character == CharacterChoices.Jay)
        {
            JayChosen = true;
        }
    }

    public void DeselectCharacter(CharacterChoices character, bool isPlayer1)
    {
        if (isPlayer1)
        {
            //Player1Choice = character;
            GameManagerScript.Instance.Player1Character = CharacterChoices.None;
        }

        if (character == CharacterChoices.Liz)
        {
            LizChosen = false;
        }
        else if (character == CharacterChoices.Jay)
        {
            JayChosen = false;
        }
    }

    public void CheckStartGame()
    {
        if (LizChosen == true && JayChosen == true)
        {
            GameManagerScript.Instance.StartGame();
        }
    }
}
