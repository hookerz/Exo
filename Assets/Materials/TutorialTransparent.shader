Shader "Unlit/TutorialTransparent"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_AlphaPower("Alpha Power", Range(0, 100)) = 1.0
		_ColorPower("Color Power", Range(0, 100)) = 1.0
		_AlphaMin("Alpha Min", Range(0, 1)) = 0.0
		_AlphaMax("Alpha Max", Range(0, 1)) = 1.0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Cull Back
		Blend SrcAlpha One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
		
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 viewSpaceNormal : TEXCOORD1;
				float3 viewSpacePosition : TEXCOORD2;
			};

			fixed4 _Color;
			float _AlphaPower;
			float _ColorPower;
			float _AlphaMin;
			float _AlphaMax;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.viewSpaceNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				o.viewSpacePosition = mul(UNITY_MATRIX_MV, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.viewSpaceNormal);
				float3 viewDir = normalize(i.viewSpacePosition);
				float d = saturate(dot(-viewDir, normal));
				float apow = pow(d, _AlphaPower);
				float cpow = pow(d, _ColorPower);
				fixed4 col = lerp(_Color, fixed4(1.0, 1.0, 1.0, 1.0), cpow);
				float a = max(_AlphaMin, apow);
				a = min(_AlphaMax, a);
				return fixed4(col.r, col.g, col.b, a);
			}
			ENDCG
		}
	}
}
