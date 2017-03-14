using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class RollControl : MonoBehaviour {

	Image targetImage;
	public float rollSpeed = 1F;
	float rollingTime = 0;
	public int angle = 90;
	public bool randomAngle = false;
	public bool clockwise = false;

	// Use this for initialization
	void Start () 
	{
		if (targetImage == null)
			targetImage = GetComponent<Image>();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) 
		{
			if (randomAngle)
			{
				rollingTime += (int)Random.Range (0, angle);
			}
			else 
			{
				rollingTime += angle;
			}
			Debug.Log (rollingTime);
		}

		float speed = Mathf.Floor((rollingTime*rollSpeed)/angle);

		if (rollingTime > 0) 
		{
			rollingTime -= speed;
			RollingStone (speed);
		}
			
	}

	void RollingStone (float speed) 
	{
		if (clockwise)
			speed *= -1;
		Vector3 angleOfRotation = new Vector3 (0, 0,speed);

		targetImage.rectTransform.Rotate (angleOfRotation);
	}
}
