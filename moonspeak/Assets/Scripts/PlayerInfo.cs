using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

[Serializable]
public class User
{
    public string username { get; set; }
    public Dictionary<string, int> scores { get; set; }
    public override string ToString()
    {
        string output = username + "\n";
        if (scores == null) scores = new Dictionary<string, int>();
        if (scores.Count == 0) return output + "Scores is Empty!\n";
        foreach (KeyValuePair<string, int> kvp in scores) output += $" - {kvp.Key}: {kvp.Value}\n";
        return output;
    }
}

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo playerInfo;
    public string username;
    public User user;

    private static User getUser(string username)
    {
        User user = null;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Regex.Replace($"https://moonspeak-giraffesyo.vercel.app/user/{username}", @"[^\x00-\x7F]+", ""));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader readStream = new StreamReader(response.GetResponseStream());
        string jsonResponse = readStream.ReadToEnd();
        user = JsonConvert.DeserializeObject<User>(jsonResponse);
        return user;
    }

    async void Awake()
    {
        Debug.Log("hi");
        playerInfo = this;

        DontDestroyOnLoad(this);
        username = "";
        // comment out below line to force login everytime
        username = PlayerPrefs.GetString("username");

        // if we're on the splash screen, move to the right scene based on if we're logged in
        await RemoteAssetLoader.Instance.LoadAllAssets();
        if (SceneManager.GetActiveScene().name == "Splash")
        {
            // TODO: We can put our team logo on this and load the new scene after 1-2 seconds

            if (string.IsNullOrWhiteSpace(username))
            {
                // no player found, send to login
                SceneLoader.LoadScene("login");
            }
            else
            {
                // already have a player, send to main menu
                SceneLoader.LoadScene("menu");
            }
        }
    }

    public void SetPlayer(string username)
    {
        this.username = username;
        PlayerPrefs.SetString("username", username);
        user = getUser(username);
        Debug.Log(user.ToString());
    }

    public void UpdateScore(string key, int delta)
    {
        string output = $"key={key}, delta={delta}";
        if (user.scores.ContainsKey(key)) user.scores[key] += delta;
        else user.scores[key] = delta;
        Debug.Log(user.ToString());
    }

    public void PostDB()
    {
        if (user == null) return;
        if (user.scores == null) return;
        if (string.IsNullOrWhiteSpace(user.username)) return;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Regex.Replace("https://moonspeak-giraffesyo.vercel.app/user/", @"[^\x00-\x7F]+", ""));
        request.ContentType = "application/json; charset=utf-8";
        request.Method = "POST";

        string jsonBody = "{ \"user\": { \"username\": \"" + user.username + "\", \"scores\": { ";
        foreach (KeyValuePair<string, int> kvp in user.scores) jsonBody += $" \"{kvp.Key}\": {kvp.Value},";
        jsonBody = jsonBody.Remove(jsonBody.Length - 1, 1);
        jsonBody += " } } }";

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(jsonBody);
            streamWriter.Flush();
        }

        Debug.Log(jsonBody);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    }
}

