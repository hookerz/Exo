// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Hologram"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Noise", 2D) = "white" {}
		_Pow("Pow", Range(0.0, 10.0)) = 1.0
		_ScanFreq("Scan line Frequency", Float) = 1000.0
		_ScanFreq2("Scan line Frequency 2", Float) = 500.0
		_Speed("Speed", Float) = 2.0
		_Speed2("Speed2", Float) = 3.0
		_SpeedMod("SpeedModulate", Float) = 1.0
		[Toggle(CONE_CUTOFF)] _ConeCutoff("Cone Cutoff", Float) = 1.0
		_InConeFactor("In Cone Factor", Range(0, 1)) = 1.0
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
		ZTest Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature CONE_CUTOFF
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewSpaceNormal : TEXCOORD1;
				float3 viewSpacePosition : TEXCOORD2;
				float3 worldSpacePosition : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			uniform float _Pow;
			uniform float4 _Color;
			uniform float _ScanFreq;
			uniform float _ScanFreq2;
			uniform float _Speed;
			uniform float _Speed2;
			uniform float _SpeedMod;
			uniform float4 _ConeOrigin;
			uniform float4 _ConeEnd;
			uniform float _ConeHalfAngle;
			uniform float _ConeCosHalfAngle;
			uniform float _ConeLength;
			uniform float _InConeFactor;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0));
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.viewSpaceNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
				o.viewSpacePosition = mul(UNITY_MATRIX_MV, v.vertex);
				o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}

			float InCone(float3 worldSpacePosition)
			{

				float3 v1 = worldSpacePosition - _ConeOrigin.xyz;
				float distV1 = length(v1);
				float3 v2 = _ConeEnd.xyz - _ConeOrigin.xyz;
				float cosAngle = dot(v1, v2) / (distV1 * length(v2));
				float res = 1 - saturate((1 - cosAngle) / (1 - _ConeCosHalfAngle));
				if (distV1 > _ConeLength)
				{
					res = 0.0f;
				}
				return res;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 noise = tex2D(_MainTex, i.uv + _Time[0]);
#ifdef CONE_CUTOFF
				float inCone = InCone(i.worldSpacePosition);
#else
				float inCone = 1.0f;
#endif
				inCone = lerp(1.0f, inCone, _InConeFactor);
				//return fixed4(inCone, inCone, inCone, 1);

				float3 normal = normalize(i.viewSpaceNormal);
				float3 viewDir = normalize(i.viewSpacePosition);
				float d = 1 - saturate(dot(-viewDir, normal));
				float dpow = pow(d, _Pow);
				float2 screenPos = i.screenPos.xy / i.screenPos.w;
				float mod = sin(_Time[0] * _SpeedMod);
				float a = sin(screenPos.y * _ScanFreq + _Time[0] * _Speed * mod);
				float a2 = cos(sin(screenPos.y * _ScanFreq2 + _Time[0] * _Speed2));
				float alpha = dpow * a * a2 * inCone;

				_Color = _Color * noise;

				return fixed4(_Color.r, _Color.g, _Color.b, alpha);
			}

			ENDCG
		}
	}
}
