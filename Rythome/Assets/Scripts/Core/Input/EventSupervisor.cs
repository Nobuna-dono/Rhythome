using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rythome.Core
{
    public static class ActionExtension
    {
        public static void SafeCall(this Action _action)
        {
            if (null != _action)
                _action();
        }

        public static void SafeCall(this UnityEvent _action)
        {
            if (null != _action)
                _action.Invoke();
        }
    }

    public enum EInputState
    {
        Pressed,
        Stay,
        Released,
        Axis
    }
    public enum EButtonType
    {
        Positive,
        Negative,
        Both
    }

    public class EventSupervisor : MonoBehaviour
    {
        #region Intern Class Field

        [Serializable]
        public class UInputButton
        {
            #region Intern Variables Field
            public string Name;
            public Action OnPositivePressedEvent, OnPositiveStayEvent, OnPositiveReleasedEvent;
            public Action OnNegativePressedEvent, OnNegativeStayEvent, OnNegativeReleasedEvent;
            public bool bPositivePressed = false;
            public bool bNegativePressed = false;
            #endregion

            #region Custom Methods Field
            public virtual void Update()
            {
                if (Input.GetAxis(Name) > 0)
                {
                    bPositivePressed = true;
                    if (OnPositivePressedEvent != null && Input.GetButtonDown(Name))
                        OnPositivePressedEvent.SafeCall();
                    else if (OnPositiveStayEvent != null)
                        OnPositiveStayEvent.SafeCall();
                }
                else if (Input.GetAxis(Name) < 0)
                {
                    bNegativePressed = true;
                    if (OnNegativePressedEvent != null && Input.GetButtonDown(Name))
                        OnNegativePressedEvent.SafeCall();
                    else if (OnNegativeStayEvent != null)
                        OnNegativeStayEvent.SafeCall();
                }
                else if (bPositivePressed)
                {
                    if (OnPositiveReleasedEvent != null && Input.GetButtonUp(Name))
                        OnPositiveReleasedEvent.SafeCall();
                    bPositivePressed = false;
                }
                else if (bNegativePressed)
                {
                    if (OnNegativeReleasedEvent != null && Input.GetButtonUp(Name))
                        OnNegativeReleasedEvent.SafeCall();
                    bNegativePressed = false;
                }
            }

            public virtual void Reset()
            {
                OnPositivePressedEvent = OnPositiveReleasedEvent = OnPositiveStayEvent = null;
                OnNegativePressedEvent = OnNegativeReleasedEvent = OnNegativeStayEvent = null;
            }
            #endregion
        }

        [Serializable]
        public class UInputAxis : UInputButton
        {
            #region Intern Variables Field
            public event AxisProtocol AxisEvent;
            public float m_value = 0.0f;
            public float m_prevValue = 0.0f;

            private bool bLastUpdate = false;
            #endregion

            #region Custom Methods Field
            public override void Update()
            {
                m_prevValue = m_value;
                m_value = Input.GetAxis(Name);

                if (m_value != 0)
                {
                    EventSafeCall(m_value);
                    bLastUpdate = false;
                }
                else if (!bLastUpdate && m_value == 0)
                {
                    EventSafeCall(0);
                    bLastUpdate = true;
                }

                if (m_value == 0)
                    bNegativePressed = bPositivePressed = false;
                if ((OnPositiveReleasedEvent != null && OnNegativeReleasedEvent != null) && m_prevValue != 0 && m_value == 0)
                {
                    if (bPositivePressed)
                        OnPositiveReleasedEvent.SafeCall();
                    if (bNegativePressed)
                        OnNegativeReleasedEvent.SafeCall();
                }
                else if (OnPositivePressedEvent != null && m_prevValue == 0 && m_value > 0)
                {
                    bPositivePressed = true;
                    OnPositivePressedEvent.SafeCall();
                }
                else if (OnNegativePressedEvent != null && m_prevValue == 0 && m_value < 0)
                {
                    bNegativePressed = true;
                    OnNegativePressedEvent.SafeCall();
                }

                if (OnPositiveStayEvent != null && bPositivePressed == true)
                    OnPositiveStayEvent.SafeCall();
                if (OnNegativeStayEvent != null && bNegativePressed == true)
                    OnNegativeStayEvent.SafeCall();
            }

            public override void Reset()
            {
                base.Reset();
                AxisEvent = null;
                m_value = m_prevValue = 0f;
            }

            public void EventSafeCall(float _value)
            {
                if (AxisEvent != null)
                    AxisEvent(_value);
            }
            #endregion
        }

        #endregion



        #region Intern Properties Field

        // Serialized 
        [SerializeField]
        private List<UInputButton> m_Buttons = new List<UInputButton>();
        [SerializeField]
        private List<UInputAxis> m_Axis = new List<UInputAxis>();

        // Delegate
        public delegate void Protocol();
        public delegate void AxisProtocol(float _value);

        // Active event update ? see SetActiveUpdate()
        private bool m_bActiveUpdate = true;

        #endregion



        #region Unity Methods Field            
        // To avoid frames creation 
        private int m_IndexBuffer = 0;
        private int m_CountBuffer = 0;
        private void Update()
        {
            if (!m_bActiveUpdate)
                return;

            m_CountBuffer = m_Buttons.Count;
            for (m_IndexBuffer = 0; m_IndexBuffer < m_CountBuffer; m_IndexBuffer++)
                m_Buttons[m_IndexBuffer].Update();

            m_CountBuffer = m_Axis.Count;
            for (m_IndexBuffer = 0; m_IndexBuffer < m_CountBuffer; m_IndexBuffer++)
                m_Axis[m_IndexBuffer].Update();
        }

        #endregion



        #region Custom Methods Field

        public void Initialize(List<string> _buttons, List<string> _axes)
        {
            foreach (string name in _buttons)
            {
                UInputButton but = new UInputButton();
                but.Name = name;
                m_Buttons.Add(but);
            }

            foreach (string name in _axes)
            {
                UInputAxis axis = new UInputAxis();
                axis.Name = name;
                m_Axis.Add(axis);
            }

        }

        #region Listeners

        public void AddAxisListener(string _axisName, AxisProtocol _func)
        {
            if (!CheckAxis(_axisName))
                AddAxis(_axisName);
            for (int i = 0; i < m_Axis.Count; i++)
            {
                if (m_Axis[i].Name == _axisName)
                    m_Axis[i].AxisEvent += _func.Invoke;
            }
        }

        public void AddButtonListener(string _buttonName, EInputState _state, EButtonType _type, Protocol _func)
        {
            if (!CheckButton(_buttonName))
                AddButton(_buttonName);

            switch (_state)
            {
                case EInputState.Pressed:
                    AddPressedListener(_buttonName, _type, _func);
                    break;
                case EInputState.Stay:
                    AddStayPressedListener(_buttonName, _type, _func);
                    break;
                case EInputState.Released:
                    AddReleasedListener(_buttonName, _type, _func);
                    break;
                default: break;
            }
        }
        private void AddPressedListener(string _buttonName, EButtonType _type, Protocol _func)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Buttons[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Buttons[i].OnPositivePressedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Buttons[i].OnNegativePressedEvent += _func.Invoke;
                }
            }

            count = m_Axis.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Axis[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Axis[i].OnPositivePressedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Axis[i].OnNegativePressedEvent += _func.Invoke;
                }
            }
        }
        private void AddStayPressedListener(string _buttonName, EButtonType _type, Protocol _func)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Buttons[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Buttons[i].OnPositiveStayEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Buttons[i].OnNegativeStayEvent += _func.Invoke;
                }
            }

            count = m_Axis.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Axis[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Axis[i].OnPositiveStayEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Axis[i].OnNegativeStayEvent += _func.Invoke;
                }
            }
        }
        private void AddReleasedListener(string _buttonName, EButtonType _type, Protocol _func)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Buttons[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Buttons[i].OnPositiveReleasedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Buttons[i].OnNegativeReleasedEvent += _func.Invoke;
                }
            }

            count = m_Axis.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Axis[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Axis[i].OnPositiveReleasedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Axis[i].OnNegativeReleasedEvent += _func.Invoke;
                }
            }
        }

        public void AddButtonListener(string _buttonName, EInputState _state, EButtonType _type, UnityEvent _func)
        {
            if (!CheckButton(_buttonName))
                AddButton(_buttonName);

            switch (_state)
            {
                case EInputState.Pressed:
                    AddPressedListener(_buttonName, _type, _func);
                    break;
                case EInputState.Stay:
                    AddStayPressedListener(_buttonName, _type, _func);
                    break;
                case EInputState.Released:
                    AddReleasedListener(_buttonName, _type, _func);
                    break;
                default: break;
            }
        }
        private void AddPressedListener(string _buttonName, EButtonType _type, UnityEvent _func)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Buttons[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Buttons[i].OnPositivePressedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Buttons[i].OnNegativePressedEvent += _func.Invoke;
                }
            }

            count = m_Axis.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Axis[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Axis[i].OnPositivePressedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Axis[i].OnNegativePressedEvent += _func.Invoke;
                }
            }
        }
        private void AddStayPressedListener(string _buttonName, EButtonType _type, UnityEvent _func)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Buttons[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Buttons[i].OnPositiveStayEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Buttons[i].OnNegativeStayEvent += _func.Invoke;
                }
            }

            count = m_Axis.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Axis[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Axis[i].OnPositiveStayEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Axis[i].OnNegativeStayEvent += _func.Invoke;
                }
            }
        }
        private void AddReleasedListener(string _buttonName, EButtonType _type, UnityEvent _func)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Buttons[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Buttons[i].OnPositiveReleasedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Buttons[i].OnNegativeReleasedEvent += _func.Invoke;
                }
            }

            count = m_Axis.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Axis[i].Name == _buttonName)
                {
                    if (_type == EButtonType.Positive || _type == EButtonType.Both)
                        m_Axis[i].OnPositiveReleasedEvent += _func.Invoke;
                    else if (_type == EButtonType.Negative || _type == EButtonType.Both)
                        m_Axis[i].OnNegativeReleasedEvent += _func.Invoke;
                }
            }
        }

        #endregion

        public float GetAxisByName(string _name)
        {
            for (int i = 0; i < m_Axis.Count; i++)
            {
                if (m_Axis[i].Name == _name)
                {
                    return m_Axis[i].m_value;
                }
            }
            return 0f;
        }

        private bool CheckButton(string _buttonName)
        {
            int count = m_Buttons.Count;
            for (int i = 0; i < count; i++)
                if (m_Buttons[i].Name == _buttonName)
                    return true;
            return false;
        }
        private bool CheckAxis(string _axisName)
        {
            int count = m_Axis.Count;
            for (int i = 0; i < count; i++)
                if (m_Axis[i].Name == _axisName)
                    return true;
            return false;
        }

        private void AddButton(string _buttonName)
        {
            UInputButton but = new UInputButton();
            but.Name = _buttonName;
            m_Buttons.Add(but);
        }
        private void AddAxis(string _axisName)
        {
            UInputAxis ax = new UInputAxis();
            ax.Name = _axisName;
            m_Axis.Add(ax);
        }

        public void SetActiveUpdate(bool _bvalue)
        {
            m_bActiveUpdate = _bvalue;
        }

        public void Reset()
        {
            foreach (UInputButton button in m_Buttons)
            {
                button.Reset();
            }
            foreach (UInputAxis axis in m_Axis)
            {
                axis.Reset();
            }
        }
        #endregion
    }
}
