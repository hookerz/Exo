Shader "Custom/DroneUV" {
	Properties {
		_Color ("White Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("White Smoothness", Range(0,1)) = 0.5
		_Metallic("White Metallic", Range(0,1)) = 0.0
		_BlackColor("Black Color", Color) = (0,0,0,1)
		_BlackGlossiness("Black Smoothness", Range(0,1)) = 0.5
		_BlackMetallic("Black Metallic", Range(0,1)) = 0.0
		_BlueColor("Blue Color", Color) = (0,0,0,1)
		_BlueEmission("Blue Emission", Color) = (0,0,0,1)
		_BlueGlossiness("Blue Smoothness", Range(0,1)) = 0.5
		_BlueMetallic("Blue Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		half _Glossiness;
		half _Metallic;

		fixed4 _BlackColor;
		half _BlackGlossiness;
		half _BlackMetallic;

		fixed4 _BlueColor;
		half _BlueGlossiness;
		half _BlueMetallic;
		half3 _BlueEmission;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half2 uvIndex = floor(IN.uv_MainTex.xy * 2);
			half index = uvIndex.x + uvIndex.y * 2;
			half white = (index == 2);
			half black = (index == 3);
			half blue = (index == 0);
			fixed4 c = white * _Color + black * _BlackColor + blue * _BlueColor;
			o.Albedo = c.rgb;
			o.Metallic = white * _Metallic + black * _BlackMetallic + blue * _BlueMetallic;
			o.Smoothness = white * _Glossiness + black * _BlackGlossiness + blue * _BlueGlossiness;
			o.Alpha = 1.0;
			o.Emission = blue * _BlueEmission;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
