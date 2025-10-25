Shader "Custom/GaussianBlur"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * 0.36;
                col += tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * _BlurSize) * 0.16;
                col += tex2D(_MainTex, i.uv - _MainTex_TexelSize.xy * _BlurSize) * 0.16;
                col += tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _BlurSize) * 0.16;
                col += tex2D(_MainTex, i.uv - float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _BlurSize) * 0.16;
                return col;
            }
            ENDCG
        }
    }
}
