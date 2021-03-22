using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI holaText;

    private void Start()
    {
        // Get the players name
        string username = PlayerInfo.playerInfo.username;
        // set the holaText to the players name
        holaText.text = $"¡Hola, {username}!";
    }

}
