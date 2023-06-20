using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Novel parts/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public int idDialogue;
    public Background currentBg;
    public Background nextBg;
    public List<Character> sceneCharacters = new List<Character>();
    public List<Character> activeCharacters = new List<Character>();
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[System.Serializable]
public class DialogueLine
{
    public int idLine;
    public Character whom;
    public string what;
    public Action action = null;
}

[System.Serializable]
public class Scenario
{
    public List<Dialogue> dialoguesList = new List<Dialogue>();
}