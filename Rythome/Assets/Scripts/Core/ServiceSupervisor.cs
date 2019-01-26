using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rythome
{
	[System.Serializable]
	public struct FEventWrapperAttributes
	{
		public List<string> ButtonName;
		public List<string> AxisName;
	}

	namespace Core
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

            private EventSupervisor m_EventWrapper;
            public EventSupervisor EventWrapper
            {
                get
                {
                    if (m_EventWrapper == null)
                    {
                        //DebugWrapper.FLog.PrintError(this, "The Hell ?! Failed to create FEventWrapper ! Please inform a prog.");
                        return null;
                    }
                    return m_EventWrapper;
                }
            }

            //[Header("- Sound Supervisor:")]
            //public FSoundSupervisor Sound;

            // Temporary !!!
            [Space(20)]
            [Header("- State Machine:")]
            public EGameMode GameMode;
            #endregion

            void Start()
			{				
				m_Instance = FindObjectOfType<ServiceSupervisor>();
				m_EventWrapper = gameObject.AddComponent<EventSupervisor>();
				//m_EventWrapper.Initialize(m_EventWrapperAttributes.ButtonName, m_EventWrapperAttributes.AxisName);

            }

			void Update()
			{}
		}
	}
}