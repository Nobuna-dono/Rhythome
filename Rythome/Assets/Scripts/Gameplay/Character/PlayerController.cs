using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rhythome.Core;

namespace Rhythome.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        public InputBuilder InputBuilder;

        private void Start()
        {
            foreach (InputSelector input in InputBuilder.inputList)
            {
                ServiceSupervisor.Instance.Event.AddButtonListener(input.InputName, input.State, input.Type, input.Event);
            }
        }
    }
}