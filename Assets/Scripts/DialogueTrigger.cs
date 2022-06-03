using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue variables")] //put any necessary variables for dialogue system under here

    //Array containing DialogueData. One DialogueData = one wave worth of dialogue
    [SerializeField] private DialogueData _dialogueData;
    //Dialogue Manager object
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private bool StartSceneWithDialogue = false;

    private void Awake()
    {
        if (StartSceneWithDialogue)
        {
            triggerDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Player2")
        {
            triggerDialogue();
            this.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void triggerDialogue()
    {
        if(_dialogueManager)
        {
            //do the dialogue, have dialogue call activate spawners when done
            _dialogueManager.StartDialogue(_dialogueData);
        }
        else
        {
            Debug.Log("No Dialogue Manager in " + this.gameObject.name);
        }
    }
}
