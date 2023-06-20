using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public List<Character> charList = new List<Character>();
    public Dictionary<Character, GameObject> characterObjects = new Dictionary<Character, GameObject>();
    public Transform characterHolder;
    public GameObject characterPrefab;
    public static CharacterController instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        BuildCharacters();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void BuildCharacters()
    {
        Debug.Log("Start building characters");
        foreach(var person in charList)
        {
            GameObject go = Instantiate(characterPrefab, characterHolder);
            go.name = person.sysName;
            go.GetComponent<UnityEngine.UI.Image>().sprite = person.characterImage;
            Debug.Log($"Builded {person.Name}");
            characterObjects.Add(person, go);
            if (person.characterImage != null) continue;
            else go.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0);
        
        }
        Debug.Log("Done building");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
