Shader "Custom/Alpha Diffuse Vertex Colored" {
Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
 
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200
 
        CGPROGRAM
 
        #pragma surface surf Lambert_Tinted alpha
 
        // Surface
 
        sampler2D _MainTex;
 
        struct SurfaceOutputCustom {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            half Specular;
            fixed Gloss;
            fixed Alpha;
            fixed3 VertexColor;
        };
 
        struct Input {
            float2 uv_MainTex;
            float4 color : COLOR;
        };
 
        void surf (Input IN, inout SurfaceOutputCustom o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.VertexColor = IN.color.rgb;
        }
 
        inline fixed4 LightingLambert_Tinted (SurfaceOutputCustom s, fixed3 lightDir, fixed atten) {
            fixed diff = max (0, dot (s.Normal, lightDir));
 
            fixed4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2) + (s.VertexColor * s.Albedo);
            c.a = s.Alpha;
            return c;
        }
 
        inline fixed4 LightingLambert_Tinted_PrePass (SurfaceOutputCustom s, half4 light) {
            fixed4 c;
            c.rgb = (s.Albedo * light.rgb) + (s.VertexColor * s.Albedo);
            c.a = s.Alpha;
            return c;
        }
 
        ENDCG
    }
Fallback "Transparent/VertexLit"
}