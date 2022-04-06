using System;
using System.IO;
using UnityEngine;

[Serializable]
public class Report
{
    public string version = "0.0.1";
    public string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public double epoch = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public string eventKey;
    public string levelID;
    public string userID;
    public Report(string userID, string eventKey, string levelID)
    {
        this.userID = userID;
        this.eventKey = eventKey;
        this.levelID = levelID;
    }
}

public static class WinsuranceAnalytics
{
    private const string path = "Assets/Logs/userLog.json";
    private static readonly StreamWriter Writer = new(path, true);

    public static void ReportEvent(string userId, string eventKey, string levelID)
    {
        Writer.WriteLine(JsonUtility.ToJson(new Report(userId, eventKey, levelID)));
        Writer.Flush();
    }
    
    public static void Close() => Writer.Close();
}
    

