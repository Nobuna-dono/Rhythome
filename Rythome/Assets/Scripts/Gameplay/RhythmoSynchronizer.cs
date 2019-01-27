using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class RhythmoSynchronizer : RhythmObject
    {
        #region ENUMS
        public enum EMarkType
        {
            Rythm1,
            Rythm2,
            Rythm3,
            Save,
            Back
        }
        #endregion


        #region CLASSES
        [System.Serializable]
        public class RhythmMark
        {
            public RhythmMark(EMarkType _noteIndex, float _beatElasped)
            {
                Note = _noteIndex;
                BeatBeforeNextNote = _beatElasped;
            }

            public EMarkType Note;
            public float BeatBeforeNextNote;
        }
        #endregion


        #region PROPERTIES
        RhythmoStation RStation;


        private Material m_ClassicMat;
        private SpriteRenderer m_SpriteRenderer;

        [SerializeField]
        private List<RhythmMark> RhythmPattern = new List<RhythmMark>(10);

        private int m_CurrentMark = 0;
        private float m_PlayNoteCount = 0;

        private float m_BeatCountBuffer = 0;
        private bool m_CanBeat = false;
        private bool m_AutoPlay = false;
        private bool m_FirstAutoPlay = false;

        private bool m_IsHalfBeat = false;
        #endregion

        #region UNITY METHODS
        protected override void Start()
        {
            base.Start();

            if(!(RStation = GetComponent<RhythmoStation>()))
            {
                Debug.LogError("No Station bind to the syncrhonizer...");
            }

            if (!(m_SpriteRenderer = GetComponent<SpriteRenderer>()))
            {
                Debug.LogError("No m_SpriteRenderer bind to the syncrhonizer...");
            }
            else
            {
                m_ClassicMat = m_SpriteRenderer.material;
            }
        }
        #endregion

        #region RHYTHOME METHODS
        public override void StartBeat()
        {
            if (RhythmPattern.Count > 0)
            {
                m_CanBeat = m_AutoPlay = false;
            }
            RhythmPattern.Clear();

            GetComponent<RhythmoStation>().StartBeat();

            m_SpriteRenderer.material = m_ClassicMat;
        }

        protected override void Beat()
        {
            if (m_FirstAutoPlay)
            {
                GetComponent<RhythmoStation>().StartBeat();
                m_FirstAutoPlay = false;
            }

            if (m_CanBeat)
            {
                if(m_AutoPlay)
                {
                    if (m_PlayNoteCount >= 1f)
                    {
                        if (--m_PlayNoteCount == 0)
                        {
                            PlayNote();
                        }
                    }
                }
                else
                {
                    m_BeatCountBuffer++;
                    m_IsHalfBeat = false;
                }
            }
        }

        protected override void HalfBeat()
        {
            if (m_CanBeat)
            {
                if (m_PlayNoteCount == 0.5f)
                {
                    m_PlayNoteCount = 0f;
                    PlayNote();
                }
                else
                {
                    m_IsHalfBeat = true;
                }
            }
        }

        public override void RythmoPlay1()
        {
            if(RStation)
            {
                RStation.RythmoPlay1();
            }
        }

        public override void RythmoPlay2()
        {
            if (RStation)
            {
                RStation.RythmoPlay2();
            }
        }

        public override void RythmoPlay3()
        {
            if (RStation)
            {
                RStation.RythmoPlay3();
            }
        }
        #endregion


        #region CUSTOM METHODS
        public void AddNote(EMarkType _mark)
        {
            if (RhythmPattern.Count > 0)
            {
                RhythmPattern[RhythmPattern.Count - 1].BeatBeforeNextNote = m_BeatCountBuffer + (m_IsHalfBeat ? 0.5f : 0f);
                if (RhythmPattern[RhythmPattern.Count - 1].BeatBeforeNextNote == 0)
                    RhythmPattern[RhythmPattern.Count - 1].BeatBeforeNextNote = 0.5f;
            }

            m_BeatCountBuffer = 0;

            switch (_mark)
            {
                case EMarkType.Rythm1:
                    RythmoPlay1();
                    break;
                case EMarkType.Rythm2:
                    RythmoPlay2();
                    break;
                case EMarkType.Rythm3:
                    RythmoPlay3();
                    break;
                case EMarkType.Back:
                    UndoRhythmAndExit();
                    return;
                case EMarkType.Save:
                    SaveRhythmAndExit();
                    return;
                default:
                    break;
            }

            RhythmPattern.Add(new RhythmMark(_mark, 0));
            m_CanBeat = true;
        }

        void PlayNote()
        {
            if (RhythmPattern.Count == 0 || RhythmPattern.Count <= m_CurrentMark)
                return;

            switch (RhythmPattern[m_CurrentMark].Note)
            {
                case EMarkType.Rythm1:
                    RythmoPlay1();
                    break;
                case EMarkType.Rythm2:
                    RythmoPlay2();
                    break;
                case EMarkType.Rythm3:
                    RythmoPlay3();
                    break;
                case EMarkType.Save:
                    SaveRhythmAndExit();
                    break;
                default: // CANCEL
                    UndoRhythmAndExit();
                    break;
            }
            NextMark();
        }

        void SaveRhythmAndExit()
        {
            m_CanBeat = true;
            m_AutoPlay = true;
            m_FirstAutoPlay = true;
            m_CurrentMark = 0;
            m_PlayNoteCount = RhythmPattern[m_CurrentMark].BeatBeforeNextNote;

            m_SpriteRenderer.material = ServiceSupervisor.Instance.Rythm.m_GhostMaterial;
        }

       

        void UndoRhythmAndExit()
        {
            m_CanBeat = false;
            RhythmPattern.Clear();
            RStation.StopBeat();
        }

        void NextMark()
        {
            m_PlayNoteCount = RhythmPattern[m_CurrentMark].BeatBeforeNextNote;

            if(++m_CurrentMark >= RhythmPattern.Count)
            {
                m_CurrentMark = 0;
            }
        }
        #endregion

    }
}
