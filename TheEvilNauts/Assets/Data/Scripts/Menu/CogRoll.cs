using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Image))]
public class CogRoll : MonoBehaviour {

	public Image targetImage;
	public float rollSpeed = 1.5F;
	// Use this for initialization
	void Start () 
	{
		if (targetImage == null)
			targetImage = GetComponent<Image>();
	}

	// Update is called once per frame
	void Update () 
	{
		
		Vector3 angle = new Vector3 (0, 0,rollSpeed);
	
		targetImage.rectTransform.Rotate (angle);
	}
}
