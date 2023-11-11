using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionCocosiUI : MonoBehaviour
{
    [HideInInspector] public GameObject[][] cocosiUI;
    [HideInInspector] public GameObject[] canvas = new GameObject[3];
    [HideInInspector] public GameObject currentCanvas; 
    // Start is called before the first frame update
    void Awake()
    {
        for(int i = 0; i < canvas.Length; i++)
        {
            canvas[i] = transform.GetChild(i).gameObject;
        }
        
        cocosiUI = new GameObject[3][];
        cocosiUI[0] = new GameObject[5];
        cocosiUI[1] = new GameObject[3];
        cocosiUI[2] = new GameObject[3];

        for (int i = 0; i < cocosiUI[0].Length; i++)
        {
            cocosiUI[0][i] = canvas[0].transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).gameObject;
        }
        
        for (int i = 0; i < cocosiUI[1].Length; i++)
        {
            cocosiUI[1][i] = canvas[1].transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).gameObject;
        }
        
        for(int i =0;i<cocosiUI[2].Length;i++)
        {
            cocosiUI[2][i] = canvas[2].transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).gameObject;
        }

        for (int i = 0; i < canvas.Length; i++)
        {
            canvas[i].SetActive(false);
        }
    }
    
    public void SetCocosiUI(int chapter, int index, bool isOn)
    {
        cocosiUI[chapter][index].SetActive(isOn);
    }

    public void SetCanvas(int index, bool isOn)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == index)
            {
                canvas[index].SetActive(isOn);
                currentCanvas = canvas[index];
            }
            else canvas[i].SetActive(false);
        }
    }
    
}
