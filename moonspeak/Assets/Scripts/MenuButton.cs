using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour
{


    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(loadScene);

    }

    private void loadScene()
    {
        SceneLoader.LoadScene(transform.name);
    }

}
