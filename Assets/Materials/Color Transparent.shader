// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Color Transparent" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma multi_compile_fog
                    #pragma fragmentoption ARB_precision_hint_fastest

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                    };

                    fixed4 _Color;

                    sampler2D _MainTex;
                    float4 _MainTex_ST;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        return _Color;
                    }
                ENDCG
            }
        }

}
