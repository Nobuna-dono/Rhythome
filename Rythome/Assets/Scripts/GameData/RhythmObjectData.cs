using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythome.Gameplay
{
    [CreateAssetMenu(fileName = "[RythmData]_", menuName = "Rythome", order = 0)]
    public class RythmData : ScriptableObject
    {
        [Header("- Rythm Data")]

        public AudioClip Rythm1;
        public AudioClip Rythm2;
        public AudioClip Rythm3;
    }
}
