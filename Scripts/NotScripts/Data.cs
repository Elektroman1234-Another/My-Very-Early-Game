using System;
using System.Collections.Generic;

[Serializable]
public class CharacterSaveData
{
    public string characterName;
    public int currentHP;
}

[Serializable]
public class PartySaveData
{
    public List<CharacterSaveData> members = new List<CharacterSaveData>();
}