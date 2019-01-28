using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythome.Gameplay
{
    /* Manage the clock tick ! */
    public class RhythmoSupervisor : MonoBehaviour
    {
        #region PROPERTIES
        private int m_BPM = 80; // 1 = 60bpm
        public int BPM { get { return m_BPM; } }

        [SerializeField]
        private int MaxBMP = 500;
        [SerializeField]
        private int MinBMP = 80;
        [SerializeField]
        [Range(1, 320)]
        private int FORCED_BPM = 60;

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
        public void UpdateBPM(int _newBPM)
        {
            Mathf.Clamp(_newBPM, MinBMP, MaxBMP);

            FORCED_BPM = m_BPM = _newBPM;
            if (OnTimeScaleUpdate != null)
            {
                OnTimeScaleUpdate(m_BPM / 60f);
            }
        }
        #endregion
    }
}
