using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        [SerializeField]
        float m_MovingTurnSpeed = 400f;
        [SerializeField]
        float m_StationaryTurnSpeed = 400f;
        [SerializeField]
        float m_JumpPower = 20f;
        float SecondJumpPower = 0.8f;
        [Range(1f, 100f)]
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
        [SerializeField]
        float m_RunCycleLegOffset = 0.2f;

        Rigidbody m_Rigidbody;
        Animator m_Animator;
        bool m_IsGrounded;
        bool SwichSlot1;
        bool SwichSlot2;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        float gd;
        float wdu;
        float wdd;
        float gd2;
        float m_CapsuleHeight;
        float NumberOfSlot;
        bool Slot1Use;
        bool Slot2Use;
        float Hit;
        bool oneshot;
        float JumpFloat;
        bool Reload;
        float HitHit;
        public int life;
        bool Death;
        bool Damage;

        const float k_Half = 0.5f;

        Vector3 m_GroundNormal;
        Vector3 m_CapsuleCenter;
        CapsuleCollider m_Capsule;
        Transform slot1;
        Transform slot2;
        Transform USESLOT1;
        Transform USESLOT2;
        AnimatorStateInfo stateInfo;
        AnimatorStateInfo stateInfo2;
        AnimatorStateInfo stateInfo0;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
            slot1 = transform.FindChild("vochapMR").FindChild("root").FindChild("hips").FindChild("spine").FindChild("SLOT1");
            slot2 = transform.FindChild("vochapMR").FindChild("root").FindChild("hips").FindChild("spine").FindChild("chest").FindChild("SLOT2");
            USESLOT1 = transform.FindChild("vochapMR").FindChild("root").FindChild("hips").FindChild("spine").FindChild("chest").FindChild("shoulder.R").FindChild("upper_arm.R").FindChild("forearm.R").FindChild("hand.R").FindChild("USESLOT1");
            USESLOT2 = transform.FindChild("vochapMR").FindChild("root").FindChild("hips").FindChild("spine").FindChild("chest").FindChild("shoulder.R").FindChild("upper_arm.R").FindChild("forearm.R").FindChild("hand.R").FindChild("USESLOT2");
            m_Animator.applyRootMotion = false;
            m_Animator.speed = m_AnimSpeedMultiplier;
        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            if (move.magnitude > 1f) move.Normalize();
            if (Death) move = Vector3.zero;
            Vector3 moverot = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_TurnAmount = Mathf.Atan2(moverot.x, moverot.z);
            m_ForwardAmount = moverot.z;
            ApplyExtraTurnRotation();
            WeaponChange();
            ReloadGun();
            Fire();
            Punch();
            DeathHeho();
            if (m_IsGrounded)
            {
                JumpLeg();
                HandleGroundedMovement(jump);
                Vector3 v = (move * m_MoveSpeedMultiplier) / Time.deltaTime;
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
                JumpFloat = 0;
            }
            else
            {
                HandleAirborneMovement(move);
                JumpFloat = m_Rigidbody.velocity.y;
                if (JumpFloat < -20) life -= 50;
            }
            UpdateAnimator(move);
        }

        void UpdateAnimator(Vector3 move)
        {
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            m_Animator.SetBool("OnGround", m_IsGrounded);
            m_Animator.SetFloat("Jump", JumpFloat);
            m_Animator.SetFloat("WallDistanceUp", wdu);
            m_Animator.SetFloat("WallDistanceDown", wdd);
            m_Animator.SetBool("SwichSlot1", SwichSlot1);
            m_Animator.SetBool("SwichSlot2", SwichSlot2);
            m_Animator.SetFloat("Hit", HitHit);
            m_Animator.SetBool("Slot1Use", Slot1Use);
            m_Animator.SetBool("Slot2Use", Slot2Use);
            m_Animator.SetBool("Death", Death);
            m_Animator.SetBool("Damage", Damage);
        }

        void HandleAirborneMovement(Vector3 move)
        {
            RaycastHit hitInfo, hitInfo2;
            Debug.DrawLine(transform.position + (Vector3.up * transform.localScale.y), transform.position + (Vector3.up * transform.localScale.y) + (move.normalized * m_GroundCheckDistance));

            if (Physics.Raycast(transform.position + (Vector3.up * transform.localScale.y), move.normalized, out hitInfo, m_GroundCheckDistance))
                wdu = hitInfo.distance;
            else wdu = 999;

            Debug.DrawLine(transform.position + (Vector3.up * transform.localScale.y * 0.1f), transform.position + (Vector3.up * transform.localScale.y * 0.1f) + (move.normalized * m_GroundCheckDistance));

            if (Physics.Raycast(transform.position + (Vector3.up * transform.localScale.y * 0.1f), move.normalized, out hitInfo2, m_GroundCheckDistance))
                wdd = hitInfo2.distance;
            else wdd = 999;
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);
            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 12f;

            if (wdu > 0.5 && wdd <= 0.45 && m_Animator.GetBool("IsJump"))
                m_Rigidbody.velocity = new Vector3(move.x, m_JumpPower * SecondJumpPower, move.z);
        }

        void HandleGroundedMovement(bool jump)
        {
            if (jump)
            {
                m_Animator.SetBool("IsJump", true);
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_Capsule.material.frictionCombine = PhysicMaterialCombine.Minimum;
            }
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

        void JumpLeg()
        {
            float runCycle = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            m_Animator.SetFloat("JumpLeg", jumpLeg);
        }

        void WeaponChange()
        {
            stateInfo0 = m_Animator.GetCurrentAnimatorStateInfo(0);
            stateInfo = m_Animator.GetCurrentAnimatorStateInfo(1);
            stateInfo2 = m_Animator.GetCurrentAnimatorStateInfo(2);

            if (SwichSlot1 && !Slot2Use && !SwichSlot2 && stateInfo2.IsName("Useslot2.idle_unnarmed"))
            {
                SwichSlot1 = true;
            }
            else if (SwichSlot2 && !Slot1Use && !SwichSlot1 && stateInfo.IsName("Useslot1.idle_unnarmed"))
            {
                SwichSlot2 = true;
            }


            if (stateInfo.IsName("Useslot1.take_slot1") && !Slot1Use && stateInfo.normalizedTime >= 0.5f)
            {
                slot1.GetChild(0).SetParent(USESLOT1);
                Slot1Use = true;
                SwichSlot1 = false;
                USESLOT1.GetChild(0).localPosition = new Vector3(0, 0, 0);
                USESLOT1.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

            else if (stateInfo.IsName("Useslot1.put_slot1") && Slot1Use && stateInfo.normalizedTime >= 0.5f)
            {
                USESLOT1.GetChild(0).SetParent(slot1);
                Slot1Use = false;
                SwichSlot1 = false;
                slot1.GetChild(0).localPosition = new Vector3(0, 0, 0);
                slot1.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

            else if (stateInfo2.IsName("Useslot2.take_slot2") && !Slot2Use && stateInfo.normalizedTime >= 0.5f)
            {
                slot2.GetChild(0).SetParent(USESLOT2);
                Slot2Use = true;
                SwichSlot2 = false;
                USESLOT2.GetChild(0).localPosition = new Vector3(0, 0, 0);
                USESLOT2.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }


            else if (stateInfo2.IsName("Useslot2.put_slot2") && Slot2Use && stateInfo.normalizedTime >= 0.5f)
            {
                USESLOT2.GetChild(0).SetParent(slot2);
                Slot2Use = false;
                SwichSlot2 = false;
                slot2.GetChild(0).localPosition = new Vector3(0, 0, 0);
                slot2.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

            if (stateInfo0.IsName("Unarmmed.take_damage") && stateInfo.normalizedTime >= 0.5f)
            {
                Damage = false;
            }
        }

        void Fire()
        {
            if (Hit < 0.1)
            {
                oneshot = false;
                HitHit = Hit;
            }
            if (Hit > 0.1 && Slot2Use && !oneshot && !USESLOT2.GetChild(0).GetComponent<GunBeh>().reload)
            {
                HitHit = Hit;
                USESLOT2.GetChild(0).GetComponent<GunBeh>().Fire();
                oneshot = true;
            }
            GameObject.Destroy(GameObject.Find("Sphere"), 0.5f);
        }

        void Punch()
        {
            if (Hit > 0.1 && Slot1Use)
                HitHit = Hit;

            stateInfo = m_Animator.GetCurrentAnimatorStateInfo(1);
            if (stateInfo.IsName("Useslot1.machete_punch") && Slot1Use)
            {
                Transform weapon = USESLOT1.GetChild(0);
                weapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            else if (!stateInfo.IsName("Useslot1.machete_punch") && Slot1Use)
            {
                Transform weapon = USESLOT1.GetChild(0);
                weapon.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }

        void ReloadGun()
        {
            if (Reload && Slot2Use)
            {
                Debug.Log("Rel");
                USESLOT2.GetChild(0).GetComponent<GunBeh>().Reload();
            }

        }

        void DeathHeho()
        {
            if (life <= 0 && !Death)
            {
                Death = true;
            }
        }

        public void GetInput(bool swichslot1, bool swichslot2, float hit, bool reload)
        {
            SwichSlot1 = swichslot1;
            SwichSlot2 = swichslot2;
            Hit = hit;
            Reload = reload;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "EnemyBullet")
            {
                life -= other.GetComponent<BulletBeh>().power;
                Damage = true;
                GameObject.Destroy(other.gameObject);
            }

            if (other.tag == "EnemyWeapon")
            {
                life -= other.GetComponent<WeaponBeh>().power;
                Damage = true;
                Debug.Log(life);
            }
        }
    }
}