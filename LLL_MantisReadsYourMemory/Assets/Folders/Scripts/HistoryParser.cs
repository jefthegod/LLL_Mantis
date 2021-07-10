using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using Newtonsoft.Json;

public class KeywordResult
{
    public string name;
    public int instances;
}

public class HistoryParser
{
    public static List<KeywordResult> GetSearchTermsOfAllBrowsers()
    {
        List<KeywordResult> masterKeyWordMatches = new List<KeywordResult>();
        List<string> historyLocations = HistoryGetter.CopyHistoryFilesThatExist();

        for (int i = 0; i < historyLocations.Count; ++i)
        {
            List<KeywordResult> searchTermsWeMeet = GetSearchTerms(historyLocations[i]);

            foreach (var kr in searchTermsWeMeet)
            {
                foreach (var mkr in masterKeyWordMatches)
                {
                    if (kr.name == mkr.name)    // if already exists in Master List
                    {
                        mkr.instances += kr.instances;
                        continue;
                    }
                }
                masterKeyWordMatches.Add(kr);
            }
        }

        return masterKeyWordMatches;
        // Foreach history file we copied ✔
        // Get matched terms    ✔
        // combine with the master list to avoid reduntent entries. ✔
    }
    
    public static List<KeywordResult> GetSearchTerms(string path)
    {
        var searchTerms = GetJSONSearchTerms().Searches;
        List<KeywordResult> searchTermsWeMatch = new List<KeywordResult>();

        // Open Database
        string connection = "URI=file:" + path;
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        for (int i = 0; i < searchTerms.Count; ++i)
        {
            IDbCommand cmnd_read = dbcon.CreateCommand();
            IDataReader reader;

            string query = GetQueryString(searchTerms[i]);

            cmnd_read.CommandText = query;
            reader = cmnd_read.ExecuteReader();

            int count = 0;
            while (reader.Read())
                ++count;

            if (count > 0)
            {
                KeywordResult kr = new KeywordResult();
                kr.instances = count;
                kr.name = searchTerms[i].title;
                searchTermsWeMatch.Add(kr);
            }
        }
        return searchTermsWeMatch;
    }

    public static List<KeywordResult> GetSearchTermsWeMeetChrome()
    {
        HistoryGetter.CopyChromeHistoryFile();

        var searchTerms = GetJSONSearchTerms().Searches;
        List<KeywordResult> searchTermsWeMatch = new List<KeywordResult>();

        // Open Database
        string connection = "URI=file:" + HistoryGetter.chromeHistoryCopyDir;
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        for (int i = 0; i < searchTerms.Count; ++i)
        {
            IDbCommand cmnd_read = dbcon.CreateCommand();
            IDataReader reader;
            
            string query = GetQueryString(searchTerms[i]);

            cmnd_read.CommandText = query;
            reader = cmnd_read.ExecuteReader();
            

            int count = 0;
            while (reader.Read())
            {
                ++count;
            }

            if (count > 0)
            {
                KeywordResult kr = new KeywordResult();
                kr.instances = count;
                kr.name = searchTerms[i].title;
                searchTermsWeMatch.Add(kr);
            }
        }
        return searchTermsWeMatch;
    }

    void GetURLSFromChromeHistory() // should delete, but I'm keeping for a nice reference
    {
        HistoryGetter.CopyChromeHistoryFile();

        // Open Database
        string connection = "URI=file:" + HistoryGetter.chromeHistoryCopyDir;
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        // Read and print all values in table
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM urls WHERE url OR title LIKE '%pornhub.com%';";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        int count = 0;
        while (reader.Read())
        {
            Debug.Log("url: " + reader[1].ToString());
            Debug.Log("title: " + reader[2].ToString());
            ++count;
        }

        // Close connection
        dbcon.Close();
    }


    static string GetQueryString(Search search)
    {
        string baseQuery = "SELECT * FROM urls WHERE title LIKE ";
        var keywords = search.keywords;
        string query = baseQuery;

        query += "'%" + keywords[0] + "%'";

        for (int i = 1; i < keywords.Count; ++i)
        {
            query += " AND title like ";
            query += "'%" + keywords[i] + "%'";
        }
        return query;
    }

    static HistorySearchTermsJson GetJSONSearchTerms()
    {
        var sr = new StreamReader(Application.streamingAssetsPath + "/" + "SearchTerms.json");
        HistorySearchTermsJson myDeserializedClass = JsonConvert.DeserializeObject<HistorySearchTermsJson>(sr.ReadToEnd());
        sr.Close();
        return myDeserializedClass;
    }
}