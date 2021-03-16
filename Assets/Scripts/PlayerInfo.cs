using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    // Start is called before the first frame update

    public static PlayerInfo playerInfo;
    public string username;
    void Awake()
    {
        Debug.Log("hi");
        playerInfo = this;

        DontDestroyOnLoad(this);
        username = PlayerPrefs.GetString("username");

        // if we're on the splash screen, move to the right scene based on if we're logged in
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
    }

}
