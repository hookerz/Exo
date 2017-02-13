Shader "Custom/StructureGlassSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Cube("Cubemap", CUBE) = "" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_NormalMapAmount("Normal Map Amount", Range(0, 1)) = 1.0
		_Opacity("Opacity", Range(0, 1)) = 0
		_IOR("IOR", Range(0, 10)) = 1
		_IORMax("IOR Max", Range(0, 10)) = 4.0
		//_ReflRefr("ReflRefr", Range(0, 1)) = 0
		_XPow("XPow", Range(0, 10)) = 2
		_YPow("YPow", Range(0, 10)) = 2
		_RefrOrColor("_RefrOrColor", Range(0, 1)) = 0
		_BlendDist("_BlendDist", Range(0, 1)) = 0.2
		_GlobalAlpha("_GlobalAlpha", Range(0, 1)) = 1.0
	}
	SubShader {

		Tags{
			"Queue" = "Transparent+1"
			"RenderType" = "Transparent"
			//"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Fog{ Mode Off }
		ZWrite Off

		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend
		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		samplerCUBE _Cube;

		struct Input {
			float2 uv_MainTex;
			float2 uv2_BumpMap;
			float3 worldNormal;
			float3 worldRefl; 
			float3 viewDir;
			INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Opacity;
		float _IOR;
		float _IORMax;
		float _NormalMapAmount;
		float _XPow;
		float _YPow;
		float _RefrOrColor;
		float _BlendDist;
		float _GlobalAlpha;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Normal = lerp(half3(0.0, 0.0, 1.0), UnpackNormal(tex2D(_BumpMap, IN.uv2_BumpMap)), _NormalMapAmount);
			
			half3 viewTan = -normalize(IN.viewDir);
			half3 viewWorld = WorldNormalVector(IN, viewTan);
			half3 normalWorld = WorldNormalVector(IN, o.Normal);
			
			// if we want to control the curve a bit more:
			//float x = pow(abs(IN.uv_MainTex.x), _XPow);
			//float y = pow(abs(IN.uv_MainTex.y), _YPow);
			//float d = sqrt(x * x + y * y);
			float d = length(IN.uv_MainTex.xy) * 0.7071067811865475;
			o.Alpha = min(saturate(1 - (d - (_Opacity*(1.0 + _BlendDist) - _BlendDist)) / _BlendDist), _GlobalAlpha);
			
			float ior = lerp(_IORMax, _IOR, o.Alpha);
			half3 worldRefract = refract(viewWorld, normalWorld, ior);
			//half3 worldReflect = WorldReflectionVector(IN, o.Normal);
			half3 refr = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefract);
			//half3 refl = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldReflect);
			//o.Alpha = o.Alpha > 0.0 ? 1.0 : 0.0;

			// circular fade from center
			//o.Albedo = lerp(refr, refl, _ReflRefr) * _Color;
			o.Albedo = lerp(refr, _Color, _RefrOrColor);

			// fade from corners inward rectangular
			//fixed secondPart = 1.0 - saturate(_Opacity - 0.5) * 2.0f;
			//o.Alpha = saturate(1-(length(IN.uv_MainTex.xy) * 0.7071067811865475) * _Opacity * 2.0f + secondPart);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
