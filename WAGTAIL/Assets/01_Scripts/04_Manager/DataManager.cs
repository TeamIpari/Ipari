using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    private static GameObject _container;
    
    private static DataManager _instance;

    public static DataManager Instance
    {
        get
        {
            if (!_instance)
            {
                _container = new GameObject();
                _container.name = "DataManager";
                _instance = _container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(_container);
            }
            return _instance;
        }
    }
    
    // 게임 데이터 파일이름 설정
    private string _gameDataFileName = "GameData.json";
    
    // 게임 데이터 저장용 클래스
    public GameData data = new GameData();
    
    // 불러오기
    public bool LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + _gameDataFileName;
        
        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일을 읽어옴
            string fromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(fromJsonData);
            data.Load();
#if UNITY_EDITOR
            Debug.Log("Load GameData Finish");
#endif
            return true;
        }

        else
        {
            return false;
        }
    }

    public void SaveGameDate()
    {
        // 저장할 데이터 Init
        data.Init();
        // Json으로 변환
        string toJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + _gameDataFileName;
        
        // 파일 저장
        File.WriteAllText(filePath, toJsonData);
        
#if UNITY_EDITOR
        Debug.Log("Save GameData Finish");
#endif
    }
}
