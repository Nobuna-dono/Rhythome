using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class RhythmObject : MonoBehaviour
    {
        #region PROPERTIES
        [SerializeField]
        private Animator Animator;

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
                m_ZOrder = sprite.sortingOrder;
            }
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

        public virtual void UpdateBPM(float _newBPM)
        { }

        public virtual void RythmoPlay1()
        { }

        public virtual void RythmoPlay2()
        { }

        public virtual void RythmoPlay3()
        { }
        #endregion
    }
}
