using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoresTitle;

    [SerializeField]
    private TextMeshProUGUI scoresBody;

    // Start is called before the first frame update
    void Start()
    {
        string username = PlayerInfo.playerInfo.username;
        Debug.Log(username);
        Debug.Log(PlayerInfo.playerInfo.user.ToString());

        if (string.IsNullOrWhiteSpace(username))
        {
            scoresTitle.text = "Player Scores";
            scoresBody.text = "Scores not available when not signed in!";
        }
        else
        {
            scoresTitle.text = $"Scores for {username}";

            string body = "";
            foreach (KeyValuePair<string, int> kvp in PlayerInfo.playerInfo.user.scores)
            {
                body += $"{kvp.Key}: {kvp.Value}\n";
            }

            scoresBody.text = body;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
