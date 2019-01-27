Shader "Custom/PreOutlineShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
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

		Pass
		{
			Tags { "LightMode" = "Always" }
			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#include "UnitySprites.cginc"

			sampler2D _CameraDepthTexture;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct vert2frag
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};

			vert2frag vert(appdata IN)
			{
				vert2frag OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

#ifdef UNITY_INSTANCING_ENABLED
				IN.vertex.xy *= _Flip;
#endif

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
				OUT.screenPos = ComputeScreenPos(OUT.vertex);

				return OUT;
			}

			half4 frag(vert2frag IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;

				float depthValue = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
				if (depthValue - IN.vertex.z > 0.00001 || c.a < 0.5)
				{
					#ifdef FILL_OUTLINE
					return half4(0.5, 0.5, 0.5, 1.0);
					#else
					discard;
					#endif
				}

				return half4(1.0, 1.0, 1.0, 1.0);
				//return half4(depthValue, depthValue, depthValue, 1.0);0
			}
			ENDCG
		}
	}
}
