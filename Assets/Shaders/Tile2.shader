Shader "Custom/Tile2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _GridStep ("Grid size", Float) = 6.4
        _GridWidth ("Grid width", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
 
        sampler2D _MainTex;
 
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };
 
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _GridStep;
        float _GridWidth;
       
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
           
            // grid overlay
            float2 pos = IN.worldPos.xz / _GridStep;
            float2 f  = abs(frac(pos)-.5);
            float2 df = fwidth(pos) * _GridWidth;
            float2 g = smoothstep(-df ,df , f);
            float grid = 1.0 - saturate(g.x * g.y);
            c.rgb = lerp(c.rgb, float3(1,1,1), grid);
           
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
 