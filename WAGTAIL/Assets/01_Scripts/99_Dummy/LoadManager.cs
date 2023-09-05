using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Rendering.PostProcessing;
using UnityEditor.Rendering;
using UnityEditor;
using DG.Tweening;

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
    public TextMeshProUGUI Tmps;
    public int TmpNum = 0;
    public ChapterScript Dic_Say;
    //public List<Scriptable> ChapterSay = new List<Scriptable>();
    public Dialogue dialogue = new Dialogue();
    public bool isSpeedUp = false;


    bool isTypingEnd = false;   // 치고 있는 상태인가?
    public float time = 0;
    int dialogNum;
    public float StandardTime;
    int FastForTime;

    private Queue<string> sentences = new Queue<string>();
    private string sentence;
    private bool bTyping = false;
    public float typingRate;

    protected override void Awake()
    {
        base.Awake();
        IO_GetSayer();
        IO_GetScriptable();

        isTypingEnd = true;
        bTyping = false;
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
    
    // 기록한 CSV의 내용을 찾아서 불러옴.
    public Dialogue IO_GetScriptable(int num = 0)
    {
        if (num == 0)
            num = ChapterNum;
        Scriptable sc;
        List<string> temp = new List<string>();
        for (int i = 0; i < Dic_Say.Count; i++)
        {
            Dic_Say.TryGetValue(i, out sc);
            if(sc.chapter == num)
            {
                temp.Add(sc.contents);
            }
            else if (sc.chapter > num)
            {
                break;
            }
        }
        dialogue = new Dialogue();
        dialogue.sentences = new string[temp.Count];
        for(int i = 0; i < temp.Count; i++)
        {
            dialogue.sentences[i] = temp[i];
        }
        dialogNum = 0;

        return dialogue;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        bTyping = false;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (bTyping)
        {
            StopAllCoroutines();
            Tmps.text = sentence;
            bTyping = false;
            return;
        }
        if (EndDialogue())
        {
            return;
        }
        sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        bTyping = true;
        Tmps.text = string.Empty;
        foreach (char letter in sentence.ToCharArray())
        {
            Tmps.text += letter;
            if (isSpeedUp)
                yield return new WaitForSeconds(typingRate * 0.5f);      // time setting;
            else if (!isSpeedUp)
                yield return new WaitForSeconds(typingRate);
        }
        bTyping = false;
    }

    public bool EndDialogue()
    {
        if (sentences.Count == 0)
            return true;
        //Debug.Log("End of conversation.");
        return false;
    }

    public void ResetValue()
    {
        TmpNum = 0;
        dialogNum = 0;
    }

    public void TmpSet(TextMeshProUGUI tmp)
    {
        Tmps = tmp;
        Tmps.text = string.Empty;
    }
    
    //public void AddTmp(TextMeshProUGUI tmp)
    //{
    //    Tmps = tmp;
    //}


    //private IEnumerator TypingCo(Scriptable sc)
    //{
    //    int dialogNum = 0;          // 몇 번째 문장 실행 중.
    //    int dialogMax = sc.contents.Length - 1;
    //    isTypingEnd = false;
    //    while (dialogNum < dialogMax)
    //    {
    //        if(time >= 0)
    //        {
    //            yield return null;
    //            time -= Time.deltaTime;
    //        }
    //        else
    //        {
    //            if (dialogNum > 0 
    //                && sc.contents[dialogNum - 1].ToString() == ".")
    //                Tmps.text += "\n";
    //            Tmps.text += sc.contents[dialogNum].ToString();
    //            //if (Tmps[TmpNum].text.Length % 26 == 0 )
    //            //    Tmps[TmpNum].text += "\n";

    //            dialogNum++;
    //            time = StandardTime;
    //        }
    //    }
    //    if(dialogNum >= dialogMax)
    //    {
    //        if(Tmps.Count > 0 && Tmps.Count > TmpNum)
    //        {
    //            TmpNum++;
    //        }
    //        if(TmpNum >= Tmps.Count)
    //        {
    //            TmpNum = 0;
    //        }
    //        isTypingEnd = true;
    //        this.dialogNum++;
    //        yield break;
    //    }
    //}
}
