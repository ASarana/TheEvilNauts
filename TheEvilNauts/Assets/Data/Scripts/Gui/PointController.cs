using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour {

	public  GameObject[] points;
	int value = 0;
	// Use this for initialization
	void Start () 
	{
		value = points.Length; 
	}

	// Update is called once per frame
	void Update ()
	{

	}
	
	public void amountUp()
	{
		if (value < points.Length) 
		{
			value++;		
		}
		SetAmount(value);
	}

	public void amountDown()
	{
		if (value > 0) 
		{
			value--;		
		}
		SetAmount(value);
	}
	void SetAmount(int value)
	{
		Debug.Log (value);
		if ((value > points.Length) || (value < -1))
		{
			value = 0;
		}

		for (int i = 0; i < points.Length; i++)
		{
			if (i >= value) 
			{
				points [i].SetActive (false);
			} 
			else 
			{
				points [i].SetActive (true);
			}
				
		}
	}
}
