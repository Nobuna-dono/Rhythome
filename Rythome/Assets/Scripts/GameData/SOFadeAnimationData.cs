using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(fileName = "[FadeAnimation]_", menuName = "Procedural Data/FadeAnimationData", order = 1)]
    public class SOFadeAnimationData : ScriptableObject
    {        
        [Header("- Transition Effect Description:")]
        public Texture2D MaskTexture;

        [Range(0.1f,10f)]
        public float AnimationSpeed;
        [Tooltip("Allow to manage mask animation. Only values between 0 and 1 (axis value) will be evaluated.")]
        public AnimationCurve Animation;

        [Space(10)]
        [Tooltip("If true play the invert animation at the end of the reaction.")]
        public Color MaskCustomColor;
        public bool bUseCustomColor;

        [Space(10)]
        public bool bInvertAfterReaction;
        [Header("- WARNING: may cause bugs. Please read the tips/description.")]
        public bool bCanHideFrontEnd;
        [Tooltip("Must hide the dash board? Be carefull and keep in mind to use it with @bInvertAfterReaction if you're not sure what you're doing.")]
        public bool bHideDashBoard;
        [Tooltip("Must hide the communication board? Be carefull and keep in mind to use it with @bInvertAfterReaction if you're not sure what you're doing.")]
        public bool bHideCommunicationBoard;
    }
}
