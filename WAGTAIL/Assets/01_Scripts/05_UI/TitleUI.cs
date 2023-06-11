using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public void GameStart()
    {
        SceneLoader.GetInstance().LoadScene("Chapter01");
    }
}
