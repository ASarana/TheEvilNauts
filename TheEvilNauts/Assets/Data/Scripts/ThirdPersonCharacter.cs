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
        float m_AroundCheckDistance = 10f;
        [SerializeField]
        float gdcheck = 0.2f;
        [SerializeField]
        float archeck = 0.5f;
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
        float GroundDistance;
        float WallDistanceUp;
        float WallDistanceDown;
        float m_CapsuleHeight;
        float StairGoMulti;
        bool Slot1Use;
        bool Slot2Use;
        bool IsJump;
        float Hit;
        bool oneshot;
        float JumpFloat;
        bool Reload;
        float HitHit;
        public int life;
        bool Death;
        bool Damage;
        bool Push;
        bool Swim;
        bool Canuse;
        bool Use;
        bool GoOnStair;
        bool goupup;
        bool godowndown;
        RaycastHit hitInfoObjectbehindUp;
        RaycastHit hitInfoObjectbehindDown;
        RaycastHit hitInfoGround;
        RaycastHit hitInfoHead;

        const float k_Half = 0.5f;

        Vector3 m_GroundNormal;
        Vector3 m_CapsuleCenter;
        CapsuleCollider m_Capsule;
        Transform slot1;
        Transform slot2;
        Transform USESLOT1;
        Transform USESLOT2;
        AnimatorStateInfo stateInfo1;
        AnimatorStateInfo stateInfo2;
        AnimatorStateInfo stateInfo0;
        AnimatorStateInfo stateInfo3;
        AnimatorStateInfo stateInfo4;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
            slot1 = transform.Find("vochapMR").Find("root").Find("hips").Find("spine").Find("SLOT1");
            slot2 = transform.Find("vochapMR").Find("root").Find("hips").Find("spine").Find("chest").Find("SLOT2");
            USESLOT1 = transform.Find("vochapMR").Find("root").Find("hips").Find("spine").Find("chest").Find("shoulder.R").Find("upper_arm.R").Find("forearm.R").Find("hand.R").Find("USESLOT1");
            USESLOT2 = transform.Find("vochapMR").Find("root").Find("hips").Find("spine").Find("chest").Find("shoulder.R").Find("upper_arm.R").Find("forearm.R").Find("hand.R").Find("USESLOT2");
            m_Animator.applyRootMotion = false;
            m_Animator.speed = m_AnimSpeedMultiplier;
        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            UpdateGUI();
            ChekAnimanorStateInfo();
            CheckAroundStatus();
            DeathHeho();

            //  if (Death) move = Vector3.zero;

            if (!Death)
            {
                if (move.magnitude > 1f) move.Normalize();
                if (GoOnStair)
                {
                    StairsMove(move);
                }
                else if (!GoOnStair)
                {
                    NormalMove(move, jump);
                }

                WeaponChange();
                ReloadGun();
                Fire();
                Punch();
                UseObjects();
            }
            else HandleAirborneMovement(move);

            UpdateAnimator(move);
        }

        void UpdateAnimator(Vector3 move)
        {
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            m_Animator.SetBool("OnGround", m_IsGrounded);
            m_Animator.SetFloat("JumpFloat", JumpFloat);
            m_Animator.SetFloat("WallDistanceUp", WallDistanceUp);
            m_Animator.SetFloat("WallDistanceDown", WallDistanceDown);
            m_Animator.SetBool("SwichSlot1", SwichSlot1);
            m_Animator.SetBool("SwichSlot2", SwichSlot2);
            m_Animator.SetFloat("HitHit", HitHit);
            m_Animator.SetBool("Slot1Use", Slot1Use);
            m_Animator.SetBool("Slot2Use", Slot2Use);
            m_Animator.SetBool("Death", Death);
            m_Animator.SetBool("Damage", Damage);
            m_Animator.SetBool("Reload", Reload);
            m_Animator.SetBool("Push", Push);
            m_Animator.SetBool("Swim", Swim);
            m_Animator.SetBool("GoOnStair", GoOnStair);
            m_Animator.SetBool("GoDownDown", godowndown);
            m_Animator.SetFloat("GroundDistance", GroundDistance);
            m_Animator.SetBool("IsJump", IsJump);
            m_Animator.SetFloat("StairGoMulti", StairGoMulti);
        }

        void HandleAirborneMovement(Vector3 move)
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);
            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 12f;

            if (WallDistanceUp > 0.5 && WallDistanceDown <= 0.45 && IsJump)
                m_Rigidbody.velocity = new Vector3(move.x, m_JumpPower * SecondJumpPower, move.z);
        }

        void HandleGroundedMovement(bool jump)
        {
            if (jump && !Death)
            {
                IsJump = true;
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

        void CheckAroundStatus()
        {
            // Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfoGround, m_GroundCheckDistance))
                GroundDistance = hitInfoGround.distance;
            else GroundDistance = 999;
            // Debug.DrawLine(transform.position + (Vector3.up * transform.localScale.y), transform.position + (Vector3.up * transform.localScale.y) + (move.normalized * m_GroundCheckDistance));
            if (Physics.Raycast(transform.position + (Vector3.up * transform.localScale.y), transform.forward, out hitInfoObjectbehindUp, m_AroundCheckDistance))
                WallDistanceUp = hitInfoObjectbehindUp.distance;
            else WallDistanceUp = 999;
            // Debug.DrawLine(transform.position + (Vector3.up * transform.localScale.y * 0.1f), transform.position + (Vector3.up * transform.localScale.y * 0.1f) + (move.normalized * m_GroundCheckDistance));
            if (Physics.Raycast(transform.position + (Vector3.up * transform.localScale.y * 0.1f), transform.forward, out hitInfoObjectbehindDown, m_AroundCheckDistance))
                WallDistanceDown = hitInfoObjectbehindDown.distance;
            else WallDistanceDown = 999;


            if (GroundDistance >= gdcheck)
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                m_Capsule.material.frictionCombine = PhysicMaterialCombine.Minimum;
            }
            else
            {
                m_IsGrounded = true;
                IsJump = false;
                if(hitInfoGround.transform)
                    m_GroundNormal = hitInfoGround.normal;
                else m_GroundNormal = Vector3.up;
                m_Capsule.material.frictionCombine = PhysicMaterialCombine.Maximum;
            }

            if (WallDistanceUp < archeck && hitInfoObjectbehindUp.collider.tag == "box")
                Push = true;
            else if (WallDistanceDown < archeck && hitInfoObjectbehindDown.collider.tag == "stair")
                Canuse = true;
            else
            {
                Push = false;
                Canuse = false;
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
            if (SwichSlot1 && !Slot2Use && !SwichSlot2 && stateInfo2.IsName("Useslot2.idle_unnarmed"))
            {
                SwichSlot1 = true;
            }
            else if (SwichSlot2 && !Slot1Use && !SwichSlot1 && stateInfo1.IsName("Useslot1.idle_unnarmed"))
            {
                SwichSlot2 = true;
            }

            if (stateInfo1.IsName("Useslot1.take_slot1") && !Slot1Use && stateInfo1.normalizedTime >= 0.5f)
            {
                slot1.GetChild(0).SetParent(USESLOT1);
                Slot1Use = true;
                SwichSlot1 = false;
                USESLOT1.GetChild(0).localPosition = new Vector3(0, 0, 0);
                USESLOT1.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

            else if (stateInfo1.IsName("Useslot1.put_slot1") && Slot1Use && stateInfo1.normalizedTime >= 0.5f)
            {
                USESLOT1.GetChild(0).SetParent(slot1);
                Slot1Use = false;
                SwichSlot1 = false;
                slot1.GetChild(0).localPosition = new Vector3(0, 0, 0);
                slot1.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

            else if (stateInfo2.IsName("Useslot2.take_slot2") && !Slot2Use && stateInfo2.normalizedTime >= 0.5f)
            {
                slot2.GetChild(0).SetParent(USESLOT2);
                Slot2Use = true;
                SwichSlot2 = false;
                USESLOT2.GetChild(0).localPosition = new Vector3(0, 0, 0);
                USESLOT2.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

            else if (stateInfo2.IsName("Useslot2.put_slot2") && Slot2Use && stateInfo2.normalizedTime >= 0.5f)
            {
                USESLOT2.GetChild(0).SetParent(slot2);
                Slot2Use = false;
                SwichSlot2 = false;
                slot2.GetChild(0).localPosition = new Vector3(0, 0, 0);
                slot2.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
            }

			if (stateInfo4.IsName("Hits.take_damage") && stateInfo4.normalizedTime >= 0.5f)
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
            if (Hit > 0.1 && Slot2Use && !oneshot && !USESLOT2.GetChild(0).GetComponent<GunBeh>().reload && USESLOT2.GetChild(0).GetComponent<GunBeh>().holder != 0)
            {
                HitHit = Hit;
                USESLOT2.GetChild(0).GetComponent<GunBeh>().Fire();
                oneshot = true;
            }
            
        }

        void Punch()
        {
            if (Hit > 0.1 && Slot1Use)
                HitHit = Hit;
            if ((stateInfo1.IsName("Useslot1.machete_punch1") || stateInfo1.IsName("Useslot1.machete_punch2") || stateInfo1.IsName("Useslot1.machete_punch3")) && Slot1Use)
            {
                Transform weapon = USESLOT1.GetChild(0);
                weapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            else if (!stateInfo1.IsName("Useslot1.machete_punch1") && !stateInfo1.IsName("Useslot1.machete_punch2") && !stateInfo1.IsName("Useslot1.machete_punch3") && Slot1Use)
            {
                Transform weapon = USESLOT1.GetChild(0);
                weapon.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }

        void ReloadGun()
        {
            if (Reload && Slot2Use)
            {
                USESLOT2.GetChild(0).GetComponent<GunBeh>().Reload();
            }

        }

        void DeathHeho()
        {
            if (life <= 0 && !Death)
            {
                Death = true;
              //  m_Rigidbody.velocity = Vector3.zero;
            }
        }

        public void GetInput(bool swichslot1, bool swichslot2, float hit, bool reload, bool use)
        {
            SwichSlot1 = swichslot1;
            SwichSlot2 = swichslot2;
            Hit = hit;
            Reload = reload;
            Use = use;
        }


        void UseObjects()
        {
            if(Use && Canuse && !GoOnStair && hitInfoObjectbehindDown.collider.tag == "stair")
            {
                GoOnStair = true;
                Use = false;
                Vector3 moverot = transform.InverseTransformDirection(-hitInfoObjectbehindDown.normal);
                m_TurnAmount = Mathf.Atan2(moverot.x, moverot.z);
                float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, 1);
                transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
            }
            else if (Use && Canuse && GoOnStair && hitInfoObjectbehindDown.collider.tag == "stair")
            {
                GoOnStair = false;
            }
            else if (hitInfoObjectbehindDown.collider == null || hitInfoObjectbehindDown.collider.tag != "stair")
                GoOnStair = false;
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

            if (other.tag == "water")
            {
                Swim = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "water")
            {
                Swim = false;
            }
        }

        void UpdateGUI()
        {
            GameObject.Find("Health").GetComponent<BarContoller>().SetValue(life);
        }

        void ChekAnimanorStateInfo()
        {
            stateInfo0 = m_Animator.GetCurrentAnimatorStateInfo(0);
            stateInfo1 = m_Animator.GetCurrentAnimatorStateInfo(1);
            stateInfo2 = m_Animator.GetCurrentAnimatorStateInfo(2);
            stateInfo3 = m_Animator.GetCurrentAnimatorStateInfo(3);
            stateInfo4 = m_Animator.GetCurrentAnimatorStateInfo(4);
        }

        void NormalMove(Vector3 move, bool jump)
        {
            m_Animator.speed = 2.5f;
            m_Rigidbody.isKinematic = false;
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            Vector3 moverot = transform.InverseTransformDirection(move);
            m_TurnAmount = Mathf.Atan2(moverot.x, moverot.z);
            m_ForwardAmount = moverot.z;
            ApplyExtraTurnRotation();

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
        }

        void StairsMove(Vector3 move)
        {
            m_Rigidbody.isKinematic = true;
            // move = Vector3.ProjectOnPlane(move, hitInfoobjectbehind.normal);
            if (move.z > 0)
            {
                transform.position += transform.up * move.z * 0.1f;
                // m_Animator.speed = 2.5f;
                StairGoMulti = 1;
                 godowndown = false;
            }
            else if (move.z < 0 && GroundDistance > gdcheck)
            {
                transform.position += transform.up * move.z * 0.1f;
                // m_Animator.speed = 2.5f;
                StairGoMulti = 1;
                godowndown = true;
            }
            else if (move.z == 0 && stateInfo0.IsName("go_up"))
            {
                //m_Animator.speed = 0;
                StairGoMulti = 0;
            }

            if (WallDistanceUp - WallDistanceDown > 1)
            {
               // m_Animator.speed = 2.5f;
                GoOnStair = false;
                Use = false;
                m_Rigidbody.isKinematic = false;
                move = Vector3.ProjectOnPlane(move, m_GroundNormal);
                //  m_Rigidbody.velocity = new Vector3(move.x, m_JumpPower * SecondJumpPower*2, move.z);
                // jump = true;
                IsJump = true;
            }
        }
    }
}