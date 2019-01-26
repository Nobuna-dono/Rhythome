using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameData;

namespace Rythome.Enhance
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class FadePostProcess : MonoBehaviour
    {
        private Shader m_Shader;
        private Material m_Material;
        [SerializeField]
        private Texture2D m_DefaultMask = null;
        public SOFadeAnimationData DefaultFadeData = null;


        [SerializeField]
        private bool m_bFadeOut = false;
        public bool bFadeOut
        {
            get { return m_bFadeOut; }
        }

        [SerializeField]
        private bool m_bAnimate = false;
        [SerializeField]
        [Range(0, 1.0f)]
        private float m_MaskValue;

        private float m_TimeStamp = 0;

        #region DEBUG FIELD
        [Header("- DEBUG FIELD:")]
        [SerializeField]
        private AnimationCurve m_AnimationCurve;
        [SerializeField]
        [Range(0f, 10f)]
        private float m_AnimationSpeed;

        [SerializeField]
        private Color m_MaskColor = Color.black;
        [SerializeField]
        private Texture2D m_MaskTexture;
        [SerializeField]
        private bool m_bUseCustomColor;
        #endregion

        Material material
        {
            get
            {
                if (m_Material == null)
                {
                    m_Material = new Material(m_Shader);
                    m_Material.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_Material;
            }
        }

        #region UNITY FIELD
        private void Awake()
        {
            if (m_DefaultMask == null)
                m_DefaultMask = Resources.Load<Texture2D>("Textures/Masks/Mask13");
        }

        void Start()
        {
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }

            m_Shader = Shader.Find("Hidden/ImageEffectShaderTransition");

            // Disable the image effect if the shader can't
            // run on the users graphics card
            if (m_Shader == null || !m_Shader.isSupported)
                enabled = false;
        }

        void OnDisable()
        {
            if (m_Material)
            {
                DestroyImmediate(m_Material);
            }
        }

        private void FixedUpdate()
        {
            if (m_bAnimate)
            {
                if (!m_bFadeOut)
                    m_TimeStamp += Time.deltaTime * m_AnimationSpeed;
                else
                    m_TimeStamp -= Time.deltaTime * m_AnimationSpeed;

                m_MaskValue = Mathf.Clamp01(m_AnimationCurve.Evaluate(m_TimeStamp));

                if ((!m_bFadeOut && m_TimeStamp >= 1) || (m_bFadeOut && m_TimeStamp <= 0))
                    m_bAnimate = false;
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!enabled)
            {
                Graphics.Blit(source, destination);
                return;
            }

            material.SetColor("_MaskColor", m_MaskColor);
            material.SetFloat("_MaskValue", m_MaskValue);
            material.SetTexture("_MainTex", source);
            material.SetTexture("_MaskTex", m_MaskTexture);
            material.SetFloat("_CustomColor", m_bUseCustomColor ? 1 : 0);

            Graphics.Blit(source, destination, material);
        }
        #endregion

        #region CUSTOM FIELD
        public void SetFadeEffectData(SOFadeAnimationData _data, bool _fadeOut)
        {
            if (!_fadeOut)
                m_MaskValue = m_TimeStamp = 0;
            else
                m_MaskValue = m_TimeStamp = 1;

            if (_data)
            {
                m_AnimationSpeed = _data.AnimationSpeed;
                m_AnimationCurve = _data.Animation;
                m_MaskTexture = _data.MaskTexture;
                if (m_bUseCustomColor = _data.bUseCustomColor)
                    m_MaskColor = _data.MaskCustomColor;
            }
            else if (DefaultFadeData)
            {
                m_AnimationSpeed = DefaultFadeData.AnimationSpeed;
                m_AnimationCurve = DefaultFadeData.Animation;
                m_MaskTexture = DefaultFadeData.MaskTexture;
                if (m_bUseCustomColor = DefaultFadeData.bUseCustomColor)
                    m_MaskColor = DefaultFadeData.MaskCustomColor;
            }
            else
            {
                m_AnimationSpeed = 1f;
                m_AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                m_MaskTexture = m_DefaultMask;
                m_bUseCustomColor = true;
                m_MaskColor = Color.black;

            }
            m_bFadeOut = _fadeOut;
            m_bAnimate = true;
        }

        public void InverseFade()
        {
            m_bFadeOut = !m_bFadeOut;
            m_bAnimate = true;
        }
        #endregion
    }
}