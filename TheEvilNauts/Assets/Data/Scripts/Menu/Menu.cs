using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	public GameObject StartMenu;
	public GameObject Gui;
	public GameObject Character;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void PressStart()
	{
		Debug.Log("<color=red>Start</color>"); 
		StartMenu.SetActive(false);
		Character.SetActive (true);
		Gui.SetActive(true);
	}

	public void PressQuit()		
	{		
		Debug.Log("<color=red>Quit</color>"); 
		//Application.Quit ();
	}
}
