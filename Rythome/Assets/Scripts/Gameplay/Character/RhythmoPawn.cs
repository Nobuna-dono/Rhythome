using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class RhythmoPawn : RhythmObject
    {
        #region ENUM
        public enum ERhythmoPawnState
        {
            Idle = 0,

            MoveRight = 1,
            MoveLeft = 2,

            FaceDown,
            FaceUp,
        }
        #endregion

        #region STRUCTURES AND CLASSES
        [System.Serializable]
        struct MovementAttribute
        {

        }

        [System.Serializable]
        struct UStateMachineSynchronizers
        {
        }
        #endregion


        #region PROPERTIES
        /** SERIALIZED VARIABLES
         */
        [Space(10)]
        [Header("- Rhythmo Pawn Settings:")]
        [SerializeField]
        private GameObject ResponsiveRhythmUI;
        private Animator ResponsiveUIAnimator;

        [SerializeField, Tooltip("Data synchronized with the state machine.")]
        private StateMachineSyncDataOfInt PawnState;

        [SerializeField]
        private UAnimationAttributes m_AnimationAttributes;

        [SerializeField]
        private float m_MoveValue = 1;
        [SerializeField]
        private bool m_AlwaysEnableTickSound = false;
        [SerializeField]
        private float m_MinWorldBound = -1f, m_MaxWorldBound = 1f;


        /** VARIABLES
        */
        private ERhythmoPawnState m_CurrentState = ERhythmoPawnState.Idle;
        private ERhythmoPawnState m_CurrentStateBuffer = ERhythmoPawnState.Idle;

        // Animation
        private AnimatorHelper m_AnimatorHelper = null;
        #endregion

        #region UNITY METHODS
        protected override void Start()
        {
            base.Start();

            // Animator
            if (ResponsiveRhythmUI)
            {
                ResponsiveUIAnimator = ResponsiveRhythmUI.GetComponent<Animator>();
            }

            // Animator Helper
            m_AnimatorHelper = new AnimatorHelper(transform, m_AnimationAttributes.bFacingRight);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            RhythmoStation SRStation = collision.gameObject.GetComponent<RhythmoStation>();
            if (!SRStation)
                return;

            if (m_CurrentState == ERhythmoPawnState.FaceDown)
            {
                if (SRStation.ZOrder < ZOrder)
                {
                    SRStation.EnableFeedback(true);
                    EnableFeedback(true);
                }
            }
            else if (m_CurrentState == ERhythmoPawnState.FaceUp)
            {
                if (SRStation.ZOrder > ZOrder)
                {
                    SRStation.EnableFeedback(true);
                    EnableFeedback(true);
                }
            }
            else
            {
                SRStation.EnableFeedback(false);
                EnableFeedback(false);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            RhythmoStation SRStation = collision.gameObject.GetComponent<RhythmoStation>();
            if (SRStation)
            {
                SRStation.EnableFeedback(false);
                EnableFeedback(false);
            }
        }
        #endregion


        #region RHYTHOME METHODS
        protected override void Beat()
        {
            if (m_CurrentStateBuffer != m_CurrentState)
            {
                PawnState.SetData((int)m_CurrentState, m_Animator);
                m_CurrentStateBuffer = m_CurrentState;
            }
            switch (m_CurrentState)
            {
                case ERhythmoPawnState.Idle:
                    break;
                case ERhythmoPawnState.MoveRight:
                    MoveRight(m_MoveValue);
                    break;
                case ERhythmoPawnState.MoveLeft:
                    MoveRight(-m_MoveValue);
                    break;
                case ERhythmoPawnState.FaceUp:
                    //FaceUp(m_MoveValue);
                    break;
                case ERhythmoPawnState.FaceDown:
                    //FaceUp(-m_MoveValue);
                    break;
            }

            if (m_AlwaysEnableTickSound)
            {
                RythmoPlay1();
            }
        }

        protected override void HalfBeat()
        {
            if (m_CurrentState >= ERhythmoPawnState.FaceDown)
            {
                RythmoPlay2();
            }
        }

        public override void RythmoPlay1()
        {
            if (m_TickSound)
            {
                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_TickSound;
                m_Source.Play();
            }
        }

        public override void RythmoPlay2()
        {
            if (m_TockSound)
            {
                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_TockSound;
                m_Source.Play();
            }
        }

        public override void EnableFeedback(bool _enable)
        {
            ResponsiveRhythmUI.SetActive(_enable);
            if (_enable)
            {
                ResponsiveUIAnimator.speed = m_Animator.speed;
            }
        }
        #endregion


        #region CUSTOM METHODS
        // Call this with Input builder to define direction.
        public void SetPawnState(int _state)
        {
            m_CurrentState = (ERhythmoPawnState)_state;
        }

        public void MoveRight(float _value)
        {
            // Step 1: Check if able to move.
            if (!CanMoveForward(_value))
            {
                m_AnimatorHelper.Update(0f);
                PawnState.SetData((int)(m_CurrentState = ERhythmoPawnState.Idle), m_Animator);
                return;
            }
            Vector3 pos = transform.position + new Vector3(_value, 0, 0);
            pos.x = Mathf.Clamp(pos.x, m_MinWorldBound, m_MaxWorldBound);
            transform.position = pos;

            m_AnimatorHelper.Update(_value);

            if (!m_AlwaysEnableTickSound)
            {
                RythmoPlay1();
            }
        }

        //public void FaceUp(float _value)
        //{

        //}

        private bool CanMoveForward(float _direction)
        {
            return true;// !ComputeVerticalRay(_direction);
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