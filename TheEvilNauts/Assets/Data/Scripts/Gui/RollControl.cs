using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollControl : MonoBehaviour {

	public Image targetImage;
	public float rollSpeed = 1F;
	float RollingTime = 0;
	int angle = 90;
	// Use this for initialization
	void Start () 
	{
		if (targetImage == null)
			targetImage = GetComponent<Image>();
	}

	// Update is called once per frame
	void Update()
	{
		if (RollingTime > 0) {
			RollingStone ();
			RollingTime -= (RollingTime*rollSpeed)/angle;
		}

	}

	public void PlsRollMe()
	{
		RollingTime += angle;
	}

	void RollingStone () 
	{
		float speed =(RollingTime*rollSpeed)/angle;
		Vector3 angleOfRotation = new Vector3 (0, 0,speed);

		targetImage.rectTransform.Rotate (angleOfRotation);
	}
}
