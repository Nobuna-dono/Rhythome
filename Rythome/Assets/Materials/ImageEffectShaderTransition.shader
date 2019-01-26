Shader "Hidden/ImageEffectShaderTransition"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_MaskValue ("Mask Value", Range(0,1)) = 0.5
		_MaskColor ("Mask Color", Color) = (0,0,0,1)
		_CustomColor ("Custom Color", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			// Render Process tex
			sampler2D _MainTex;
			// Mask Tex use for effect
			sampler2D _MaskTex;
			// Percentage of the mask value
			float _MaskValue;
			float4 _MaskColor;
			float _CustomColor;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				float mask = tex2D(_MaskTex, i.uv).a;

				float alpha = mask * (1 - 1 / 255.0);


				float weight = step(_MaskValue, alpha);


				float3 finalColor = _MaskColor;

				if (!_CustomColor)
				{
					alpha = lerp(alpha, col.rgb * (0.05 + (0.1 / (_MaskValue + 1) - (_MaskValue / 10))), _MaskValue);
					finalColor = alpha;
				}
				
				col.rgb = lerp(col.rgb, lerp(finalColor, col.rgb, weight), _MaskColor.a);			

				return col;
			}
			ENDCG
		}
	}
}
