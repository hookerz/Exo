Shader "Custom/PlatformUV" {
	Properties {
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		
		_Color1("Color 1", Color) = (1,1,1,1)
		_Emission1("Emission 1", Color) = (0,0,0,0)
		_Glossiness1("Glossiness 1", Range(0,1)) = 0
		_Metallic1("Metallic 1", Range(0,1)) = 0
		
		_Color2("Color 2", Color) = (1,1,1,1)
		_Emission2("Emission 2", Color) = (0,0,0,0)
		_Glossiness2("Glossiness 2", Range(0,1)) = 0
		_Metallic2("Metallic 2", Range(0,1)) = 0
				
		_Color3("Color 3", Color) = (1,1,1,1)
		_Emission3("Emission 3", Color) = (0,0,0,0)
		_Glossiness3("Glossiness 3", Range(0,1)) = 0
		_Metallic3("Metallic 3", Range(0,1)) = 0
				
		_Color4("Color 4", Color) = (1,1,1,1)
		_Emission4("Emission 4", Color) = (0,0,0,0)
		_Glossiness4("Glossiness 4", Range(0,1)) = 0
		_Metallic4("Metallic 4", Range(0,1)) = 0
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};


		fixed4 _Color1;
		fixed4 _Color2;
		fixed4 _Color3;
		fixed4 _Color4;

		float _Glossiness1;
		float _Glossiness2;
		float _Glossiness3;
		float _Glossiness4;

		float _Metallic1;
		float _Metallic2;
		float _Metallic3;
		float _Metallic4;
		
		fixed4 _Emission1;
		fixed4 _Emission2;
		fixed4 _Emission3;
		fixed4 _Emission4;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uvIndex = floor(IN.uv_MainTex.xy * 2);
			float index = uvIndex.x + uvIndex.y * 2;
			
			float4 matIndex = float4(index == 0, index == 1, index == 2, index == 3);
			float4 metallic = float4(_Metallic1, _Metallic2, _Metallic3, _Metallic4);
			float4 smoothness = float4(_Glossiness1, _Glossiness2, _Glossiness3, _Glossiness4);

			o.Albedo = (matIndex.x * _Color1 + matIndex.y * _Color2 + matIndex.z * _Color3 + matIndex.w * _Color4).rgb;
			o.Metallic = dot(matIndex, metallic);
			o.Smoothness = dot(matIndex, smoothness);
			o.Emission = (matIndex.x * _Emission1 + matIndex.y * _Emission2 + matIndex.z * _Emission3 + matIndex.w * _Emission4).rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
