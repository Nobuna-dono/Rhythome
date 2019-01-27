using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class RhythmoStation : RhythmObject
    {
        #region ENUMS
        public enum ERhythmoStationState
        {
            Idle_BeatOff = 0,
            Idle_BeatOn = 1,

            Idle_RhythmPlay = 2,
            RhythmPlay1 = 3,
            RhythmPlay2 = 4,
            RhythmPlay3 = 5,
        }
        #endregion

        #region PROPERTIES7
        [Space(10)]
        [Header("- Rhythmo Station Settings:")]
        [SerializeField]
        private RythmData m_Data;

        [SerializeField]
        private StateMachineSyncDataOfInt StationState;
        #endregion


        #region UNITY METHODS
        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            if ((m_Source = GetComponent<AudioSource>()) == null)
            {
                m_Source = gameObject.AddComponent<AudioSource>();
                m_Source.spatialize = true;
            }

            StopBeat();
        }
        #endregion

        #region RHYTHOME METHODS
        public override void StartBeat()
        {
            StationState.SetData((int)ERhythmoStationState.Idle_BeatOn, m_Animator);
            ServiceSupervisor.Instance.Rythm.OnHalfBeat += HalfBeat;
        }

        public override void StopBeat()
        {
            ServiceSupervisor.Instance.Rythm.OnHalfBeat -= HalfBeat;
            StationState.SetData((int)ERhythmoStationState.Idle_BeatOff, m_Animator);
        }

        protected override void HalfBeat()
        {
            StationState.SetData((int)ERhythmoStationState.Idle_BeatOn, m_Animator);
        }

        public override void RythmoPlay1()
        {
            if (m_Data.Rythm1)
            {
                StationState.SetData((int)ERhythmoStationState.RhythmPlay1, m_Animator);

                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_Data.Rythm1;
                m_Source.Play();
            }
        }

        public override void RythmoPlay2()
        {
            if (m_Data.Rythm2)
            {
                StationState.SetData((int)ERhythmoStationState.RhythmPlay2, m_Animator);
                if (m_Data.Rythm2)
                {
                    if (m_Source.isPlaying)
                    {
                        m_Source.Stop();
                    }
                    m_Source.clip = m_Data.Rythm2;
                    m_Source.Play();
                }
            }
        }

        public override void RythmoPlay3()
        {
            if (m_Data.Rythm3)
            {
                StationState.SetData((int)ERhythmoStationState.RhythmPlay3, m_Animator);
                if (m_Data.Rythm3)
                {
                    if (m_Source.isPlaying)
                    {
                        m_Source.Stop();
                    }
                    m_Source.clip = m_Data.Rythm3;
                    m_Source.Play();
                }
            }
        }

        public override void EnableFeedback(bool _enable)
        {
            if (_enable)
            {
                gameObject.layer = 8;
            }
            else
            {
                gameObject.layer = 0;
            }
        }
        #endregion
    }
}
