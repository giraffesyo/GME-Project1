using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LoginButton : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI usernameField;
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(login);
    }

    private void login()
    {
        string username = usernameField.text;
        PlayerInfo.playerInfo.SetPlayer(username);
        SceneLoader.LoadScene("menu");
    }
}
