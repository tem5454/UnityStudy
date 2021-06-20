using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataModel : SingletonBase<GameDataModel>
{
    /// <summary> Game Data Model Initialize State Flag  </summary>
    public bool IsInitialized { get; private set; } = false;

    [System.Serializable]
    public class DictionaryOfGameDataAndDataBase : SerializableDictionary<GameDataList, GameDataBase> { }

    [System.Serializable]
    public class DictionaryOfGameDataAndTextAsset : SerializableDictionary<GameDataList, string> { }

    /// <summary> Game Data Container </summary>
    public DictionaryOfGameDataAndDataBase gameDataContainer = new DictionaryOfGameDataAndDataBase();

    /// <summary> Game Data Text Asset Contaier </summary>
    public DictionaryOfGameDataAndTextAsset gameDataTextAsset = new DictionaryOfGameDataAndTextAsset();


    public CharacterBase CharacterBase;



    [ContextMenu("Initialize")]
    public void Initialize()
    {
        InitializeGameData();
    }

    /// <summary> Game Data Initialize Coroutine </summary>
    public void InitializeGameData()
    {
        // Initialize Start
        IsInitialized = false;

        // Game Data Initialize Start Log
        Debug.Log("Game Data Model Initialize Start");

        // Initialize All Container
        for (int index = 0; index < (int)GameDataList.End; index++)
        {
            gameDataContainer[(GameDataList)index] = null;
            gameDataTextAsset[(GameDataList)index] = string.Empty;
        }

        foreach (var data in gameDataContainer)
        {
            // Load Game Data Text Asset
            LoadGameDataTextAsset(data.Key);

            // Get Target Field Type
            System.Type targetType = GetFieldType(data.Key.ToString());

            // Get Target Data Field Exception Check
            if (targetType == null) { Debug.LogErrorFormat("Could not Find Target Field : {0}", data.Key); continue; }

            // Try Convert Target Data
            GameDataBase readGameData = null;
            try
            {
                readGameData = JsonConvert.DeserializeObject(gameDataTextAsset[data.Key], targetType) as GameDataBase;
            }
            catch (JsonReaderException error)
            {
                Debug.LogErrorFormat("Data Type : {0} - JsonReaderException Catched : {1}", data.Key.ToString(), error);
            }

            // Target Data Field Null Exception Check
            if (readGameData == null) { Debug.LogErrorFormat("Data Field is Null : {0}", data.Key); continue; }

            // Set Target Field Data
            SetFieldValue(data.Key.ToString(), readGameData);
        }

        // Initialize Complete
        Debug.Log("Game Data Model Initialize Complete");
        IsInitialized = true;
    }

    /// <summary> Load Game Data Text Asset </summary>
    void LoadGameDataTextAsset(GameDataList dataType)
    {
        if (dataType == GameDataList.None || dataType == GameDataList.End)
        {
            return;
        }

        TextAsset text = Resources.Load<TextAsset>("Data/" + dataType.ToString());
        if(text == null || string.IsNullOrEmpty(text.text))
        {
            return;
        }
        
        // Input Loaded Data Result
        gameDataTextAsset[dataType] = text.text;
    }

    public System.Type GetFieldType(string fieldName)
    {
        System.Reflection.FieldInfo fieldInfo = GameDataModel.Instance.GetType().GetField(fieldName);
        if (fieldInfo != null)
            return fieldInfo.FieldType;

        return null;
    }

    public void SetFieldValue(string fieldName, object data)
    {
        var type = GameDataModel.Instance.GetType();
        var field = type.GetField(fieldName);
        field.SetValue(GameDataModel.Instance, data);
    }
}
