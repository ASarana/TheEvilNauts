using UnityEngine;
using System.Collections;
using System;

public class MyDebug : MonoBehaviour {

    // Use this for initialization
    int debi = 20;
    string[] names = new string[20];
    string[] param = new string[20];
    public TextMesh debtext;
    void Start ()
    {
	
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void AddParamDebug (string _name, string _param)
    {
        if (_name == null) return;
        for (int i = 0; i < debi; i++)
        {
            if (names[i] == _name)
            {
                param[i] = _param;
                return;
            }
        }
        for (int i = 0; i < debi; i++)
        {
            if (names[i] == null)
            {
                names[i] = _name;
                param[i] = _param;
                return;
            }
        }
    }

    public void ShowDebug()
    {
        debtext.text = null;
        for (int i = 0; i < debi; i++)
        {
            if(param[i] != null) debtext.text += (names[i] + ": " + param[i] + '\n');
        }
    }
}
