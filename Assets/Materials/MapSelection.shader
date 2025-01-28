Shader "Unlit/MapSelection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SelectedColor ("Selected Color", Color) = (.0, .0, .0, 1)
        _SelectionTint ("Selection Tint", Color) = (.0, .0, .0, 1)
        _TintForce ("Tint Force", Range(0.0, 1.0)) = 1.0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            fixed4 _SelectedColor;
            fixed4 _SelectionTint;
            fixed _TintForce;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                const fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 out_col = col; 
                
                if (distance(col, _SelectedColor) <= 0.05f)
                {
                    //out_col = clamp(col + _SelectionTint * _TintForce, col, _SelectionTint);
                    fixed time = frac(_Time.y);
                    if (time > 0.5)
                        time = 1. - time;
                    out_col = col + _SelectionTint * _TintForce * time;
                }
                
                return out_col;
            }
            ENDCG
        }
    }
}
