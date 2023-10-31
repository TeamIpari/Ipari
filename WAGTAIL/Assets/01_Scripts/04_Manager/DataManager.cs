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
    
    // ���� ������ �����̸� ����
    private string _gameDataFileName = "GameData.json";
    
    // ���� ������ ����� Ŭ����
    public GameData data = new GameData();
    
    // �ҷ�����
    public bool LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + _gameDataFileName;
        
        // ����� ������ �ִٸ�
        if (File.Exists(filePath))
        {
            // ����� ������ �о��
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
        // ������ ������ Init
        data.Init();
        // Json���� ��ȯ
        string toJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + _gameDataFileName;
        
        // ���� ����
        File.WriteAllText(filePath, toJsonData);
        
#if UNITY_EDITOR
        Debug.Log("Save GameData Finish");
#endif
    }
}
