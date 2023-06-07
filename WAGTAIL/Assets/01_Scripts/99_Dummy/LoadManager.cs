using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using TMPro;
using System.Data;
using System;
using UnityEditor.Rendering;

public class LoadManager : Singleton<LoadManager>
{
    [System.Serializable]
    public class Scriptable
    {
        public int number;
        public int chapter;
        public string sayTarget;
        public string contents;

        public void Init(string[] str)
        {
            Debug.Log(str[0]);
            if (str[0] != "")
                number = int.Parse(str[0]);
            if (str[1] != "")
                chapter = int.Parse(str[1]);
            if (str[2] != "")
                sayTarget = str[2];
            if (str[3] != "")
                contents = str[3];
        }
    }
    [System.Serializable]
    public class ChapterScript : SerializableDictionary<int , Scriptable>
    {

    }



    //public GameObject
    public TextMeshProUGUI tmps;
    public ChapterScript dic_Say;
    public List<Scriptable> ChapterSay = new List<Scriptable>();

    protected override void Awake()
    {
        base.Awake();
        IO_GetSayer();
    }

    public void IO_GetSayer()
    {
        TextAsset _text = (TextAsset)Resources.Load("1");
        string testFile = _text.text;
        bool endOfFile = false;
        var data_values = testFile.Split('\n');
        int count1 = 0;
        while (!endOfFile)
        {
            Scriptable scriptable = new Scriptable();
            if (count1 == 0)
            {
                count1++;
                continue;
            }
            var data_value = data_values[count1].Split(',');
            if (data_value == null)
            {
                endOfFile = true;
                break;
            }
            if (data_value[0] == "")
            {
                endOfFile = true;
                break;
            }
            scriptable.Init(data_value);
            dic_Say.Add(int.Parse(data_value[0]), scriptable);
            count1++;
        }
    }

    private void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    Scriptable sc;
        //    dic_Say.TryGetValue(0, out sc);
        //    Typing(sc.contents, tmps);
        //}
    }

    public float timeForCharacter;

    public float timeForCharacter_Fast;

    float characterTime;

    string[] dialogsSave;
    TextMeshProUGUI tmpSave;

    public static bool isDialogEnd;

    bool isTypingEnd = false;
    int dialogNumber = 0;

    float timer;

    /// <summary>
    /// 현재 스테이지가 몇 번째 스테이지에 따라 호출.
    /// </summary>
    /// <param name="i"></param>
    public void CallCutScene(int i)
    {
        
    }

    public void Typing(string[] dialogs, TextMeshProUGUI textObj)
    {
        isDialogEnd = false;
        dialogsSave = dialogs;
        tmpSave = textObj;
        if(dialogNumber < dialogs.Length)
        {
            char[] chars = dialogs[dialogNumber].ToCharArray(); // 받아온 다이얼 로그를 char로 변환
            StartCoroutine(ITyper(chars , textObj));
        }
        else
        {
            tmpSave.text = "";
            isDialogEnd = true;
            dialogsSave = null;
            tmpSave = null;
            dialogNumber = 0;
        }
    }
    IEnumerator ITyper(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        isTypingEnd = false;

        while (currentChar < charLength)
        {
            if (timer >= 0)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
            else
            {
                textObj.text += chars[currentChar].ToString();
                currentChar++;
                timer = characterTime;  // 타이머 초기화
            }
        }
        if(currentChar >= charLength)
        {
            isTypingEnd = true;
            dialogNumber++;
            yield break;
        }
    }

    public void GetInputDown()
    {
        if(dialogsSave != null)
        {
            if (isTypingEnd)
            {
                tmpSave.text = "";
                Typing(dialogsSave, tmpSave);
            }
            else
            {
                characterTime = timeForCharacter_Fast; // 빠른 문장으로 넘김.
            }
        }
    }

    public void GetInputUp()
    {
        // 인풋이 끝나면?
        if(dialogsSave != null)
        {
            characterTime = timeForCharacter;
        }
    }

}
