using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Data", menuName = "ScriptableObjects/DialogueData", order = 2)]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    private struct DialoguePair
    {
        public string speaker;
        public string line;
        public Sprite speakerImage; 
    }
    [SerializeField] List<DialoguePair> dialogue; 
    //Dialogue itself is stored as a string value; the speaker's name is stored as a string key 
    public string GetLine(int lineNum)
    {
        return dialogue[lineNum].line; 
    }
    public string GetSpeaker(int lineNum)
    {
        return dialogue[lineNum].speaker;
    }
    public int GetLength()
    {
        return dialogue.Count; 
    }
    public Sprite GetImage(int lineNum)
    {
        return dialogue[lineNum].speakerImage; 
    }

}
