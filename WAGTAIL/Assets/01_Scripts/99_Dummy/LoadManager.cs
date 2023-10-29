using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class LoadManager : Singleton<LoadManager>
{
    #region DialogueScriptes
    [System.Serializable]
    public class Scriptable
    {
        public int number;
        public int chapter;
        public string sayTarget;
        public string Korean;
        public string English;
        public string Japanese;
        public bool wait;

        public void Init(string[] str)
        {
            #region Omit
            try
            {
                if (str[0] != "")
                    number = int.Parse(str[0]);
                if (str[1] != "")
                    chapter = int.Parse(str[1]);
                if (str[2] != "")
                    sayTarget = str[2];
                if (str[3] != "")
                {
                    Korean = str[3].Replace('\\', '\n');
                    Korean = Korean.Replace('*', ',');
                }
                if (str[4] != "")
                {
                    English = str[4].Replace('\\', '\n');
                    English = English.Replace('*', ',');
                }
                if (str[5] != "")
                {
                    Japanese = str[5].Replace('\\', '\n');
                    Japanese = Japanese.Replace('*', ',');
                }
                }
            catch
            {
                Debug.Log($"{number} contents is null;");
            }
            #endregion
        }
    }
    #endregion

    #region Properties_N_Fields
    [System.Serializable]
    public class ChapterScript : SerializableDictionary<int, Scriptable>
    {

    }

    //===================================================
    /////               Fields                      /////
    //===================================================
    public int ChapterNum = 0;
    public TextMeshProUGUI Tmps;
    public TextMeshProUGUI NameTag;
    public ChapterScript Dic_Say;
    public Dialogue dialogue = new Dialogue();
    public bool isSpeedUp = false;
    public float KorTypingRate = 0.1f;
    public float EngTypingRate = 0.05f;
    public float JpTypingRate = 0.1f;
    public float CnTypingRate = 0.1f;

    private float typingRate = .1f;
    private Queue<string> sentences = new Queue<string>();
    private string sentence;
    private bool bTyping = false;
    #endregion

    //====================================================
    /////               magic Methods               /////
    //====================================================
    protected override void Awake()
    {
        base.Awake();
        IO_GetSayer();
        IO_GetScriptable();

        bTyping = false;
    }

    //====================================================
    /////               Core Methods                /////
    //====================================================
    public void IO_GetSayer()
    {
        #region Omit
        TextAsset _text = (TextAsset)Resources.Load("subtitleExcelFileCSV");
        string testFile = _text.text;
        bool endOfFile = false;
        var data_values = testFile.Split('\n');
        int count1 = 0;
        try
        {
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
        catch
        {
            Debug.LogWarning("설정된 CSV의 다음 대사가 존재하지 않음.");
        }
        #endregion
    }
    
    public Dialogue IO_GetScriptable(int num = 0)
    {
        #region Omit
        if (num == 0) num = ChapterNum;
        Scriptable sc;
        List<string> nametmp = new List<string>();
        List<string> temp = new List<string>();
        for (int i = 0; i < Dic_Say.Count; i++)
        {
            Dic_Say.TryGetValue(i, out sc);
            if (sc.chapter == num)
            {
                nametmp.Add(sc.sayTarget);
                temp.Add(Language(sc));
            }
            else if (sc.chapter > num) break;
        }
        dialogue = new Dialogue();
        dialogue.name = nametmp[0];
        dialogue.sentences = new string[temp.Count];

        for (int i = 0; i < temp.Count; i++) dialogue.sentences[i] = temp[i];

        return dialogue;
        #endregion
    }

    private string Language(Scriptable sc)
    {
        #region Omit        
        switch (UIManager.GetInstance().GetLanguageType)
        {
            case LanguageType.KOR:
                return sc.Korean;
            case LanguageType.ENG:
                return sc.English;
            case LanguageType.JP:
                return sc.Japanese;
            case LanguageType.CN:
                break;
        }
        throw new System.NotImplementedException();
        #endregion
    }

    public void StartDialogue(Dialogue dialogue)
    {
        #region Omit
        bTyping = false;
        sentences.Clear();

        if (NameTag != null) NameTag.text = dialogue.name;

        foreach (string sentence in dialogue.sentences) sentences.Enqueue(sentence);

        DisplayNextSentence();
        #endregion
    }

    public void DisplayNextSentence()
    {
        #region Omit
        if (bTyping)
        {
            StopAllCoroutines();
            Tmps.text = sentence;
            bTyping = false;
            return;
        }
        if (EndDialogue()) return;
        sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
        #endregion
    }

    private float GetLanguageRate()
    {
        #region Omit
        switch (UIManager.GetInstance().GetLanguageType)
        {
            case LanguageType.KOR:
                return KorTypingRate;
            case LanguageType.ENG:
                return EngTypingRate;
            case LanguageType.JP:
                return JpTypingRate;
            case LanguageType.CN:
                return CnTypingRate;
            default:
                break;
        }
        throw new System.NotImplementedException();
        #endregion
    }

    private IEnumerator TypeSentence(string sentence)
    {
        #region Omit    
        bool strType = false;
        string tmp = "";
        bTyping = true;
        Tmps.text = string.Empty;
        foreach (char letter in sentence.ToCharArray())
        {
            if (letter == '<' || strType == true)
            {
                strType = true;
                tmp += letter;
                if (letter == '>')
                {
                    strType = false;
                    Tmps.text += tmp;
                    tmp = "";
                }
                continue;
            }
            else
                Tmps.text += letter;
            if (isSpeedUp)
                yield return new WaitForSeconds(GetLanguageRate() * 0.5f);      // time setting;
            else if (!isSpeedUp)
                yield return new WaitForSeconds(GetLanguageRate());
        }
        bTyping = false;
        #endregion 
    }

    public bool EndDialogue()
    {
        if (sentences.Count == 0)
            return true;

        return false;
    }
    
    public void NameTagSet(TextMeshProUGUI tmp)
    {
        NameTag = tmp;
        NameTag.text = string.Empty;
    }

    public void TmpSet(TextMeshProUGUI tmp)
    {
        Tmps = tmp;
        Tmps.text = string.Empty;
    }
}
