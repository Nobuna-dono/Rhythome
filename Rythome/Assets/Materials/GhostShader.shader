Shader "Custom/GhostShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_GhostColor("Ghost Tint", Color) = (1,1,1)
		_BrightnessFactor("Brightness Factor", float) = 1.0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed3 _GhostColor;
			float _BrightnessFactor;

			fixed4 frag(v2f IN) : SV_TARGET
			{
				fixed4 c = SpriteFrag(IN);
				fixed lum = Luminance(c.rgb) * _BrightnessFactor;
				
				fixed4 OUT;
				OUT.rgb = fixed3(lum, lum, lum) * _GhostColor.rgb;
				OUT.a = c.a;
				return OUT;
			}
		ENDCG
		}
	}
}
