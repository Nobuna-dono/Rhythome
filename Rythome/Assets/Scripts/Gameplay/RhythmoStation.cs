﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythome.Gameplay
{
    public class RhythmoStation : RhythmObject
    {
        #region ENUMS
        public enum ERhythmoStationState
        {
            Idle_BeatOff,
            Idle_BeatOn,

            Idle_RhythmPlay,
            RhythmPlay1,
            RhythmPlay2,
            RhythmPlay3,
        }
        #endregion

        #region PROPERTIES
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

            if((m_Source = GetComponent<AudioSource>() ) == null)
            {
                m_Source = gameObject.AddComponent<AudioSource>();
                m_Source.spatialize = true;
            }
        }
        #endregion

        #region RHYTHOME METHODS
        public override void StartBeat()
        {
            // Add transition
            // bIsAllowedToBeat = true;
            // Animation Idle to
        }

        public override void StopBeat()
        {
            // Add transition
            //m_Mesh.enabled = true;
        }

        public override void RythmoPlay1()
        {
            // Animation
            if (m_Source.isPlaying)
            {
                m_Source.Stop();
            }
            m_Source.clip = m_Data.Rythm1;
        }

        public override void RythmoPlay2()
        {
            // Animation
            if (m_Data.Rythm2)
            {
                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_Data.Rythm2;
            }
        }

        public override void RythmoPlay3()
        {
            // Animation
            if (m_Data.Rythm3)
            {
                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_Data.Rythm3;
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
