using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataModel : SingletonBase<UserDataModel>
{
    ///<summary> UserData Dictionary Container </summary>
    private Dictionary<UserDataList, object> UserData = new Dictionary<UserDataList, object>();

    /// <summary> Public Generical Get Data Function </summary>
    public T GetData<T>(UserDataList type) where T : UserDataBase, new()
    {
        if (UserData[type] == null) UserData[type] = LoadData<T>(type);
        return UserData[type] as T;
    }

    /// <summary> Public Generical Set Data Function </summary>
    public bool SetData<T>(UserDataList type, T data) where T : UserDataBase, new()
    {
        if (UserData[type] is T && data != null) UserData[type] = data;
        return SaveData<T>(type);
    }

    /// <summary> UserDataModel Each Data Load </summary>
    public void Initialize()
    {
        // To do : Each UserData Load
        UserData[UserDataList.UserData] = LoadData<UserData>(UserDataList.UserData);          // Player Data
    }

    /// <summary> UserData Load - Typeof(T) </summary>
    public static T LoadData<T>(UserDataList dataType) where T : UserDataBase, new()
    {
        string key = "UnityStudy";
        string strData = PlayerPrefs.GetString(key);

        // String Data Empty Check
        if (string.IsNullOrEmpty(strData))
        {
            return new T();
        }

        // Deserialized Data Null Exception Check
        if (!(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strData) is T deserializeData))
        {
            return new T();
        }

        // Set Property Data To UserData[Data Type]
        Instance.UserData[dataType] = deserializeData;

        return deserializeData;
    }

    /// <summary> UserData Save - Typeof(T) </summary>
    public static bool SaveData<T>(UserDataList dataType) where T : UserDataBase
    {
        // Type Check
        if (false == (UserDataModel.Instance.UserData[dataType] is T))
        {
            return false;
        }

        // Convert UserData Type
        T userData = UserDataModel.Instance.UserData[dataType] as T;

        // Json Converting
        string convertData = string.Empty;
        convertData = Newtonsoft.Json.JsonConvert.SerializeObject(userData);

        // Converted String Null & Empty Check
        if (string.IsNullOrEmpty(convertData))
            return false;

        string key = "UnityStudy";
        PlayerPrefs.SetString(key, convertData);        

        return true;
    }

}
