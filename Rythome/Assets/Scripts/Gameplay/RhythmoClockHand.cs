using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class RhythmoClockHand : RhythmObject
    {
        #region PROPERTIES
        [Tooltip("Angle to increment at each tick (6° for minutes, 30° for hours)")]
        public float Angle = 1f;

        public Vector2 PivotOffset = Vector2.zero;
        private Vector3 Pivot = Vector3.zero;
        private float timer = 0f;
        #endregion


        #region UNITY METHODS
        protected override void Start()
        {
            base.Start();
            Pivot = transform.position + new Vector3(PivotOffset.x, PivotOffset.y, 0f);
        }
        #endregion


        #region RHYTHOME METHODS
        protected override void Beat()
        {
            transform.RotateAround(Pivot, Vector3.back, Angle);

            if(m_TickSound)
            {
                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_TickSound;
                m_Source.Play();
            }
        }

        protected override void HalfBeat()
        {
            if (m_TockSound)
            {
                if (m_Source.isPlaying)
                {
                    m_Source.Stop();
                }
                m_Source.clip = m_TockSound;
                m_Source.Play();
            }
        }
        #endregion

    }
}
