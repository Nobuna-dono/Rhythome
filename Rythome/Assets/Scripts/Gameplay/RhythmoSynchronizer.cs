using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Cancel
        }
        #endregion


        #region CLASSES
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
        RhythmoPawn RPawn;
        RhythmoStation RStation;

        public List<RhythmMark> RhythmPattern = new List<RhythmMark>(10);

        private int m_CurrentMark = 0;
        private float m_PlayNoteCount = 0;

        private float m_BeatCountBuffer = 0;
        //private bool m_IsHalfBeat = false;
        #endregion

        #region UNITY METHODS
        // Update is called once per frame
        protected override void Beat()
        {
            if(--m_PlayNoteCount <= 0f)
            {
                PlayNote();
            }

            m_BeatCountBuffer++;
           // m_IsHalfBeat = false;
        }

        /*protected override void HalfBeat()
        {
            if (m_PlayNoteCount == 0.5f)
            {
                PlayNote();
                m_PlayNoteCount = 0f;
            }

            m_IsHalfBeat = true;
        }*/
        #endregion


        #region CUSTOM METHODS
        public override void RythmoPlay1()
        {
            if(RPawn)
            {
                RPawn.RythmoPlay1();
            }
            if(RStation)
            {
                RStation.RythmoPlay1();
            }
        }

        public override void RythmoPlay2()
        {
            if (RPawn)
            {
                RPawn.RythmoPlay2();
            }
            if (RStation)
            {
                RStation.RythmoPlay2();
            }
        }

        public override void RythmoPlay3()
        {
            if (RPawn)
            {
                RPawn.RythmoPlay3();
            }
            if (RStation)
            {
                RStation.RythmoPlay3();
            }
        }

        void AddNote(EMarkType _mark)
        {
            float beatOffset = m_BeatCountBuffer;// + (m_IsHalfBeat ? 0.5f : 0f);

            if (RhythmPattern.Count > 0)
            {
                RhythmPattern[RhythmPattern.Count].BeatBeforeNextNote = beatOffset;
            }

            switch (_mark)
            {
                case EMarkType.Cancel:
                    UndoRhythmAndExit();
                    return;
                case EMarkType.Save:
                    SaveRhythmAndExit();
                    break;
                default:
                    break;
            }

            RhythmPattern.Add(new RhythmMark(_mark, 0));
        }

        void PlayNote()
        {
            switch(RhythmPattern[m_CurrentMark].Note)
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
            RStation.StartBeat();
        }

        void UndoRhythmAndExit()
        {
            RhythmPattern.Clear();
            RStation.StopBeat();
        }

        void NextMark()
        {
            m_CurrentMark++;
            m_PlayNoteCount = RhythmPattern[m_CurrentMark].BeatBeforeNextNote;
        }
        #endregion

    }
}
