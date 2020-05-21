//Mod by Minh Lượng :)))

//No Support LightMap

Shader "Mobile/Diffuse Emission" {
Properties {
    _Color("Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    
    [HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
    _EmissionMap ("Emission Map", 2D) = "white" {}
}
SubShader {

    Tags { "RenderType"="Opaque" }
    LOD 150
    
    CGPROGRAM
        #pragma surface surf Lambert noforwardadd
        sampler2D _MainTex;
        fixed4 _Color;
        
        sampler2D _EmissionMap;
        fixed4 _EmissionColor;
        
        struct Input {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            
            fixed4 c2 = tex2D(_EmissionMap, IN.uv_MainTex) * _EmissionColor;
            o.Emission = c2;
        }
        
        ENDCG
}

Fallback "Mobile/VertexLit"
}
