using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public float typingRate;

    private bool bTyping;
    private Queue<string> sentences;
    private string sentence;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        bTyping = false;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if(nameText != null)
           nameText.text = dialogue.name;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
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
            dialogueText.text = sentence;
            bTyping = false;
            return;
        }
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        bTyping = true;
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingRate);      // time setting;
        }
    }

    private void EndDialogue()
    {
        Debug.Log("End of conversation.");
    }
}
