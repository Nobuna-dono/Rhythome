using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rhythome.Core
{
    [System.Serializable]
    public struct InputBuilder
    {
        public List<InputSelector> inputList;
    }

    [System.Serializable]
    public struct InputSelector
    {
        //public enum EInputType
        //{
        //	Positive,
        //	Negative,

        //	Count
        //}

        public string DescriptiveName;
        public string InputName;
        public EButtonType Type;
        public EInputState State;
        public UnityEvent Event;
    }
}