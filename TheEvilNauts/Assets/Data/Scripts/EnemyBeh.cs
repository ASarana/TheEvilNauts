using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeh : MonoBehaviour
{

    [SerializeField]
    float m_MovingTurnSpeed = 400f;
    [SerializeField]
    float m_StationaryTurnSpeed = 400f;
    [SerializeField]
    float m_GravityMultiplier = 10f;
    [SerializeField]
    float m_MoveSpeedMultiplier = 2f;
    [SerializeField]
    float m_AnimSpeedMultiplier = 2f;
    [SerializeField]
    float m_GroundCheckDistance = 10f;
    [SerializeField]
    float gdcheck = 0.2f;


    Rigidbody m_Rigidbody;
    Animator m_Animator;
    bool m_IsGrounded;
    //bool m_Crouching;
    float m_OrigGroundCheckDistance;
    float m_TurnAmount;
    float m_ForwardAmount;
    float gd;
    float gd2;
    float m_CapsuleHeight;
    bool Hit;
    Vector3 move;
    public int life;
    bool getdamage;
    bool fire;
    bool death;
    bool reborn;


    Vector3 m_GroundNormal;
    CapsuleCollider m_Capsule;
    Transform slot1;
    Transform slot2;
    //const float k_Half = 0.5f;
    AnimatorStateInfo stateInfo;
    AnimatorStateInfo stateInfo1;
    TextMesh textlife;
    Vector3 startpos;
    Quaternion startorint;
    float JumpFloat;

    // Use this for initialization
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;
         slot1 = transform.Find("walking_zombie_metarig").Find("root").Find("hips").Find("chest").Find("shoulder.L").Find("upper_arm.L").Find("forearm.L").Find("hand.L").Find("palm.01.L");
         slot2 = transform.Find("walking_zombie_metarig").Find("root").Find("hips").Find("chest").Find("shoulder.R").Find("upper_arm.R").Find("forearm.R").Find("hand.R").Find("palm.01.R");
        m_Animator.applyRootMotion = false;
        m_Animator.speed = m_AnimSpeedMultiplier;
        life = 20;
        startpos = transform.position;
        startorint = transform.rotation;
        reborn = false;
        m_Capsule.enabled = true;
        m_Rigidbody.useGravity = true;
        death = false;
        move = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("hp").GetComponent<TextMesh>().text = life.ToString();

        stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
         stateInfo1 = m_Animator.GetCurrentAnimatorStateInfo(1);

        if (!stateInfo1.IsName("actions.take_damage") && !stateInfo.IsName("activity.death") && !death)
        {
            if ((GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).magnitude > 1)
            {
                move = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
                fire = false;
                slot1.GetComponent<SphereCollider>().enabled = false;
                slot2.GetComponent<SphereCollider>().enabled = false;
            }
            else
            {
                move = Vector3.zero;
                fire = true;
                if (stateInfo1.IsName("actions.attack"))
                {
                    slot1.GetComponent<SphereCollider>().enabled = true;
                    slot2.GetComponent<SphereCollider>().enabled = true;
                }
                else if (!stateInfo1.IsName("actions.attack"))
                {
                    slot1.GetComponent<SphereCollider>().enabled = false;
                    slot2.GetComponent<SphereCollider>().enabled = false;
                }
            }
        }
        else if (stateInfo1.IsName("actions.take_damage") && stateInfo.normalizedTime > 0.5f)
        {
            Hit = false;
        }

        if (move.magnitude > 1f) move.Normalize();
        Vector3 moverot = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(moverot.x, moverot.z);
        m_ForwardAmount = moverot.z;
        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            Vector3 v = (move * m_MoveSpeedMultiplier) / Time.deltaTime;
            v.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = v;
            JumpFloat = 0;
        }
        else
        {
            HandleAirborneMovement(move);
            JumpFloat = m_Rigidbody.velocity.y;
        }

        // send input and other state parameters to the animator
        Death();
        UpdateAnimator(move);

       // RaycastHit hitInfo, hitInfo2;
        Debug.DrawLine(transform.position + (Vector3.up * transform.localScale.y), transform.position + (Vector3.up * transform.localScale.y) + (move.normalized * m_GroundCheckDistance));

    }

    void UpdateAnimator(Vector3 move)
    {

        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetBool("OnGround", m_IsGrounded);
        m_Animator.SetBool("Fire", fire);
        m_Animator.SetBool("Hit", Hit);
        m_Animator.SetBool("death", death);
        m_Animator.SetFloat("Jump", JumpFloat);
    }

    void HandleAirborneMovement(Vector3 move)
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);
        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 12f;
    }


    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }


    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            gd = hitInfo.distance;

        if (gd >= gdcheck)
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            m_Animator.SetFloat("GroundDistance", gd);
            m_Capsule.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }
        else
        {
            m_IsGrounded = true;
            m_Animator.SetBool("IsJump", false);
            m_GroundNormal = hitInfo.normal;
            m_Animator.SetFloat("GroundDistance", gd);
            m_Capsule.material.frictionCombine = PhysicMaterialCombine.Maximum;
        }
    }

    void Death()
    {
        if (life <= 0 && !death)
        {
            death = true;
            m_Capsule.enabled = false;
            slot1.GetComponent<SphereCollider>().enabled = false;
            slot2.GetComponent<SphereCollider>().enabled = false;
            m_Rigidbody.useGravity = false;
            move = Vector3.zero;
            fire = false;
            Hit = false;
            if (!reborn)
            {
                GameObject.Instantiate(this, startpos, startorint);
                GameObject.Instantiate(this, startpos + new Vector3(10, 0, 10), startorint);
                reborn = true;
            }

        }

        if(death) move = Vector3.zero;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "bullet")
        {
            life -= other.GetComponent<BulletBeh>().power;
            Hit = true;
            GameObject.Destroy(other.gameObject);           
        }

        if (other.tag == "weapon")
        {
            life -= other.GetComponent<WeaponBeh>().power;
            Hit = true;
        }
    }
}
