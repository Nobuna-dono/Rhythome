using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythome.Gameplay
{
    /* Manage the clock tick ! */
    public class RhythmoSupervisor : MonoBehaviour
    {
        #region PROPERTIES
        [SerializeField]
        [Range(1f, 240f)]
        private float m_BPM = 60f; // 1 = 60bpm
        public float BPM { get { return m_BPM; } }

        //List<RythmObject> m_RythmObjects;
        public delegate void TimeScaleUpdate_Delegate(float _newTimeScale);
        public TimeScaleUpdate_Delegate OnTimeScaleUpdate;

        public delegate void Beat_Delegate();
        public event Beat_Delegate OnBeat;
        public event Beat_Delegate OnHalfBeat;

        private bool m_IsBeatAtEnd = false;

        float CurrentTimeBuffer = 0;
        #endregion

        // Use this for initialization
        void Start()
        {
            //m_RythmObjects = new List<RythmObject>();
            //m_RythmObjects.AddRange(FindObjectsOfType<RythmObject>());
        }

        // Update is called once per frame
        void Update()
        {
            CurrentTimeBuffer += Time.deltaTime;

            if (CurrentTimeBuffer >= m_BPM / 60f)
            {
                if(OnBeat != null)
                {
                    OnBeat();
                }
                m_IsBeatAtEnd = false;

                CurrentTimeBuffer = 0;
            }
            else if (!m_IsBeatAtEnd && CurrentTimeBuffer >= (m_BPM / 60f) * 0.5f)
            {
                if (OnHalfBeat != null)
                {
                    OnHalfBeat();
                }
                m_IsBeatAtEnd = true;
            }
        }

        #region CUSTOM METHODS
        public void UpdateBPM(float _newBPM)
        {
            OnTimeScaleUpdate(m_BPM = _newBPM / 60f);
        }
        #endregion
    }
}
