Shader "Fristy/Nature/Rocks"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
        [NoScaleOffset]_MainTex ("Albedo (RBG) and Occlusion (A)", 2D) = "white" {}
        _brightness("Brightness",Range(0,2))=1
         [Space(20)]
        
         [NoScaleOffset][Normal]_MainNorm ("Normal ", 2D) = "bump" {}
         _MainNormPow("Normal power", Range(-2,2)) = 1
         _uv("uv", float) = 1
        [Space(50)]
         _dexTex("Detail Albedo", 2D) = "white"{}
         _dexTexPow("Blend Albedo", Range(0,2)) = 0.5
         _uv2("uv", float)=1
         [Space(20)]

         [NoScaleOffset][Normal]_DexNorm ("Detail Normal ", 2D) = "bump" {}
         _uv3("uv", float)=1
         _DexNormPow("Normal power", Range(-2,2)) = 1
         [Space(20)]
         
        _occlusionPow ("Occlusion Power", Range(0,1)) = 0.5
        _Glossiness ("Smoothness highs", Range(0,1)) = 0.5
        _Glossiness1 ("smoothness lows", Range(0,1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _MainNorm, _dexTex, _DexNorm;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness, _Glossiness1, _MainNormPow, _uv, _occlusionPow,_brightness, _DexNormPow, _uv2, _uv3, _dexTexPow;
      
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


            void Unity_Blend_HardLight_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
    {
        float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
        float4 result2 = 2.0 * Base * Blend;
        float4 zeroOrOne = step(Blend, 0.5);
        Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
        Out = lerp(Base, Out, Opacity);
    }


    void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, float Opacity, out float4 Out)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}


        void surf (Input IN, inout SurfaceOutputStandard o)
        {


            IN.uv_MainTex *= _uv, _uv;
            // Albedo comes from a texture tinted by color
            fixed4 c =tex2D (_MainTex, IN.uv_MainTex);
            fixed4 b = tex2D(_dexTex, IN.uv_MainTex*_uv2);
            fixed4 cl,cl2;
            
            Unity_Blend_HardLight_float4(c,c,cl,_brightness);
            fixed3 cnorm = UnpackNormal(tex2D(_MainNorm, IN.uv_MainTex));
            cnorm.x *= _MainNormPow;
            cnorm.y *= _MainNormPow;
            Unity_Blend_Overlay_float4(cl, b,_dexTexPow,cl2);

             fixed3 cnorm2 = UnpackNormal(tex2D(_DexNorm, IN.uv_MainTex*_uv3));
            cnorm2.x *= _DexNormPow;
            cnorm2.y *= _DexNormPow;
            fixed sm = c;

            o.Albedo =  lerp(_Color, cl2*_Color,_Color.a);
            o.Normal = normalize(float3(cnorm.rg+cnorm2.rg,cnorm.b*cnorm2.b));
            o.Occlusion = lerp(1,c.a, _occlusionPow );
            // Metallic and smoothness come from slider variables
           
            o.Smoothness = lerp(_Glossiness, _Glossiness1, sm);
            o.Alpha = c.a;
        } 
        ENDCG
    }
    FallBack "Diffuse"
}
