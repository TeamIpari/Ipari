//using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using TMPro;
using System.Data;
using System;
using UnityEditor.Rendering;
using JetBrains.Annotations;
using System.Linq;

public class LoadManager : Singleton<LoadManager>
{
    [System.Serializable]
    public class Scriptable
    {
        public int number;
        public int chapter;
        public string sayTarget;
        public string contents;
        public bool wait;

        public void Init(string[] str)
        {
            if (str[0] != "")
                number = int.Parse(str[0]);
            if (str[1] != "")
                chapter = int.Parse(str[1]);
            if (str[2] != "")
                sayTarget = str[2];
            if (str[3] != "")
                wait = int.Parse(str[3]) == 0 ? false : true;
            if (str[4] != "")
                contents = str[4];
        }
    }
    [System.Serializable]
    public class ChapterScript : SerializableDictionary<int , Scriptable>
    {

    }

    //public GameObject
    public int ChapterNum = 0;
    public List<TextMeshProUGUI> Tmps;
    public int TmpNum = 0;
    public ChapterScript Dic_Say;
    public List<Scriptable> ChapterSay = new List<Scriptable>();



    bool isTypingEnd = false;   // 치고 있는 상태인가?
    float time = 0;
    int dialogNum;
    public float StandardTime;
    int FastForTime;

    protected override void Awake()
    {
        base.Awake();
        IO_GetSayer();
        IO_GetScriptable();

        isTypingEnd = true;
        //dialogsSave = sc.contents;
        //GetInputUp();
        //tmpSave = tmps;
        //isTypingEnd = true;
    }

    public void IO_GetSayer()
    {
        TextAsset _text = (TextAsset)Resources.Load("subtitleExcelFileCSV");
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
            Dic_Say.Add(int.Parse(data_value[0]), scriptable);
            count1++;
        }
    }
    
    public void IO_GetScriptable(int num = 0)
    {
        if (num == 0)
            num = ChapterNum;
        Scriptable sc;
        ChapterSay.Clear();
        //dic_Say.TryGetValue(chapterNum, out sc);
        for (int i = 0; i < Dic_Say.Count; i++)
        {
            Dic_Say.TryGetValue(i, out sc);
            if(sc.chapter == num)
            {
                ChapterSay.Add(sc);
            }
            else if (sc.chapter > num)
            {
                break;
            }
        }
        dialogNum = 0;
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0) && dialogNum < ChapterSay.Count)
        //{
        //    PlayTyping();
        //}
        //else
        //{
        //    //Tmps[TmpNum]
        //}

    }

    public bool IsSayEnding()
    {
        if (dialogNum < ChapterSay.Count)
            return false;
        return true;
    }
    public void SearchTypePoint(int num = 0)
    {
        IO_GetScriptable(num);
    }

    public void TmpSet(TextMeshProUGUI tmp)
    {
        Tmps.Clear();
        Tmps.Add(tmp);
    }
    
    public void AddTmp(TextMeshProUGUI tmp)
    {
        Tmps.Add(tmp);
    }

    public void PlayTyping()
    {
        //if(tmp == null)

        if (isTypingEnd)        
        {
            if (TmpNum == 0)
            {
                Tmps[Tmps.Count - 1].enabled = false;
            }
            else
            {
                Tmps[TmpNum - 1].enabled = false;
            }
            Tmps[TmpNum].enabled = true;
            Tmps[TmpNum].text = "";

            StartCoroutine(TypingCo(ChapterSay[dialogNum]));
        }
        else
        {
            StopAllCoroutines();
            Tmps[TmpNum].text = ChapterSay[dialogNum].contents;
            dialogNum++;
            isTypingEnd = true;
            if (Tmps.Count > 0 && Tmps.Count > TmpNum)
            {
                TmpNum++;
            }
            if (TmpNum >= Tmps.Count)
            {
                TmpNum = 0;
            }
        }
    }

    private void GetTyping()
    {

    }

    private IEnumerator TypingCo(Scriptable sc)
    {
        int dialogNum = 0;          // 몇 번째 문장 실행 중.
        int dialogMax = sc.contents.Length - 1;
        isTypingEnd = false;
        while (dialogNum < dialogMax)
        {
            if(time >= 0)
            {
                yield return null;
                time -= Time.deltaTime;
            }
            else
            {
                if (dialogNum > 0 
                    && sc.contents[dialogNum - 1].ToString() == ".")
                    Tmps[TmpNum].text += "\n";
                Tmps[TmpNum].text += sc.contents[dialogNum].ToString();
                if (Tmps[TmpNum].text.Length % 26 == 0 )
                    Tmps[TmpNum].text += "\n";

                dialogNum++;
                time = StandardTime;
            }
        }
        if(dialogNum >= dialogMax)
        {
            if(Tmps.Count > 0 && Tmps.Count > TmpNum)
            {
                TmpNum++;
            }
            if(TmpNum >= Tmps.Count)
            {
                TmpNum = 0;
            }
            isTypingEnd = true;
            this.dialogNum++;
            yield break;
        }
    }

    //public float timeForCharacter;

    //public float timeForCharacter_Fast;

    //[SerializeField] float characterTime;

    //[SerializeField] string dialogsSave;
    //TextMeshProUGUI tmpSave;

    //public static bool isDialogEnd;

    //[SerializeField] bool isTypingEnd = false;
    //int dialogNumber = 0;

    //float timer;

    ///// <summary>
    ///// 현재 스테이지가 몇 번째 스테이지에 따라 호출.
    ///// </summary>
    ///// <param name="i"></param>
    //public void CallCutScene(int i)
    //{
        
    //}

    //public void Typing(string dialogs, TextMeshProUGUI textObj)
    //{
    //    isDialogEnd = false;
    //    dialogsSave = dialogs;
    //    tmpSave = textObj;
    //    if(dialogNumber < dialogs.Length)
    //    {
    //        char[] chars = dialogs.ToCharArray(); // 받아온 다이얼 로그를 char로 변환
    //        StartCoroutine(ITyper(chars , textObj));
    //    }
    //    else
    //    {
    //        tmpSave.text = "";
    //        isDialogEnd = true;
    //        dialogsSave = null;
    //        tmpSave = null;
    //        dialogNumber = 0;
    //    }
    //}
    //IEnumerator ITyper(char[] chars, TextMeshProUGUI textObj)
    //{
    //    int currentChar = 0;
    //    int charLength = chars.Length;
    //    isTypingEnd = false;

    //    while (currentChar < charLength)
    //    {
    //        if (timer >= 0)
    //        {
    //            yield return null;
    //            timer -= Time.deltaTime;
    //        }
    //        else
    //        {
    //            textObj.text += chars[currentChar].ToString();
    //            currentChar++;
    //            timer = characterTime;  // 타이머 초기화
    //        }
    //    }
    //    if(currentChar >= charLength)
    //    {
    //        isTypingEnd = true;
    //        dialogNumber++;
    //        yield break;
    //    }
    //}

    //public void GetInputDown()
    //{
    //    if(dialogsSave != null)
    //    {
    //        if (isTypingEnd)
    //        {
    //            tmpSave.text = "";
    //            Typing(dialogsSave, tmpSave);
    //        }
    //        else
    //        {
    //            characterTime = timeForCharacter_Fast; // 빠른 문장으로 넘김.
    //            Debug.Log(characterTime);
    //        }
    //    }
    //}

    //public void GetInputUp()
    //{
    //    // 인풋이 끝나면?
    //    if(dialogsSave != null)
    //    {
    //        characterTime = timeForCharacter;
    //    }
    //}

}
