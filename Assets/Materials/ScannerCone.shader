Shader "Unlit/ScannerCone"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Noise Texture", 2D) = "white" {}
		_FadePower("Fade Power", Range(0, 10)) = 1.0
	}
		SubShader
	{
		Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Fog{ Mode Off }
		ZWrite Off
		Cull Off
		//ZTest Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;
			float _FadePower;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewSpaceNormal : TEXCOORD1;
				float3 viewSpacePosition : TEXCOORD2;
			};
				
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.viewSpaceNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				o.viewSpacePosition = mul(UNITY_MATRIX_MV, v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				float3 normal = normalize(i.viewSpaceNormal);
				float3 viewDir = normalize(i.viewSpacePosition);
				float d = saturate(dot(-viewDir, normal));
				float alpha = _Color.a * (1-pow(i.uv.y, _FadePower));
				return fixed4(_Color.r, _Color.g, _Color.b, alpha) * tex2D(_MainTex, i.uv + float2(_Time[3], 0));
			}

			ENDCG
		}
	}
}
