using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Linq; 
using UnityEngine.Audio;
public class DialogueManager : MonoBehaviour
{
    private bool _isDialogue = false; 
    [SerializeField] GameObject DialoguePanel;
    [SerializeField] GameObject GoPanel;

    [SerializeField] Image SpeakerImage;
 
    [SerializeField] TextMeshProUGUI SpeakerName; 
    [SerializeField] TextMeshProUGUI DialogueLine;
    private int currentIndex = 0;
    private DialogueData privateData;
    
    public void StartDialogue(DialogueData data)
    {
        _isDialogue = true; 
        DialoguePanel.SetActive(true);
        GoPanel.SetActive(false);
        //First, freeze time
        //Time.timeScale = 0;
        privateData = data;
        DisplayDialogue();
    }

    private void DisplayDialogue()
    {
        if(privateData)
        {
            DialogueLine.text = privateData.GetLine(currentIndex);
            SpeakerName.text = privateData.GetSpeaker(currentIndex);
            SpeakerImage.sprite = privateData.GetImage(currentIndex);
        }
        else
        {
            Debug.Log("No Dialogue Data");
        }
    }

    public void OnPressNextButton()
    {
        if(privateData)
        {
            if (privateData.GetLength() != currentIndex + 1)
            {
                currentIndex += 1;
                DisplayDialogue();
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            Debug.Log("No Dialogue Data");
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        _isDialogue = false; 
        //Time.timeScale = 1;
        currentIndex = 0;
        DialoguePanel.SetActive(false); 
    }

    public bool IsDialogueOver()
    {
        return _isDialogue; 
    }

    public void SetDialogueBool(bool Dialogue)
    {
        _isDialogue = Dialogue;
    }
}
