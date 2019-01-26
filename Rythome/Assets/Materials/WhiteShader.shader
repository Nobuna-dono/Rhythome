Shader "Custom/OutlineShader"
{
	Properties
	{
		[Toggle(FILL_OUTLINE)] _FillOutline("Fill", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque"}
		LOD 100

		ZWrite Off
		Lighting Off
		ZTest Less

		Pass
		{
			Tags { "LightMode" = "Always" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature FILL_OUTLINE
			
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				return o;
			}
			
			half frag (v2f i) : SV_Target
			{
				float depthValue = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
				if (depthValue - i.vertex.z > 0.00001)
				{
					#ifdef FILL_OUTLINE
					return 0.5;
					#else
					discard;
					#endif
				}

				return 1.0;
				//return half4(depthValue, depthValue, depthValue, 1.0);
			}
			ENDCG
		}
	}
}
