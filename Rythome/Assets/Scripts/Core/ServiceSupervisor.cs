using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Gameplay;

namespace Rhythome.Core
{
    public enum EGameMode
    {
        MainMenu, InGame_Interaction, InGame_Navigation, InMenu
    }

    public class ServiceSupervisor : MonoBehaviour
    {
        #region Internal Properties Field
        private static ServiceSupervisor m_Instance;
        public static ServiceSupervisor Instance
        {
            get { return m_Instance; }
        }

        private EventSupervisor m_Event;
        public EventSupervisor Event
        {
            get
            {
                if (m_Event == null)
                {
                    return null;
                }
                return m_Event;
            }
        }

        [SerializeField]
        private RhythmoSupervisor m_Rythm = null;
        public RhythmoSupervisor Rythm
        {
            get
            {
                if (m_Rythm == null)
                {
                    return null;
                }
                return m_Rythm;
            }
        }

        //public FSoundSupervisor Sound;

        // Temporary !!!
        [Space(20)]
        [Header("- State Machine:")]
        public EGameMode GameMode;
        #endregion

        void Start()
        {
            Cursor.visible = false;
            m_Instance = FindObjectOfType<ServiceSupervisor>();
            m_Event = gameObject.AddComponent<EventSupervisor>();

            if ((m_Rythm = GetComponent<RhythmoSupervisor>()) == null)
            {
                m_Rythm = gameObject.AddComponent<RhythmoSupervisor>();
            }
        }
    }
}