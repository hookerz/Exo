Shader "Unlit/LaserBall"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_AlphaPower("Alpha Power", Range(0, 100)) = 1.0
		_ColorPower("Color Power", Range(0, 100)) = 1.0
	}
	SubShader
	{
		// HACK: the queue is set to Transparent+1 so it draws AFTER the laser beam... HACK
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
		
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewSpaceNormal : TEXCOORD1;
				float3 viewSpacePosition : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _AlphaPower;
			float _ColorPower;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.viewSpaceNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				o.viewSpacePosition = mul(UNITY_MATRIX_MV, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
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
				return fixed4(col.r, col.g, col.b, apow);
			}
			ENDCG
		}
	}
}
