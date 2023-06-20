using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Novel parts/Character", order = 3)]
public class Character : ScriptableObject
{
    public int idCharacter;
    public Sprite characterImage;
    public string sysName;
    public string Name;
    public List<Sprite> emotions = new List<Sprite>();
}