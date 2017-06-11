using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 200f;
		[SerializeField] float m_StationaryTurnSpeed = 200f;
        [SerializeField] float m_JumpPower = 250f;
        [Range(1f, 100f)][SerializeField] float m_GravityMultiplier = 100f;
		[SerializeField] float m_MoveSpeedMultiplier = 2f;
		[SerializeField] float m_AnimSpeedMultiplier = 2f;
		[SerializeField] float m_GroundCheckDistance = 4f;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		bool m_Crouching;
        bool SwichSlot1;
        float m_OrigGroundCheckDistance;
		float m_TurnAmount;
		float m_ForwardAmount;
		float gd;
		float m_CapsuleHeight;
		float NumberOfSlot;
		bool Slot1Use;
		Vector3 m_GroundNormal;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
        Transform slot1;
        Transform hand;
        Transform chest;
        //const float k_Half = 0.5f;
        AnimatorStateInfo stateInfo;
        GameObject[] g;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;

            slot1 = transform.FindChild("vochap_mesh").FindChild("vochap_metarig").FindChild("root").FindChild("hips").FindChild("spine").FindChild("chest").FindChild("SLOT_1");
            chest = transform.FindChild("vochap_mesh").FindChild("vochap_metarig").FindChild("root").FindChild("hips").FindChild("spine").FindChild("chest");
            hand = transform.FindChild("vochap_mesh").FindChild("vochap_metarig").FindChild("root").FindChild("hips").FindChild("spine").FindChild("chest").FindChild("shoulder.R").FindChild("upper_arm.R").FindChild("forearm.R").FindChild("hand.R");

        }

			
		public void Move(Vector3 move, bool crouch, bool jump, bool swichslot1)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}

            SwichingSlots(swichslot1);

			//ScaleCapsuleForCrouching(crouch);
			//PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}


		/*void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}*/

		/*void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}*/


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);
            m_Animator.SetBool("SwichSlot1", SwichSlot1);
            m_Animator.SetBool("Slot1Use", Slot1Use);

            //slot1.SetParent(chest);

            stateInfo = m_Animator.GetCurrentAnimatorStateInfo(1);
            if (!Slot1Use)
            {
                if (Animator.StringToHash("Useslot1.take_slot1") == stateInfo.fullPathHash && stateInfo.normalizedTime >= 0.35f)
                {
                    slot1.SetParent(hand);
                    Slot1Use = true;
                    slot1.localPosition = new Vector3(0.006f,0.015f,0.008f);
                    slot1.localRotation = Quaternion.Euler(110, 0, 90); 
                }
            }
            else if (Slot1Use)
            {
                if (Animator.StringToHash("Useslot1.put_slot1") == stateInfo.fullPathHash && stateInfo.normalizedTime >= 0.65f)
                {
                    slot1.SetParent(chest);
                    Slot1Use = false;

                    slot1.localPosition = new Vector3(0.042f, -0.03f, -0.02f);
                    slot1.localRotation = Quaternion.Euler(-90, 50, 45);
                }
            }


			if (!m_IsGrounded) {
				m_Animator.SetFloat ("Jump", m_Rigidbody.velocity.y);
			} else 
			{
				m_Animator.SetFloat ("Jump", 0);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
            }
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}

	/*	public void UpdateSlot (float slot)
		{
			if (slot == 1 && Slot1Use == false) 
			{
                Slot1Use = true;
				m_Animator.SetBool ("Slot1Use", Slot1Use);
            } 
			else 
			{
				Slot1Use = false;
				m_Animator.SetBool ("Slot1Use", Slot1Use);
			}
		}*/

        void SwichingSlots(bool swich)
        {
            SwichSlot1 = swich;
        }



        void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);
			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.1f;

        }


		void HandleGroundedMovement(bool crouch, bool jump)
		{          
           // m_Animator.SetFloat("GroundDistance", m_GroundCheckDistance);
            if (jump && !crouch)
            {
                // jump!
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
            }
        }

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
           
            if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.velocity.y;
                //v.y = Vector3.down.y;
                m_Rigidbody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f ), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
                gd = hitInfo.distance;
				m_Animator.SetFloat("GroundDistance", gd);
            }
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
                gd = hitInfo.distance;
				m_Animator.SetFloat("GroundDistance", gd);
			}
		}
	}
}