using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythome.Gameplay
{

    public class AnimationBegin : MonoBehaviour
    {
        [SerializeField]
        private StateMachineSyncDataOfInt State;

        [SerializeField]
        Animator m_Animator;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetState(int _index)
        {
            if(m_Animator)
               State.SetData(_index, m_Animator);
        }
    }
}
