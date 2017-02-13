// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Texture Transparent II" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Intensity("Intensity", Range(0.0, 3.0)) = 1.0
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            ZWrite Off
            Blend SrcAlpha One

            Pass {
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
                    };

                    fixed4 _Color;
                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    float _Intensity;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                        o.texcoord = 
                            TRANSFORM_TEX(
                                v.texcoord, 
                                _MainTex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 tex = tex2D(_MainTex, i.texcoord);
                        fixed4 col = _Color * _Intensity;
                        col.a = tex.a;

                        return col;
                    }
                ENDCG
            }
        }

}
