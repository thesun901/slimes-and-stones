using System.Collections.Generic;

[System.Serializable]
public class RuneSaveData
{
    public List<string> collectedRuneIDs = new List<string>();
    public List<string> activeRuneIDs = new List<string>(); // up to 3 entries
}
