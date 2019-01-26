Shader "Custom/PostOutlineShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_OutlineCutoff("Outline Cutoff", Float) = 0.5
		_OutlineWidth("Outline Width", Float) = 1
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
	}
	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;

			fixed _OutlineCutoff;
			fixed _OutlineWidth;
			fixed4 _OutlineColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if (tex2D(_MainTex, i.uv.xy).a > _OutlineCutoff)
					discard;

				fixed intensity = 0.0;

				// For each surrounding pixel
				for (fixed k = -_OutlineWidth ; k < _OutlineWidth; ++k)
				{
					for (fixed j = -_OutlineWidth ; j < _OutlineWidth; ++j)
					{
						fixed2 offset = fixed2(k, j);
						intensity += tex2D(_MainTex, i.uv.xy + offset * _MainTex_TexelSize).a;
					}
				}

				intensity = clamp(intensity, 0.0, 1.0);
				return intensity * _OutlineColor;
			}
			ENDCG
		}
	}
}
