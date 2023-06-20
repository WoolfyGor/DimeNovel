using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class DialogueController : MonoBehaviour
{
    [SerializeField] Image backgroundCollor, imageHolder, textBox;

    [SerializeField] List<Image> characters = new List<Image>();
    [SerializeField] List<Image> debugCharacters = new List<Image>();
    [SerializeField] DialogueScene currentScene = new DialogueScene();
    private void Awake()
    {
        CreateInitialScene();
    }
    void CreateInitialScene()
    {
        currentScene.FillSceneContent(imageHolder, characters, backgroundCollor, textBox);
        currentScene.HideScene();
        characters.Clear();
    }

    public void ExampleStart()
    {
        currentScene.RenewCharacters(characters);
        currentScene.StartScene();

    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public void InsertCharacter(int num)
    {
        characters.Clear();
        switch (num)
        {
            case 0:
                InsertCharacter(3);
                currentScene.HideScene();
                break;
            case 1:
                characters.Add(debugCharacters[0]);
                break;
            case 2:
                characters.Add(debugCharacters[0]);
                characters.Add(debugCharacters[1]);
                break;
            case 3:
                characters.Add(debugCharacters[0]);
                characters.Add(debugCharacters[1]);
                characters.Add(debugCharacters[2]);
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InsertCharacter(1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            InsertCharacter(2);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            InsertCharacter(3);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExampleStart();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InsertCharacter(0);
        }
        
    }
}
public class DialogueScene
{
    public Image backgroundHolder;
    public Image characterHolder;
    public Image collorHolder;
    public Image textBox;

    public List<Vector2> InitialSizes = new List<Vector2>();
    public List<Image> characters = new List<Image>();
    void FillSizes()
    {
        InitialSizes.Add(backgroundHolder.rectTransform.rect.size);
        InitialSizes.Add(collorHolder.rectTransform.rect.size);
        InitialSizes.Add(textBox.rectTransform.rect.size);
    }

    public void FillSceneContent(Image bck,List<Image> character,Image collor,Image textB)
    {
        backgroundHolder = bck;
        characters.AddRange(character);
        collorHolder = collor;
        textBox = textB;

        FillSizes();
    }

    public void HideScene()
    {
        backgroundHolder.DOFade(0, 0);
        textBox.DOFade(0, 0);
        textBox.rectTransform.sizeDelta = new Vector2(0, 0);
        foreach(var character in characters)
        {
            character.DOFade(0,0);
        }   
    }
    public void RenewCharacters(List<Image> charactersNew)
    {
        characters.Clear();
        characters.AddRange(charactersNew);
    }
    public void StartScene()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(backgroundHolder.DOFade(1, 1));
        switch (characters.Count)
        {
            case 1:
                seq.Append(characters[0].rectTransform.DOLocalMoveX(characters[0].rectTransform.position.x + 100, 0));

                seq.Insert(1f, characters[0].DOFade(1, 0.5f)); 
                seq.Append(characters[0].rectTransform.DOLocalMoveX(0, .8f));
                break;
            case 2:
                seq.Append(characters[0].rectTransform.DOLocalMoveX(0, 0));
                seq.Append(characters[1].rectTransform.DOLocalMoveX(0, 0));

                seq.Insert(1f, characters[0].DOFade(1, 0.5f));
                seq.Append(characters[0].rectTransform.DOLocalMoveX(350, .8f));

                seq.Insert(1.5f, characters[1].DOFade(1, 0.5f));
                seq.Insert(1.65f,characters[1].rectTransform.DOLocalMoveX(-350, .8f));
                break;
            case 3:
                seq.Append(characters[0].rectTransform.DOLocalMoveX(0, 0));
                seq.Append(characters[1].rectTransform.DOLocalMoveX(0, 0));
                seq.Append(characters[2].rectTransform.DOLocalMoveY(characters[2].rectTransform.position.y - 150, 0));

                seq.Insert(1f, characters[0].DOFade(1, 0.5f));
                seq.Append(characters[0].rectTransform.DOLocalMoveX(600, .8f));


                seq.Insert(1.5f, characters[1].DOFade(1, 0.5f));
                seq.Insert(1.65f, characters[1].rectTransform.DOLocalMoveX(-600, .8f));

                seq.Insert(2f, characters[2].DOFade(1, 0.5f));
                seq.Insert(2.1f, characters[2].rectTransform.DOLocalMoveY(-40, .8f));

                break;
        }




        switch (characters.Count)
        {
            case 1:
                seq.Append(characters[0].rectTransform.DOLocalMoveX(0, 1f));
                seq.Insert(1f, characters[0].DOFade(1, 0.5f));
                break;
        }

        seq.InsertCallback(1.75f,() =>
        {
            textBox.rectTransform.DOSizeDelta(InitialSizes[2],0.5f);
            textBox.DOFade(1,0.25f);
        });

    }
}
