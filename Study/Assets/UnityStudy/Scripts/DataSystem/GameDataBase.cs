using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataBase : ScriptableObject
{
    public long version;
}

[System.Serializable]
public class CharacterBase : GameDataBase
{
    [System.Serializable]
    public class CharacterData
    {
        public long level;
        public long exp;
        public long maxHP;
        public long maxStamina;
    }

    public List<CharacterData> data;
}
