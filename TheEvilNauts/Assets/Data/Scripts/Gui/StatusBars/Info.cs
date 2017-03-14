using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**/
public class Info : Status
{
	int counter = 0;

	protected override void Configurate()
	{


	}

	protected override void GrowUp()
	{
		if (counter != _lifeTime) 
		{
			counter++;
			_text.text = counter.ToString ();
		}
	}

	protected override void GrowDown()
	{
		if (_lifeTime == 0)
			Destroy (this.gameObject);
	}
}
