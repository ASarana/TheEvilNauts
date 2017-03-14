using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsController : MonoBehaviour
{
	[SerializeField] private GameObject _prefab;
	private Dictionary <int,GameObject> _elements;
	private int counter = 0;		//all elements
	private PositionController positionContorller;

	public bool availability = true;

	void Start () 
	{
		if (positionContorller == null)
			positionContorller = this.GetComponent<PositionController> ();
		
		_elements = new Dictionary<int,GameObject> ();
	}

	void Update ()
	{
		if (counter > 0)
			Sort ();
		
		if (positionContorller != null) 
		{
			if (counter != positionContorller.Counter)
				Re_position ();
		}
	}
		
	public void AddElement(float value,string text)
	{
		if (availability) 
		{
			if(!(AvabilityCheck (value, text)))
				CreateElement (value, text);
		}
		else 
		{
			CreateElement (value, text);
		}

	}

	private bool AvabilityCheck(float value,string text)
	{
		for (int i = 0; i < counter; i++) 
		{
			if (_elements [i] != null)
			{
				if (_elements [i].GetComponent<Status> ().Type == text)
				{
					_elements [i].GetComponent<Status> ().AddTime (value);
					return true;
				}
			}
		}
		return false;
	}

	private void CreateElement(float value,string text)
	{
		_elements [counter] = Instantiate (_prefab) as GameObject;

		if (_elements [counter].GetComponent<Status> () != null)
		{
			_elements [counter].GetComponent<Status> ().CreateStatus (value, text);
		}
		counter++;
	}

	private void Sort()
	{
		
		if (_elements [counter-1] == null)	
		{
			_elements.Remove (counter-1);	
			counter--;
		}

		for (int i = 0; i < _elements.Count; i++) 
		{
			if (_elements [i] == null) 
			{
				for (int j = i; j < _elements.Count; j++) 
				{
					if (_elements [j] != null) 
					{
						_elements [i] = _elements [j];
						_elements [j] = null;
						break;
					}
				}
			}
		}
	}

	private void Re_position()
	{
		positionContorller.Counter = counter;
		for (int i = 0; i < counter; i++) 
		{
			if (_elements [i] != null) 
			{
				positionContorller.SetPosition (_elements [i],i);			
			}
		}
	}

	public void addhyed() //УДАЛИТЬ!
	{
		AddElement(15.0f,"stroka 1");
		AddElement(20.0f,"stroka 1");
		AddElement(10.0f,"stroka 3");
		AddElement(25.0f,"stroka 4");
	}
}


