using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**/
public class Effect : Status
{
	private float size = 1f;


	protected override void GrowUp()
	{
		_text.text = Mathf.Round (_lifeTime).ToString ();
		_lifeTime -= Time.deltaTime;
		
		if (transform.localScale.x < size)
		{
			transform.localScale += new Vector3 (speed, speed, 0f);
		}
	}

	protected override void GrowDown()
	{
		
		if (transform.localScale.x >= 0)
		{
			transform.localScale -= new Vector3 (speed, speed, 0f);
		}
		else
		{
			Destroy (this.gameObject);
		}
	}


}
