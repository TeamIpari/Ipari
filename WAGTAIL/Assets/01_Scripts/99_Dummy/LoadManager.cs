using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public bool isSpeedUp = false;


    bool isTypingEnd = false;   // 치고 있는 상태인가?
    public float time = 0;
    int dialogNum;
    public float StandardTime;
    int FastForTime;

    protected override void Awake()
    {
        base.Awake();
        IO_GetSayer();
        IO_GetScriptable();

        isTypingEnd = true;
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

    //private void Update()
    //{
    //    //if (Input.GetMouseButtonDown(0) && dialogNum < ChapterSay.Count)
    //    //{
    //    //    PlayTyping();
    //    //}
    //    //else
    //    //{
    //    //    //Tmps[TmpNum]
    //    //}

    //}

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

    public void ResetValue()
    {
        TmpNum = 0;
        dialogNum = 0;
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
            Debug.Log("TmpNum = " + TmpNum + "// Dialong = " + dialogNum);
            if (dialogNum >= ChapterSay.Count)
                dialogNum = 0;
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
                //if (Tmps[TmpNum].text.Length % 26 == 0 )
                //    Tmps[TmpNum].text += "\n";

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
}
