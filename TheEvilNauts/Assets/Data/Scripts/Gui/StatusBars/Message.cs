using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**/
public class Message : Status
{
	private Color textColor;
	private float alphaMin = 0f;
	private float alphaMax = 1f;

	protected override void Configurate()
	{
		_text.text = _type;
		textColor = new Color ();
		textColor = _text.color;
	}

	protected override void GrowUp()
	{
		_lifeTime -= Time.deltaTime;
		if (textColor.a < alphaMax)
		{
			textColor.a += speed;
			_text.color = textColor;
		}
	}

	protected override void GrowDown()
	{

		if (textColor.a >= alphaMin)
		{
			textColor.a -= speed;
			_text.color = textColor;
		}
		else
		{
			Destroy (this.gameObject);
		}
	}

}
