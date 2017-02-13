Shader "Custom/Landscape" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_HeightGradient("Height Gradient (RGB)", 2D) = "white" {}
		_HeightPower("Height Power", Range(0.00001, 10)) = 1.0
		_MinHeight("Min Height", Range(-100, 100)) = 0.0
		_MaxHeight("Max Height", Range(-100, 100)) = 25.0
		_RimPower("Rim Power", Range(0.0001, 10)) = 1.0
		_RimStrength("Rim Strength", Range(0, 1)) = 1.0
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GroundColor("Ground Color", Color) = (1,1,1,1)
		_MountainColor("Mountain Color", Color) = (1,1,1,1)
		[Toggle(FAKE_POINT_LIGHTS)] _FakePointLights("Fake Point Lights",  Float) = 0

		_PointLightPos1("Point Light 1 Position", Vector) = (0, 0, 0, 1)
		_PointLightColor1("Point Light Color 1", Color) = (1, 0, 0,1)
		_PointLightAttenuationPower1("Point Light 1 Attenuation Power", Range(0.1, 10)) = 1.0
		_PointLightIntensity1("Point Light 1 Intensity", Range(0, 100)) = 1.0
		
		_PointLightPos2("Point Light 2 Position", Vector) = (0, 0, 0, 1)
		_PointLightColor2("Point Light Color 2", Color) = (1, 0, 0,1)
		_PointLightAttenuationPower2("Point Light 2 Attenuation Power", Range(0.1, 10)) = 1.0
		_PointLightIntensity2("Point Light 2 Intensity", Range(0, 100)) = 1.0

		_PointLightPos3("Point Light 3 Position", Vector) = (0, 0, 0, 1)
		_PointLightColor3("Point Light Color 3", Color) = (1, 0, 0,1)
		_PointLightAttenuationPower3("Point Light 3 Attenuation Power", Range(0.1, 10)) = 1.0
		_PointLightIntensity3("Point Light 3 Intensity", Range(0, 100)) = 1.0

		_PointLightPos4("Point Light 4 Position", Vector) = (0, 0, 0, 1)
		_PointLightColor4("Point Light Color 4", Color) = (1, 0, 0,1)
		_PointLightAttenuationPower4("Point Light 4 Attenuation Power", Range(0.1, 10)) = 1.0
		_PointLightIntensity4("Point Light 4 Intensity", Range(0, 100)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf Standard fullforwardshadows approxview
		#pragma target 3.0
		#pragma shader_feature USE_TEXTURE
		#pragma fragmentoption ARB_precision_hint_fastest

		//sampler2D _MainTex;
		//sampler2D _HeightGradient;

		struct Input {
			//float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		half4 _Color;
		half4 _RimColor;
		half _HeightPower;
		half _MinHeight;
		half _MaxHeight;
		half _RimPower;
		half _RimStrength;
		half4 _GroundColor;
		half4 _MountainColor;

#ifdef FAKE_POINT_LIGHTS
		float4 _PointLightPos1;
		half4 _PointLightColor1;
		float _PointLightAttenuationPower1;
		float _PointLightIntensity1;

		float4 _PointLightPos2;
		half4 _PointLightColor2;
		float _PointLightAttenuationPower2;
		float _PointLightIntensity2;

		float4 _PointLightPos3;
		half4 _PointLightColor3;
		float _PointLightAttenuationPower3;
		float _PointLightIntensity3;

		float4 _PointLightPos4;
		half4 _PointLightColor4;
		float _PointLightAttenuationPower4;
		float _PointLightIntensity4;

		float4 pointLight(float3 vertexPosition, float3 normal, float3 lightPosition, float attenuationPower, float4 color, float intensity) {
			float3 vertexToPointLight = lightPosition - vertexPosition;
			float vertexToPointLightMag = length(vertexToPointLight);
			float pointLightAttenuation = 1 / pow(vertexToPointLightMag, attenuationPower);
			return max(dot(normal, vertexToPointLight / vertexToPointLightMag), 0.0) * color * intensity * pointLightAttenuation;
		}
#endif

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			half hp = pow(saturate((IN.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight)), _HeightPower);
			half rim = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower) * _RimStrength * hp;
			half4 c = lerp(_GroundColor, _MountainColor, hp);

#ifdef FAKE_POINT_LIGHTS
			float4 pointLight1Color = pointLight(IN.worldPos, IN.worldNormal, _PointLightPos1.xyz, _PointLightAttenuationPower1, _PointLightColor1, _PointLightIntensity1);
			float4 pointLight2Color = pointLight(IN.worldPos, IN.worldNormal, _PointLightPos2.xyz, _PointLightAttenuationPower2, _PointLightColor2, _PointLightIntensity2);
			float4 pointLight3Color = pointLight(IN.worldPos, IN.worldNormal, _PointLightPos3.xyz, _PointLightAttenuationPower3, _PointLightColor3, _PointLightIntensity3);
			float4 pointLight4Color = pointLight(IN.worldPos, IN.worldNormal, _PointLightPos4.xyz, _PointLightAttenuationPower4, _PointLightColor4, _PointLightIntensity4);
			o.Albedo = c + rim * _RimColor + pointLight1Color + pointLight2Color + pointLight3Color + pointLight4Color;
#else
			o.Albedo = c + rim * _RimColor;
#endif
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
