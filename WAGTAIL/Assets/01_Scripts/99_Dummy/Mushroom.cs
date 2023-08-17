using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public GameObject[] mush;
    public int cur = 0;
    public bool isMush;
    public float curTime = 0;
    public float targetTime = 0;
    public float firsttime;
    public float secondstime;
    // Start is called before the first frame update
    void Start()
    {
        isMush = true;
        for(int i = 1; i < mush.Length; i++) { mush[i].SetActive(false); }
        targetTime = firsttime;
        curTime = targetTime;
    }

    public void ChangeMushroom()
    {
        mush[cur++].SetActive(false);
        if (cur >= mush.Length)
            cur = 0;
        mush[cur].SetActive(true);
        curTime = 0;

    }

    public void CallAnim()
    {
        if (isMush)
        {
            if (targetTime > curTime)
            {
                curTime += Time.deltaTime;
            }
            else
            {
                ChangeMushroom();
                isMush = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CallAnim();
    }
}
