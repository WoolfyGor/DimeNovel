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
    public Scenario scenario; // Сценарий под игру (содержит все диалоги игры)
    public Dialogue currentDialogue; // Текущий воспроизводимый сценарий
    public int lineId = 1; // индекс текущей строки
    CharacterController charController; // Ссылка на контроллер персонажей (для ссылок)
    public List<GameObject> activeCharacters = new List<GameObject>();//активные персонажи на основном экране
    [Header("Scene objects")]
    public Transform mainCanvas; // Канвас для назначения ему персонажей
    public Image backgroundImage; // для мены фонов
    public List<Transform> characterPositions = new List<Transform>(); // ссылки на позиции на сцене
    public List<GameObject> positionSlots = new List<GameObject>(5); // занятые слоты персонажей для перемещения через экшены
    public GameObject textArea; // поле под текст (картинка)
    public TMPro.TMP_Text textComponent; // основной текст диалога
    public TMPro.TMP_Text nameComponent; // имя говорящего

    bool isBusy;//Занята ли программа (выводом текста)
    // Start is called before the first frame update
    void Start()
    {
        charController = CharacterController.instance;
        GameWorker();//Заменить на норм контроллер диалогов в игре
    }
    public void GameWorker()
    {
        StartCoroutine("DialogueLiner"); // Запускает основную корутину на отслеживание кликов
        DoScroll(); // делает стартовую прокрутку
    }

    IEnumerator DialogueLiner()
    {
        currentDialogue = scenario.dialoguesList[0]; // запускаем первый в списке диалог :: ЗАМЕНИТЬ НА ПОСЛЕДНИЙ НАЧАТЫЙ
        while (enabled) // активна всегда
        {
            if (Input.GetMouseButtonDown(0)) yield return ClickEvent(); // Если была нажата ЛКМ, вызывается функция одиночного клика
            yield return null;
        }
    }

    void DoScroll()
    {
        if (isBusy) // если текст уже выводится, то заполним текстовое поле силой и сделаем шаг назад, пропустив скролл
        {
            ForceFillText(currentDialogue.dialogueLines[lineId-1].what);
            return;
        }

        if (lineId < currentDialogue.dialogueLines.Count) {//Условие истинна, пока строки диалога в диалоге не закончены 
            if (backgroundImage.sprite != currentDialogue.currentBg.image)// Проверка на замену фона (нужна ли?)
            {
                backgroundImage.sprite = currentDialogue.currentBg.image;
                Debug.Log($"Renew bg to {currentDialogue.currentBg.backgroundName}");
            }
            if (currentDialogue.dialogueLines[lineId].action.idAction == 0) // Если нет действия - запускаем заливку текста и обновляем имя
            {
                nameComponent.text = currentDialogue.dialogueLines[lineId].whom.Name;
                StartCoroutine(FillText(currentDialogue.dialogueLines[lineId].what));
                lineId++;
            }
            else
            {//Иначе запускаем экшен :: РАСКИДАТЬ ПО ПЕРЕМЕННЫМ
                DoAction(currentDialogue.dialogueLines[lineId].action.idAction, currentDialogue.dialogueLines[lineId].action.attributes);
                lineId++;
                DoScroll(); // Прокручиваем скролл ещё раз, потому что экшены (в данный момент) воспроизводятся моментально
            }
        }
        else
        {
            Debug.Log("Reached end of dialogue"); // Сделать смену на следующий трек диалога
        }


    }
    void DoAction(int id, List<string> args)
    {
        switch (id)
        {
            case 1: // смена диалога
                if (int.Parse(args[0]) != currentDialogue.idDialogue)
                { // Должно менять текущий диалог на тот, что указан в атрибуте экшена
                    currentDialogue = scenario.dialoguesList[int.Parse(args[0])];
                }
                break;
            case 2: //добавление персонажей
                AddCharacters(args); // в формате (id перса, индекс позиции) ::: ЗАМЕНИТЬ НА НОМЕР ПОЗИЦИИ
                break;
            case 3:
                RemoveCharacters(args); // удаляет персонажа со сцены по его id
                break;
            case 7://Сдвинуть персонажей
                MoveCharacters(args);//Сдвигает перса в формате (Позиция до, позиция после) :::ЗАМЕНИТЬ С ИНДЕКСА НА НОМЕРЫ
                break;

        }
    }
    void MoveCharacters(List<string> args)
    {
        //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        int movesCount = args.Count / 2;
        List<GameObject> charList = new List<GameObject>();
        for (int i = 0; i < movesCount; i++) // подготовка персонажей для переноса
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
        //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        int charCount = args.Count / 2;
        List<Character> charList = new List<Character>();
        for (int i = 0; i < charCount; i++) // подготовка персонажей для переноса
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
        for (int i = 0; i < charList.Count; i++)//Перемещение персонажей к центру сцены
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
    { //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
      //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
      //ВЫНЕСТИ ИНДЕКСЫ В ПЕРЕМЕННЫЕ
        int removeCount = args.Count;
        List<GameObject> charList = new List<GameObject>();
        for (int i = 0; i < removeCount; i++) // подготовка персонажей для переноса
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
        DoScroll(); //Делает скролл при клике
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
