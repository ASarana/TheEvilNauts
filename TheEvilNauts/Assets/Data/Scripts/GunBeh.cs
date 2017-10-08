using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBeh : MonoBehaviour {

    // Use this for initialization
    public int holder = 2;
    public int ammunition =100;
    public int holderReal =2;
    public int ammunitionReal = 100;
    bool fire;
    bool oneshot;
    bool isreload;
    public bool reload;
    public int power = 5;
    public float speed = 1;
    public int numofbull = 20;
    public float destroyuntil = 0.5f;
    Animator m_Animator;
    AnimatorStateInfo stateInfo;

    void Start ()
    {
        m_Animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateAnimator();
        GameObject.Destroy(GameObject.Find("Cube"), destroyuntil);
        GameObject.Destroy(GameObject.Find("Cylinder"), 10);
    }

    void UpdateAnimator()
    {
        m_Animator.SetBool("Reload", reload);
        stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("BaseLayer.v_shortgun_reload"))
            isreload = true;
        else isreload = false;
        if (stateInfo.IsName("BaseLayer.v_shortgun_reload") && stateInfo.normalizedTime >= 0.9f)
            reload = false;
    }

    public void Reload()
    {
        if(ammunitionReal>0 && !isreload && !reload)
        {
            Debug.Log("Reload");
            reload = true;
            ////
            int i = 0;
            while(i<holderReal)
            {
                TubeGen();
                i++;
            }
            ////

            holder = 0;
            while (holder<holderReal && ammunitionReal>0)
            {
                holder++;
                ammunitionReal--;
            }
        }

    }

    public void Fire()
    {
        if(holder>0)
        {
            fire = true;
            holder--;
            Transform bend = transform.Find("bend");
            Transform bstart = transform.Find("bstart");
            int countbull = 0;
            while (numofbull > countbull)
            {
                GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bullet.tag = "bullet";
                bullet.transform.position = bend.position;
                bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                bullet.AddComponent<Rigidbody>();
                bullet.AddComponent<BulletBeh>().power = Mathf.RoundToInt(power/numofbull);
                bullet.GetComponent<BoxCollider>().isTrigger = true; 
                
                float x = Random.Range(-0.05f, 0.05f);
                float y = Random.Range(-0.05f, 0.05f);
                float z = Random.Range(-0.05f, 0.05f);


                // Vector3 v = (bend.position - bstart.position) * speed / Time.deltaTime;

                //Vector3 v = (bend.position - new Vector3(bstart.position.x + x, bstart.position.y + y , bstart.position.z + z )) * speed / Time.deltaTime;
                Vector3 v = (transform.forward + new Vector3(x,y,z)) * speed / Time.deltaTime;
                v.y = bullet.GetComponent<Rigidbody>().velocity.y;
                bullet.GetComponent<Rigidbody>().velocity = v;
                countbull++;
            }
        }
        else
        {
            return;
        }
    }

    void TubeGen()
    {
        Transform bstart = transform.Find("bstart");
        Transform bend = transform.Find("bend");
        GameObject tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tube.tag = "tube";
        tube.transform.position = bstart.position;
        tube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        tube.AddComponent<Rigidbody>();
      //  tube.GetComponent<CapsuleCollider>().isTrigger = true;

        float x = Random.Range(-0.05f, 0.05f);
        float y = Random.Range(-0.05f, 0.05f);
        float z = Random.Range(-0.05f, 0.05f);

       // Vector3 v = (bstart.position - new Vector3(bend.position.x + x, bend.position.y + y, bend.position.z + z)) * 0.1f / Time.deltaTime;
        Vector3 v = (-transform.forward + new Vector3(x, y, z)) * 0.1f / Time.deltaTime;
        v.y = tube.GetComponent<Rigidbody>().velocity.y;
        tube.GetComponent<Rigidbody>().velocity = v;
    }
}
