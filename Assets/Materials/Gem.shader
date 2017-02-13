// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Gem" {
    Properties{
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _LerpPowerMin("Power Min", Float) = 1.0
		_AlphaMultiplier("Alpha Multiplier", Range(0,1)) = 1 
    }

        SubShader{
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

        struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f {
        float4 vertex : SV_POSITION;
        half2 texcoord : TEXCOORD0;
        UNITY_FOG_COORDS(1)
    };

    fixed4 _Color;
    fixed _UpperThreshold;
    fixed _LowerThreshold;
    half _LerpPowerMin;
	half _AlphaMultiplier;

    sampler2D _MainTex;
    float4 _MainTex_ST;

    v2f vert(appdata_t v)
    {
        v2f o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        half2 uv = i.texcoord;
        fixed4 col = tex2D(_MainTex, uv);
        fixed4 modulatedColor = lerp(_Color, col, _LerpPowerMin);
        modulatedColor.a = col.a * _AlphaMultiplier;
        UNITY_APPLY_FOG(i.fogCoord, col);
        return modulatedColor;
    }
        ENDCG
    }
    }

}
