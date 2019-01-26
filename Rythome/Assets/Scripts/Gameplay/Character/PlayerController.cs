using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rythome.Core;

namespace Rythome.Character
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        public InputBuilder InputBuilder;

        private void Start()
        {
            foreach (InputSelector input in InputBuilder.inputList)
            {
                ServiceSupervisor.Instance.EventWrapper.AddButtonListener(input.InputName, input.State, input.Type, input.Event);
            }
        }
    }
}