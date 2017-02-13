Shader "Custom/PauseShell" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ColorOrRefr("Color Or Refract", Range(0, 1)) = 0
		_CubeLOD("Cube LOD", Range(0, 15)) = 0
		_IOR("Index of Refraction", Range(0, 10)) = 1
		_Alpha("Alpha", Range(0, 1)) = 0
	}
	SubShader {
		Tags{
			//"RenderType" = "Opaque"
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Fog { Mode Off }
		Cull Off
		ZWrite Off
		ZTest Off
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd alpha:blend
		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest

		sampler2D _MainTex;

		fixed4 _Color;

		half _IOR;
		half _ColorOrRefr;
		half _CubeLOD;
		half _Alpha;


		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			float3 worldRefl;
			float3 viewDir;
			INTERNAL_DATA
		};

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo, s.Alpha);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			half3 viewTan = -normalize(IN.viewDir);
			half3 viewWorld = WorldNormalVector(IN, viewTan);
			half3 normalWorld = WorldNormalVector(IN, o.Normal);

			half3 worldRefract = refract(viewWorld, -normalWorld, _IOR);
			half3 refr = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefract, _CubeLOD);

			o.Albedo = refr;
			o.Alpha = _Alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
