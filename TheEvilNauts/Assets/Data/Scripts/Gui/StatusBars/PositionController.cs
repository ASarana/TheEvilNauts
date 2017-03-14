using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 	Default Settings For Align Type
	Bottom :	indent x = 1,	indent y = 0.1, start pos = 0;
	Top:		indent x = 1,	indent y = 0.1, start pos = 1;
	Left:		indent x = 0.1,	indent y = 1, 	start pos = 0;
	Right: 		indent x = 0.1,	indent y = 1, 	start pos = 1;
	Center: 	start pos = 0.5;
*/
public class PositionController : MonoBehaviour
{
	private int _counter = 0; 

	public enum Align
	{
		Bottom = 0, 
		Top = 1, 
		CenterColumn = 2,
		Left = 3, 	
		Right = 4, 	
		CenterRow = 5
	}

	public float indentX = 0.0f;
	public float indentY = 0.0f;
	public float startPos = 0.0f;
	public Align align = Align.Top; 

	public int Counter
	{
		get { return _counter; }
		set { _counter = value; }
	}

	public void SetPosition(GameObject element, int key)
	{
		float xMin = 0;
		float yMin = 0;

		switch (align) {

		case Align.Bottom:
			yMin = startPos + (indentY * key);
			break;

		case Align.Top:
			yMin = startPos - (indentY * (key + 1));
			break;

		case Align.CenterColumn: 
			yMin = (startPos - (indentY * _counter) / 2) + (indentY * key);
			break;
		
		case Align.Left:
			xMin = startPos + (indentX * key);
			break;
				
		case Align.Right:
			xMin = startPos - (indentX * (key + 1));
			break;	
				
		case Align.CenterRow: 	
			xMin = (startPos - (indentX * _counter) / 2) + (indentX * key);
			break;
		}

		SetTransform (element, xMin, yMin);
	}

	private void SetTransform (GameObject element, float minX, float minY)
	{
			RectTransform rectTransform = new RectTransform ();
			rectTransform = element.GetComponent<RectTransform> ();
			rectTransform.SetParent (this.transform);
			rectTransform.anchorMin = new Vector2 (minX, minY);
			rectTransform.anchorMax = new Vector2 (minX + indentX, minY + indentY);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
	}
}
