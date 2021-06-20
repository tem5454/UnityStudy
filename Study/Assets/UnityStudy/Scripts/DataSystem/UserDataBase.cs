using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataBase
{
    
}

[System.Serializable]
public class UserData : UserDataBase
{
    public long level;
    public long exp;
    public long currentHP;
}