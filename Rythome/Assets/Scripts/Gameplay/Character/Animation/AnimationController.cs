using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rhythome.Gameplay
{
    public enum StateMachineType
    {
        None, Bool, Int, Float
    }

    [System.Serializable]
    public struct UStateMachineParametersAttributes
    {
        public string ParameterName;
        public StateMachineType Type;
    }

    [System.Serializable]
    public struct UAnimationAttributes
    {
        public bool bFacingRight;
    }

    [System.Serializable]
    public class StateMachineSynchData<T>
    {
        protected T m_Data;
        public string ParameterName;

        public static implicit operator T(StateMachineSynchData<T> _object)
        {
            return _object.m_Data;
        }

        // Set value and synchronize with the state machine
        virtual public void SetData(T _value, Animator _animator = null)
        { }
    }

    [System.Serializable]
    public class StateMachineSyncDataOfBool : StateMachineSynchData<bool>
    {
        public override void SetData(bool _value, Animator _animator = null)
        {
            m_Data = _value;
            if (_animator != null && ParameterName.Length > 0)
            {
                _animator.SetBool(ParameterName, System.Convert.ToBoolean(m_Data));
            }
            else
            {
                Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
            }
        }
    }

    [System.Serializable]
    public class StateMachineSyncDataOfInt : StateMachineSynchData<int>
    {
        public override void SetData(int _value, Animator _animator = null)
        {
            m_Data = _value;
            if (_animator != null && ParameterName.Length > 0)
            {
                _animator.SetInteger(ParameterName, System.Convert.ToInt32(m_Data));
            }
            else
            {
                Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
            }
        }
    }

    [System.Serializable]
    public class StateMachineSyncDataOfFloat : StateMachineSynchData<float>
    {
        public override void SetData(float _value, Animator _animator = null)
        {
            m_Data = _value;
            if (_animator != null && ParameterName.Length > 0)
            {
                _animator.SetFloat(ParameterName, Mathf.Abs(System.Convert.ToSingle(m_Data)));
            }
            else
            {
                Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
            }
        }
    }

    public class AnimatorHelper
    {
        #region Intern Properties Field
        private Transform m_Transform = null;
        private bool m_FacingRight;
        #endregion

        #region Custom Methods Field
        public AnimatorHelper(Transform _transform, bool _facingRight = true)
        {
            if (_transform == null)
            {
                Debug.LogWarning("Null transform's ref passed as argument.");
            }
            m_Transform = _transform;
            m_FacingRight = _facingRight;
        }

        public void Update(float _axisMove)
        {
            if (m_Transform)
            {
                if (_axisMove > 0 && !m_FacingRight)
                    Flip();
                else if (_axisMove < 0 && m_FacingRight)
                    Flip();
            }
        }

        public void Flip()
        {
            m_FacingRight = !m_FacingRight;
            Vector3 scale = m_Transform.localScale;
            scale.x *= -1;
            m_Transform.localScale = scale;
        }

        #endregion
    }

}