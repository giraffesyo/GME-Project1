using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour
{

    private AudioSource audioSource;
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(loadScene);
        audioSource = FindObjectOfType<AudioSource>();
    }

    private void loadScene()
    {
        audioSource.Play();
        SceneLoader.LoadScene(transform.name);
    }

}
