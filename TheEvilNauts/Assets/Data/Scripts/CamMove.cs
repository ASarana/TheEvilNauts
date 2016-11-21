using UnityEngine;
using System.Collections;

public class CamMove : MonoBehaviour
{

     Vector3 camzero;
    //public Vector3 camtrans;
    public Transform hero;
    void Start()
    {
        camzero = new Vector3(0, -80, 75);
        //camzero = hero.position - this.transform.position; //начальное положение камера относительно пакмана
		// this.transform.rotation = Quaternion.Euler(camtrans);-82, -72
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = hero.position - camzero; //двигаем камеру в след за пакманом
       // print(camzero);
    }
}
