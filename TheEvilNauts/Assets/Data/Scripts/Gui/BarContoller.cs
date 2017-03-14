using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent (typeof(Slider))]
public class BarContoller : MonoBehaviour 
{
	private Slider Bar;
	public Text targedText;
	public int maxPoints = 100;
	public int value = 30;

	void Start () 
	{
		if (Bar == null)
			Bar = GetComponent<Slider> ();

	}

	void Update ()
	{
		BarChange ();
	}

	private void BarChange()
	{

		if (targedText != null) 
		{
			targedText.text = Bar.value.ToString() + "/" + Bar.maxValue.ToString();
		}

		if (maxPoints != Bar.maxValue) 
			Bar.maxValue = maxPoints;
		
		if (value < Bar.value)
			Bar.value--;
		
		if (value > Bar.value)
			Bar.value++;

		if (value < 0)
			value = 0;
		
		if (value > maxPoints)
			value = maxPoints;
		
	}

	public void ChangeValue(int points)
	{
		value += points;
	}

	public void SetValue(int points)
	{
		value = points;
	}



}
