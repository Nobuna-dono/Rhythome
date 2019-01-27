using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
	[SerializeField]
	private Camera m_originalCamera = null;

	[SerializeField]
	private Camera m_outlineCamera = null;
	[SerializeField]
	private Camera m_subtractCamera = null;
	private RenderTexture m_outlineRenderTexture = null;
	private RenderTexture m_subtractRenderTexture = null;

	[SerializeField]
	private LayerMask m_outlinedLayers = new LayerMask();
	[SerializeField]
	private LayerMask m_subtractedLayers = new LayerMask();

	[SerializeField]
	private Shader m_preOutlineShader = null;
	[SerializeField]
	private Shader m_postOutlineShader = null;
	[SerializeField]
	private Shader m_subtractOutlineShader = null;
	private Material m_postOutlineMaterial = null;
	private Material m_subtractOutlineMaterial = null;

	[SerializeField]
	private float m_outlineCutoff = 0.5f;

	[SerializeField]
	private float m_outlineWidth = 1f;

	[SerializeField]
	private Color m_outlineColor = Color.black;

	[SerializeField]
	private bool m_showSubtract = false, m_showOutline = false;

	private void OnEnable()
	{
		m_outlineCamera.enabled = false;
		m_subtractCamera.enabled = false;

		m_postOutlineMaterial = new Material(m_postOutlineShader);
		m_subtractOutlineMaterial = new Material(m_subtractOutlineShader);
		UpdateMaterial();
	}

	private void OnDisable()
	{
		DestroyRenderTexture(ref m_outlineRenderTexture);
		DestroyRenderTexture(ref m_subtractRenderTexture);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateRenderTexture(ref m_outlineRenderTexture, source);
		CreateRenderTexture(ref m_subtractRenderTexture, source);

		CopyCamera(m_outlineCamera, m_outlineRenderTexture, m_outlinedLayers);
		CopyCamera(m_subtractCamera, m_subtractRenderTexture, m_subtractedLayers);
		
		m_subtractCamera.RenderWithShader(m_preOutlineShader, "RenderType");
		m_outlineCamera.RenderWithShader(m_preOutlineShader, "RenderType");

		Graphics.Blit(m_subtractRenderTexture, m_outlineRenderTexture, m_subtractOutlineMaterial);
		if (m_showSubtract)
			Graphics.Blit(m_subtractRenderTexture, destination);
		else if (m_showOutline)
			Graphics.Blit(m_outlineRenderTexture, destination);
		else
		{
			Graphics.Blit(m_outlineRenderTexture, source, m_postOutlineMaterial);
			Graphics.Blit(source, destination);
		}
	}

	private void DestroyRenderTexture(ref RenderTexture texture)
	{
		if (texture)
		{
			texture.Release();
			texture = null;
		}
	}

	private void CreateRenderTexture(ref RenderTexture texture, RenderTexture source)
	{
		if (!texture)
		{
			texture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);
			texture.Create();
		}
		else if (texture.width != source.width || m_outlineRenderTexture.height != source.height)
		{
			texture.Release();
			texture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);
			texture.Create();
		}
	}

	private void CopyCamera(Camera cam, RenderTexture texture, LayerMask layerMask)
	{
		cam.CopyFrom(m_originalCamera);
		cam.cullingMask = layerMask;
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.backgroundColor = new Color(0, 0, 0, 0);
		cam.targetTexture = texture;
		cam.depth = -100;
		cam.depthTextureMode = DepthTextureMode.None;
	}

	private void OnValidate()
	{
		UpdateMaterial();
	}

	private void UpdateMaterial()
	{
		if (m_postOutlineMaterial)
		{
			m_postOutlineMaterial.SetFloat("_OutlineCutoff", m_outlineCutoff);
			m_postOutlineMaterial.SetFloat("_OutlineWidth", m_outlineWidth);
			m_postOutlineMaterial.SetColor("_OutlineColor", m_outlineColor);
		}
	}
}
