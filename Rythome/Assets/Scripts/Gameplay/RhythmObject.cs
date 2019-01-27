using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class RhythmObject : MonoBehaviour
    {
        #region PROPERTIES
        [Header("- Rhythm Object Basic Sound:")]
        [SerializeField]
        protected AudioClip m_TickSound;
        [SerializeField]
        protected AudioClip m_TockSound;

        [Space(10)]
        [Header("- Rhythm Object Settings (Optional):")]
        [SerializeField]
        protected AudioSource m_Source;
        [SerializeField]
        protected Animator m_Animator;

        private int m_ZOrder = 0;
        public int ZOrder { get { return m_ZOrder; } }
        #endregion

        #region UNITY METHODS
        protected virtual void Start()
        {
            ServiceSupervisor.Instance.Rythm.OnTimeScaleUpdate += UpdateBPM;
            ServiceSupervisor.Instance.Rythm.OnBeat += Beat;
            ServiceSupervisor.Instance.Rythm.OnHalfBeat += HalfBeat;

            SpriteRenderer sprite;
            if(sprite = GetComponent<SpriteRenderer>())
            {
                m_ZOrder = SortingLayer.GetLayerValueFromID(sprite.sortingLayerID);
            }

            if ((m_Source = GetComponent<AudioSource>()) == null)
            {
                m_Source = gameObject.AddComponent<AudioSource>();
                m_Source.spatialize = true;
                m_Source.spatialBlend = 0.5f;
            }

            m_Animator = GetComponent<Animator>();
            if (m_Animator == null)
            {
                Debug.LogWarning("Failed to get 'Animator' component. " + gameObject);
            }

            UpdateBPM(ServiceSupervisor.Instance.Rythm.BPM / 60f);
        }
        #endregion

        #region RHYTHOME METHODS
        public virtual void StartBeat()
        { }

        public virtual void StopBeat()
        { }

        protected virtual void Beat()
        { }

        protected virtual void HalfBeat()
        { }

        public virtual void EnableFeedback(bool _enable)
        { }

        public virtual void UpdateBPM(float _newBPMScale)
        {
            if(m_Animator)
            {
                m_Animator.speed = _newBPMScale;
            }
        }

        public virtual void RythmoPlay1()
        { }

        public virtual void RythmoPlay2()
        { }

        public virtual void RythmoPlay3()
        { }
        #endregion
    }
}
