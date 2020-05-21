//Mod by Minh Lượng :)))


Shader "Mobile/VertexLit (Only Directional Lights) - Emission" 
{
    Properties 
    {
        _Color("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        
        [HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionMap ("Emission Map", 2D) = "white" {}
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 80

    Pass 
    {
        Name "FORWARD"
        Tags { "LightMode" = "ForwardBase" }
        CGPROGRAM
        #pragma vertex vert_surf
        #pragma fragment frag_surf
        #pragma target 2.0
        #pragma multi_compile_fwdbase
        #pragma multi_compile_fog
        #include "HLSLSupport.cginc"
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"

        inline float3 LightingLambertVS (float3 normal, float3 lightDir)
        {
            fixed diff = max (0, dot (normal, lightDir));
            return _LightColor0.rgb * diff;
        }

        sampler2D _MainTex;
        fixed4 _Color;
        
        sampler2D _EmissionMap;
        fixed4 _EmissionColor;

        struct Input 
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            c.rgb +=  tex2D(_EmissionMap, IN.uv_MainTex) * _EmissionColor;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        struct v2f_surf 
        {
            float4 pos : SV_POSITION;
            float2 pack0 : TEXCOORD0;
            #ifndef LIGHTMAP_ON
            fixed3 normal : TEXCOORD1;
            #endif
            #ifdef LIGHTMAP_ON
            float2 lmap : TEXCOORD2;
            #endif
            #ifndef LIGHTMAP_ON
            fixed3 vlight : TEXCOORD2;
            #endif
            LIGHTING_COORDS(3,4)
            UNITY_FOG_COORDS(5)
            UNITY_VERTEX_OUTPUT_STEREO
        };
        
        float4 _MainTex_ST;
        v2f_surf vert_surf (appdata_full v)
        {
            v2f_surf o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.pos = UnityObjectToClipPos(v.vertex);
            o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
            #ifdef LIGHTMAP_ON
            o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
            float3 worldN = UnityObjectToWorldNormal(v.normal);
            #ifndef LIGHTMAP_ON
            o.normal = worldN;
            #endif
            #ifndef LIGHTMAP_ON
        
            o.vlight = ShadeSH9 (float4(worldN,1.0));
            o.vlight += LightingLambertVS (worldN, _WorldSpaceLightPos0.xyz);
        
            #endif
            TRANSFER_VERTEX_TO_FRAGMENT(o);
            UNITY_TRANSFER_FOG(o,o.pos);
            return o;
        }
        fixed4 frag_surf (v2f_surf IN) : SV_Target
        {
            Input surfIN;
            surfIN.uv_MainTex = IN.pack0.xy;
            SurfaceOutput o;
            o.Albedo = 0.0;
            o.Emission = 0.0;
            o.Specular = 0.0;
            o.Alpha = 0.0;
            o.Gloss = 0.0;
            #ifndef LIGHTMAP_ON
            o.Normal = IN.normal;
            #else
            o.Normal = 0;
            #endif
            surf (surfIN, o);
            fixed atten = LIGHT_ATTENUATION(IN);
            fixed4 c = 0;
            #ifndef LIGHTMAP_ON
            c.rgb = o.Albedo * IN.vlight * atten;
            #endif
            #ifdef LIGHTMAP_ON
            fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
            #ifdef SHADOWS_SCREEN
            c.rgb += o.Albedo * min(lm, atten*2);
            #else
            c.rgb += o.Albedo * lm;
            #endif
            c.a = o.Alpha;
            #endif
            UNITY_APPLY_FOG(IN.fogCoord, c);
            UNITY_OPAQUE_ALPHA(c.a);
            return c;
        }

        ENDCG
    }
    
    // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
            ENDCG
        }
}

FallBack "Mobile/VertexLit"
}
