Shader "Unlit/ThrustCone"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Color2("Color2", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Noise", 2D) = "white" {}
		_AlphaPower("Alpha Power", Range(0, 10)) = 1.0
		_ThrusterPower("Thruster Power", Range(0, 1)) = 1.0
		_GeoIn("Geo In", Range(-1, 0)) = -0.5
		_GeoOut("Geo Out", Range(0, 1)) = 0.5
		_ThrustSpeed("Thrust Speed", Range(0, 10)) = 4
	}
	SubShader
	{
		Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True"
			"DisableBatching" = "True"   // this makes sure the verts are still in object space and not batched together
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

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			uniform float4 _Color;
			uniform float4 _Color2;
			uniform float _AlphaPower;
			uniform float _ThrusterPower;
			uniform float _GeoIn;
			uniform float _GeoOut;
			uniform float _ThrustSpeed;
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
				
			v2f vert (appdata_base v)
			{
				v2f o;
				float3 p = v.vertex.xyz;
				p.y = p.y * _ThrusterPower;
				float3 dir = normalize(float3(p.x, 0, p.z));
				float exstrusion = lerp(_GeoIn, _GeoOut, _ThrusterPower) * (1.0 - v.texcoord.y);
				p += dir * exstrusion;
				o.pos = mul(UNITY_MATRIX_MVP, float4(p, 1.0));
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 noise = tex2D(_MainTex, i.uv + float2(0, _Time[3] * _ThrustSpeed));
				float4 col = lerp(_Color, _Color2, i.uv.y);
				float power = lerp(_AlphaPower, 1, _ThrusterPower);
				float alpha = (noise + 0.2 ) * pow(i.uv.y, power);
				return fixed4(col.r, col.g, col.b, alpha);
			}

			ENDCG
		}
	}
}
