using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
	[SerializeField]
	private Camera m_originalCamera = null;

	[SerializeField]
	private Camera m_outlineCamera = null;
	private RenderTexture m_renderTexture = null;

	[SerializeField]
	private LayerMask m_outlinedLayers = new LayerMask();
	
	[SerializeField]
	private Shader m_postOutlineShader = null;
	private Material m_postOutlineMaterial = null;

	[SerializeField]
	private float m_outlineCutoff = 0.5f;

	[SerializeField]
	private float m_outlineWidth = 1f;

	[SerializeField]
	private Color m_outlineColor = Color.black;

	private void OnEnable()
	{
		if (!m_outlineCamera)
			m_outlineCamera = GetComponent<Camera>();

		m_outlineCamera.enabled = false;

		m_postOutlineMaterial = new Material(m_postOutlineShader);
		UpdateMaterial();
	}

	private void OnDisable()
	{
		if (m_renderTexture)
		{
			m_renderTexture.Release();
			m_renderTexture = null;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!m_renderTexture)
		{
			m_renderTexture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);
			m_renderTexture.Create();
		}
		else if (m_renderTexture.width != source.width || m_renderTexture.height != source.height)
		{
			m_renderTexture.Release();
			m_renderTexture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);
			m_renderTexture.Create();
		}

		m_outlineCamera.CopyFrom(m_originalCamera);
		m_outlineCamera.cullingMask = m_outlinedLayers;
		m_outlineCamera.clearFlags = CameraClearFlags.SolidColor;
		m_outlineCamera.backgroundColor = new Color(0, 0, 0, 0);
		m_outlineCamera.targetTexture = m_renderTexture;
		m_outlineCamera.depth = -100;
		m_outlineCamera.depthTextureMode = DepthTextureMode.None;

		m_outlineCamera.Render();

		Graphics.Blit(m_renderTexture, source, m_postOutlineMaterial);
		Graphics.Blit(source, destination);
	}

	private void OnValidate()
	{
		UpdateMaterial();
	}

	private void UpdateMaterial()
	{
		if (!m_postOutlineMaterial)
			return;

		m_postOutlineMaterial.SetFloat("_OutlineCutoff", m_outlineCutoff);
		m_postOutlineMaterial.SetFloat("_OutlineWidth", m_outlineWidth);
		m_postOutlineMaterial.SetColor("_OutlineColor", m_outlineColor);
	}
}
