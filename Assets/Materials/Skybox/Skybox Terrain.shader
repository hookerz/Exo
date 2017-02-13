Shader "Custom/Skybox Terrain"
{
    Properties
    {
        _StartColor("Start Color", Color) = (0, 0, 1, 1)
        _EndColor("End Color", Color) = (0, 0, 0.25, 1)
        _Power("Power", Float) = 0.01
        _Max("Max",  Range(0, 1)) = 0.5
        _NoisyIntensity("Noise Intensity",  Float) = 1
        _NightGradient("Night Gradient", 2D) = "white" {}
        [Toggle(USE_TEXTURE)] _UseTexture("Use Texture",  Float) = 1
        [Toggle(USE_DITHER)] _UseDither("Use Dither",  Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Background" }
        Blend SrcAlpha One
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Fog{ Mode Off }

            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag
            #pragma target 3.0
            #pragma shader_feature USE_TEXTURE
            #pragma shader_feature USE_DITHER
            #pragma fragmentoption ARB_precision_hint_fastest


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 posws  : TEXCOORD0;
            };

            half4 _StartColor;
            half4 _EndColor;
            half _Power;
            half _Max;
            half _NoisyIntensity;
#ifdef USE_TEXTURE
            sampler2D _NightGradient;
#endif

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posws = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
#ifdef USE_DITHER
                // Adding noise to get rid of banding
                // Reference: http://www.137b.org/?p=3032
                const float noiseScale = 240;
                float2 wcoord = (i.posws.xy / i.posws.w) * noiseScale;
                half4 dither = dot(float2(171.0, 231.0), wcoord.xy);
                dither.rgb = frac(dither / float3(103.0f, 71.0f, 97.0f)) - float3(0.5f, 0.5f, 0.5f);
                dither = (dither / 255.0) * _NoisyIntensity;
#else
                half4 dither = half4(0, 0, 0, 0);
#endif

                half3 nposws = normalize(i.posws.xyz);
                half upness = saturate(dot(nposws, half3(0, 1, 0)));
                half d = saturate(upness / _Max);
                half v = saturate(pow(d, _Power));

#ifdef USE_TEXTURE
                half4 gradient = tex2D(_NightGradient, float2(0, v));
#else
                half4 gradient = pow(lerp(pow(_StartColor, 0.4545454), pow(_EndColor, 0.4545454), v), 2.2);
#endif
                gradient = gradient + dither;
                gradient.a = 1;
                return gradient;
            }
            ENDCG
        }
    }
}
