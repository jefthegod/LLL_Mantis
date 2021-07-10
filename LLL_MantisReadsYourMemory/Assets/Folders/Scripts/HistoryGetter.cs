using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;
using System;
using EasyButtons;

public class HistoryGetter
{
    public static string chromeDefaultHistoryDir =>  @"C:\Users\" + GetUsername() + @"\AppData\Local\Google\Chrome\User Data\Default\History";
    public static string braveDefaultHistoryDir =>  @"C:\Users\" + GetUsername() + @"\AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\History";

    public static string chromeHistoryCopyDir = Application.streamingAssetsPath + "/" + "ChromeHistory";
    public static string braveHistoryCopyDir = Application.streamingAssetsPath + "/" + "BraveHistory";

    public static List<string> historyCopies;

    static string GetUsername()
    {
        return Environment.UserName;
    }

    public static List<string> CopyHistoryFilesThatExist()
    {
        historyCopies = new List<string>();

        if (System.IO.File.Exists(braveDefaultHistoryDir))
        {
            Debug.Log("Brave History Exists");
            CopyBraveHistoryFile();
            historyCopies.Add(braveHistoryCopyDir);
        }
        
        if (System.IO.File.Exists(chromeDefaultHistoryDir))
        {
            Debug.Log("Chrome History Exists");
            CopyChromeHistoryFile();
            historyCopies.Add(chromeHistoryCopyDir);
        }

        return historyCopies;
    }

    public static void CopyChromeHistoryFile()  // DRY VIOLATION 😭😭😭😭😭
    {
        try
        {
            File.Copy(chromeDefaultHistoryDir, chromeHistoryCopyDir, true);
        }
        catch (IOException iox)
        {
            Debug.Log(iox.Message);
        }
    }

    public static void CopyBraveHistoryFile()
    {
        try
        {
            File.Copy(braveDefaultHistoryDir, chromeHistoryCopyDir, true);
        }
        catch (IOException iox)
        {
            Debug.Log(iox.Message);
        }
    }
}