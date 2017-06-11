using UnityEngine;
using System.Collections;

public class CamMove : MonoBehaviour
{

    public GameObject hero;
    Vector3 pos;
    bool clock;
    bool unclock;

    void Start()
    {
        pos = transform.position - hero.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        clock = Input.GetKey(KeyCode.Keypad9);
        unclock = Input.GetKey(KeyCode.Keypad7);
        
        if (clock & !unclock)
        {
            transform.RotateAround(hero.transform.position, Vector3.up, 5);
            transform.LookAt(hero.transform.position);
            pos = transform.position - hero.transform.position;
        }

        if (!clock & unclock)
        {
            transform.RotateAround(hero.transform.position, Vector3.up, -5);
            transform.LookAt(hero.transform.position);
            pos = transform.position - hero.transform.position;
        }
        
        transform.position = hero.transform.position + pos;
    }
}
