using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**/
public class Status : MonoBehaviour 
{
	public float speed = 0 ;
	protected float _lifeTime = 0;
	protected float _deathTime = 0;
	protected string _type = "";

	[SerializeField]protected Text _text;

	void Update ()
	{
		if (_lifeTime >= _deathTime) 
		{
			GrowUp ();
		} 
		else
		{
			GrowDown ();
		}
	}

	public void CreateStatus (float lifeTime, string type)
	{
		_lifeTime = lifeTime;
		_type = type;
		Configurate();
	}

	public string Type
	{
		get { return _type; }
	}

	public void AddTime (float value)
	{
		_lifeTime += value;
	}

	protected virtual void Configurate(){}
	protected virtual void GrowUp () {}
	protected virtual void GrowDown () {}


}
