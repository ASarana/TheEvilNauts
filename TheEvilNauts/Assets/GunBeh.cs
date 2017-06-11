using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBeh : MonoBehaviour {

    // Use this for initialization
    public int holder;
    public int ammunition;
    public int holderReal;
    public int ammunitionReal;
    bool fire;
    bool oneshot;
    public bool reload;
    public int power;
    public int speed;
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
    }

    void UpdateAnimator()
    {
        m_Animator.SetBool("Reload", reload);
        stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("BaseLayer.v_shortgun_reload") && stateInfo.normalizedTime >= 0.9f)
            reload = false;
    }

    public void Reload()
    {
        if(ammunitionReal>0)
        {
            Debug.Log("Reload");
            reload = true;           
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
            Transform bend = transform.FindChild("bend");
            Transform bstart = transform.FindChild("bstart");
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.tag = "bullet";
            bullet.transform.position = bend.position;
            bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            bullet.AddComponent<Rigidbody>();
            bullet.AddComponent<BulletBeh>().power = power;
            bullet.GetComponent<SphereCollider>().isTrigger = true;
            Vector3 v = (bend.position - bstart.position) * speed / Time.deltaTime;
            v.y = bullet.GetComponent<Rigidbody>().velocity.y;
            bullet.GetComponent<Rigidbody>().velocity = v;
        }
        else
        {
            return;
        }
    }
}
