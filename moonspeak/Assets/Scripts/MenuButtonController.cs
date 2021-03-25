using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MenuButtonController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI holaText; // Michael
    [SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;

    private void Start() //Michael
    {
        // Get the players name
        string username = PlayerInfo.playerInfo.username;
        // set the holaText to the players name
        holaText.text = $"¡Hola, {username}!";
    }

    void Update()
    {
		if(menuButtonController.index == thisIndex)
		{
			animator.SetBool ("selected", true);
			if(Input.GetAxis ("Submit") == 1){
				animator.SetBool ("pressed", true);
			}else if (animator.GetBool ("pressed")){
				animator.SetBool ("pressed", false);
				animatorFunctions.disableOnce = true;
			}
		}else{
			animator.SetBool ("selected", false);
		}
    }

}
