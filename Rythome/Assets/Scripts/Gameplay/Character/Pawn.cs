using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rythome.Core;

namespace Rythome.Character
{
    public class Pawn : MonoBehaviour
    {
        #region ENUM
        /** ENUM
         */
        // CharacterMovementComponent
        public enum EUpperBodyState
        {
            Right_Idle = 0,
            Right_Move = 1,
            Right_Action1 = 2,
            Right_Action2 = 3,
            Right_Action3 = 4,

            Up_Idle = 5,
            Up_Move = 6,
            Up_Action1 = 7,
            Up_Action2 = 8,
            Up_Action3 = 9,

            Down_Idle = 10,
            Down_Move = 11,
            Down_Action1 = 12,
            Down_Action2 = 13,
            Down_Action3 = 14,
        }

        public enum ELowerBodyState
        {
            Right_Idle,
            Right_Move,

            Up_Idle,

            Down_Idle,
            Down_Sit,
        }

        //public enum EInteractionType
        //{
        //    Idle = 0,
        //    Talk = 1,
        //    Trigger = 2,
        //    Climb = 3,
        //}
        #endregion


        #region STRUCTURES AND CLASSES
        /** STRUCT
            */
        // CharacterMovementComponent
        [System.Serializable]
        struct MovementAttribute
        {
           
        }

        [System.Serializable]
        struct UStateMachineSynchronizers
        {
            public StateMachineSyncDataOfInt UBState;
            public StateMachineSyncDataOfInt LBState;
            public StateMachineSyncDataOfFloat Velocity;

            public StateMachineSyncDataOfBool bInteracting;
        }
        #endregion


        #region PROPERTIES
        /** SERIALIZED VARIABLES
         */
        [Header("- Animation attributes:")]

        [SerializeField, Tooltip("Data synchronized with the state machine.")]
        private UStateMachineSynchronizers m_SynchronizedData;
        [SerializeField]
        private UAnimationAttributes m_AnimationAttributes;

        [Space(10)]
        [Header("- Movement attributes:")]
        [SerializeField]
        [Range(0, 10)]
        private int MaxInputFrequency;

        /** VARIABLES
        */
        // CharacterMovementComponent
        private EUpperBodyState m_UB_CurrentState = EUpperBodyState.Right_Idle;
        private ELowerBodyState m_LB_CurrentState = ELowerBodyState.Right_Idle;
        // CharacterMovementComponent
        //private Rigidbody2D m_Rigidbody = null;

        // Animation
        private Animator m_Animator = null;
        private AnimatorHelper m_AnimatorHelper = null;
        //private EInteractionType m_CurrentInteraction = EInteractionType.Idle;

        // Physics 
        private float Velocity = 0;
        //[SerializeField]
        //private CapsuleCollider2D Hitbox;
        #endregion

        #region UNITY METHODS
        void Start()
        {
            // Animator
            m_Animator = GetComponent<Animator>();
            if (m_Animator == null)
            {
                Debug.Log("Failed to get 'Animator' component.");
            }

            // Animator Helper
            m_AnimatorHelper = new AnimatorHelper(transform, m_AnimationAttributes.bFacingRight);

            // Rigidbody
            //m_Rigidbody = GetComponent<Rigidbody2D>();
            //if (m_Rigidbody == null)
            //{
            //    Debug.LogWarning("Failed to get 'Rigidbody2D' component. Auto generation...");
            //    m_Rigidbody = new Rigidbody2D();
            //}
        }

        #endregion

        #region CUSTOM METHODS
        public void MoveLeft()
        {
            if (!CanMoveForward(-1))
            {
                m_AnimatorHelper.Update(0f);
                Stop();
                return;
            }

            Vector3 pos = transform.position + new Vector3(-1,0,0);
            transform.position = pos;

            m_AnimatorHelper.Update(-1f);
            m_SynchronizedData.Velocity.SetData(-1f, m_Animator);
        }

        public void MoveRight()
        {
            if (!CanMoveForward(1))
            {
                m_AnimatorHelper.Update(0f);
                Stop();
                return;
            }

            Vector3 pos = transform.position + new Vector3(1, 0, 0);
            transform.position = pos;

            m_AnimatorHelper.Update(1f);
            m_SynchronizedData.Velocity.SetData(1f, m_Animator);
        }

        /** INTERNAL METHODS
         */
        private void SetUBMovementState(EUpperBodyState _newState)
        {
            m_UB_CurrentState = _newState;
            m_SynchronizedData.UBState.SetData((int)_newState, m_Animator);
        }

        private void SetLBMovementState(ELowerBodyState _newState)
        {
            m_LB_CurrentState = _newState;
            m_SynchronizedData.LBState.SetData((int)_newState, m_Animator);
        }

        private bool CanMoveForward(float _direction)
        {
            return !ComputeVerticalRay(_direction);
        }

        private void Stop()
        {
            m_SynchronizedData.Velocity.SetData(0, m_Animator);
        }


        /** HELPERS METHODS
         */
        private Collider2D ComputeVerticalRay(float _sign)
        {
            Vector2 origin = transform.position;
            float distance = 1;
            //float distance = Hitbox.size.y * 0.8f;
            //origin.x += _sign * Hitbox.size.x * 0.55f + (_sign * 0.1f);
            //origin.y += Hitbox.size.y * 0.4f + Hitbox.offset.y;

            //Debug.DrawRay(origin, Vector3.down * distance, Color.red);
            return Physics2D.Raycast(origin, Vector2.down, distance).collider;
        }

        private Collider2D ComputeHorizontalRaycast()
        {
            Vector2 origin = transform.position;
            //origin.y -= Hitbox.size.y * 0.55f - Hitbox.offset.y;
            //origin.x -= Hitbox.size.x * 0.4f;
            //float distance = Hitbox.size.x * 0.8f;
            float distance = 1;
            Debug.DrawRay(origin, Vector3.right * distance, Color.red);

            RaycastHit2D hitInfo = Physics2D.Raycast(origin, Vector2.right, distance);

            return hitInfo.collider;
        }
        #endregion
    }
}