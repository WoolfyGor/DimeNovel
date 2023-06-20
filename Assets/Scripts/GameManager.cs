using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public Scenario scenario; // �������� ��� ���� (�������� ��� ������� ����)
    public Dialogue currentDialogue; // ������� ��������������� ��������
    public int lineId = 1; // ������ ������� ������
    CharacterController charController; // ������ �� ���������� ���������� (��� ������)
    public List<GameObject> activeCharacters = new List<GameObject>();//�������� ��������� �� �������� ������
    [Header("Scene objects")]
    public Transform mainCanvas; // ������ ��� ���������� ��� ����������
    public Image backgroundImage; // ��� ���� �����
    public List<Transform> characterPositions = new List<Transform>(); // ������ �� ������� �� �����
    public List<GameObject> positionSlots = new List<GameObject>(5); // ������� ����� ���������� ��� ����������� ����� ������
    public GameObject textArea; // ���� ��� ����� (��������)
    public TMPro.TMP_Text textComponent; // �������� ����� �������
    public TMPro.TMP_Text nameComponent; // ��� ����������

    bool isBusy;//������ �� ��������� (������� ������)
    // Start is called before the first frame update
    void Start()
    {
        charController = CharacterController.instance;
        GameWorker();//�������� �� ���� ���������� �������� � ����
    }
    public void GameWorker()
    {
        StartCoroutine("DialogueLiner"); // ��������� �������� �������� �� ������������ ������
        DoScroll(); // ������ ��������� ���������
    }

    IEnumerator DialogueLiner()
    {
        currentDialogue = scenario.dialoguesList[0]; // ��������� ������ � ������ ������ :: �������� �� ��������� �������
        while (enabled) // ������� ������
        {
            if (Input.GetMouseButtonDown(0)) yield return ClickEvent(); // ���� ���� ������ ���, ���������� ������� ���������� �����
            yield return null;
        }
    }

    void DoScroll()
    {
        if (isBusy) // ���� ����� ��� ���������, �� �������� ��������� ���� ����� � ������� ��� �����, ��������� ������
        {
            ForceFillText(currentDialogue.dialogueLines[lineId-1].what);
            return;
        }

        if (lineId < currentDialogue.dialogueLines.Count) {//������� �������, ���� ������ ������� � ������� �� ��������� 
            if (backgroundImage.sprite != currentDialogue.currentBg.image)// �������� �� ������ ���� (����� ��?)
            {
                backgroundImage.sprite = currentDialogue.currentBg.image;
                Debug.Log($"Renew bg to {currentDialogue.currentBg.backgroundName}");
            }
            if (currentDialogue.dialogueLines[lineId].action.idAction == 0) // ���� ��� �������� - ��������� ������� ������ � ��������� ���
            {
                nameComponent.text = currentDialogue.dialogueLines[lineId].whom.Name;
                StartCoroutine(FillText(currentDialogue.dialogueLines[lineId].what));
                lineId++;
            }
            else
            {//����� ��������� ����� :: ��������� �� ����������
                DoAction(currentDialogue.dialogueLines[lineId].action.idAction, currentDialogue.dialogueLines[lineId].action.attributes);
                lineId++;
                DoScroll(); // ������������ ������ ��� ���, ������ ��� ������ (� ������ ������) ��������������� �����������
            }
        }
        else
        {
            Debug.Log("Reached end of dialogue"); // ������� ����� �� ��������� ���� �������
        }


    }
    void DoAction(int id, List<string> args)
    {
        switch (id)
        {
            case 1: // ����� �������
                if (int.Parse(args[0]) != currentDialogue.idDialogue)
                { // ������ ������ ������� ������ �� ���, ��� ������ � �������� ������
                    currentDialogue = scenario.dialoguesList[int.Parse(args[0])];
                }
                break;
            case 2: //���������� ����������
                AddCharacters(args); // � ������� (id �����, ������ �������) ::: �������� �� ����� �������
                break;
            case 3:
                RemoveCharacters(args); // ������� ��������� �� ����� �� ��� id
                break;
            case 7://�������� ����������
                MoveCharacters(args);//�������� ����� � ������� (������� ��, ������� �����) :::�������� � ������� �� ������
                break;

        }
    }
    void MoveCharacters(List<string> args)
    {
        //������� ������� � ����������
        //������� ������� � ����������
        //������� ������� � ����������
        int movesCount = args.Count / 2;
        List<GameObject> charList = new List<GameObject>();
        for (int i = 0; i < movesCount; i++) // ���������� ���������� ��� ��������
        {
            charList.Add(positionSlots[int.Parse(args[i * 2])]);
            Debug.Log($"Prepare character in slot {int.Parse(args[i * 2])} to move");
        }

        for(int i = 0; i < charList.Count; i++)
        {
            positionSlots[int.Parse(args[i * 2])] = null;
            positionSlots[int.Parse(args[i * 2 + 1])] = charList[i];
            charList[i].transform.position = 
                characterPositions[int.Parse(args[i * 2 + 1])].position;
            Debug.Log($"Moved character to slot {args[i * 2 + 1]}");
        }
    }
    void AddCharacters(List<string> args)
    {
        //������� ������� � ����������
        //������� ������� � ����������
        //������� ������� � ����������
        int charCount = args.Count / 2;
        List<Character> charList = new List<Character>();
        for (int i = 0; i < charCount; i++) // ���������� ���������� ��� ��������
        {
            Character currentCharracter = null;
            foreach (var chara in charController.charList)
            {
                if (chara.idCharacter == int.Parse(args[i * 2]))
                {
                    currentCharracter = chara;
                    break;
                }
            }
            if (currentCharracter != null)
            {
                charList.Add(currentCharracter);

                Debug.Log($"Added character {currentCharracter.Name}");
            }
            else Debug.LogError($"Can't find character with id{args[i * 2]}");
        }
        for (int i = 0; i < charList.Count; i++)//����������� ���������� � ������ �����
        {
            GameObject charGO;
            charController.characterObjects.TryGetValue(charList[i], out charGO);

            if (charGO != null)
            {
                charGO.transform.SetParent(mainCanvas);
                charGO.transform.SetSiblingIndex(backgroundImage.transform.GetSiblingIndex() + 1);
                charGO.transform.localScale = Vector3.one;
                int pos = int.Parse(args[i * 2 + 1]) - 1;
                charGO.transform.position = characterPositions[pos].position;
                positionSlots[pos] = charGO;
                activeCharacters.Add(charGO);
            }
            else
            {
                Debug.LogError($"Can't find character GO as char {charList[i].Name}");
            }
        }
    }
    void RemoveCharacters(List<string> args)
    { //������� ������� � ����������
      //������� ������� � ����������
      //������� ������� � ����������
        int removeCount = args.Count;
        List<GameObject> charList = new List<GameObject>();
        for (int i = 0; i < removeCount; i++) // ���������� ���������� ��� ��������
        {
            foreach (var charGO in activeCharacters)
            {
                Character go = charController.characterObjects.FirstOrDefault(x => x.Value == charGO).Key;
                if(go.idCharacter == int.Parse(args[i]))
                {
                    charList.Add(charGO);
                    break;
                }
            }
        }
      
       for(int i = 0; i < positionSlots.Count; i++)
        {
            for(int j = 0; j < charList.Count; j++)
            {
                if (charList[j] == positionSlots[i])
                {
                    positionSlots[i] = null;
                    activeCharacters.Remove(charList[j]);
                    charList[j].transform.SetParent(charController.characterHolder);
                    charList[j].transform.localPosition = Vector3.zero;
                }
            }
        }
    
           
    }
    private IEnumerator ClickEvent()
    {
        DoScroll(); //������ ������ ��� �����
        yield return null;
    }

    private IEnumerator FillText(string line = null)
    {
        char[] asArray = line.ToArray();
        textComponent.text = "";
        isBusy = true;
            for (int i = 0; i < asArray.Length; i++)
            {
                if (isBusy) { 
                textComponent.text += asArray[i];
                yield return new WaitForSeconds(0.01f);
                }
                else yield return 0;

            }

        isBusy = false;
        yield return 0;
    }
    void ForceFillText(string line)
    {
        isBusy = false;
        StopCoroutine("FillText");
        
        textComponent.text = line;
    }
    // Update is called once per frame
   
}
