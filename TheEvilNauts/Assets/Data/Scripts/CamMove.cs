using UnityEngine;
using System.Collections;

public class CamMove : MonoBehaviour
{

     Vector3 camzero;
    //public Vector3 camtrans;
    public Transform hero;
    void Start()
    {
        camzero = new Vector3(0, -82, -72);
        //camzero = hero.position - this.transform.position; //начальное положение камера относительно пакмана
       // this.transform.rotation = Quaternion.Euler(camtrans);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = hero.position - camzero; //двигаем камеру в след за пакманом
       // print(camzero);
    }
}
