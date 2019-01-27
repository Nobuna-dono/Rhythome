using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythome.Gameplay
{
    /* Manage the clock tick ! */
    public class RhythmoSupervisor : MonoBehaviour
    {
        #region PROPERTIES
        private float m_BPM = 60f; // 1 = 60bpm
        public float BPM { get { return m_BPM; } }

        [SerializeField]
        [Range(1f, 320f)]
        private float FORCED_BPM = 60f;

        public Material m_GhostMaterial;


        //List<RythmObject> m_RythmObjects;
        public delegate void TimeScaleUpdate_Delegate(float _newTimeScale);
        public TimeScaleUpdate_Delegate OnTimeScaleUpdate;

        public delegate void Beat_Delegate();
        public event Beat_Delegate OnBeat;
        public event Beat_Delegate OnHalfBeat;

        private bool m_IsBeatAtEnd = false;

        float CurrentTimeBuffer = 0;
        #endregion

        // Update is called once per frame
        void Update()
        {
            if(m_BPM != FORCED_BPM)
            {
                UpdateBPM(FORCED_BPM);
            }

            CurrentTimeBuffer += Time.deltaTime;

            if (CurrentTimeBuffer >=   60f / m_BPM)
            {
                if(OnBeat != null)
                {
                    OnBeat();
                }
                m_IsBeatAtEnd = false;

                CurrentTimeBuffer = 0;
            }
            else if (!m_IsBeatAtEnd && CurrentTimeBuffer >= (60f / m_BPM) * 0.5f)
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
            if (_newBPM <= 1)
            {
                _newBPM = 1;
            }

            OnTimeScaleUpdate((m_BPM = _newBPM) / 60f);
        }
        #endregion
    }
}
