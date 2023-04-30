using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public List<Level> Levels = new List<Level>();
}

[System.Serializable]
public class Level
{
    public string Title;
    public string Message;
}
