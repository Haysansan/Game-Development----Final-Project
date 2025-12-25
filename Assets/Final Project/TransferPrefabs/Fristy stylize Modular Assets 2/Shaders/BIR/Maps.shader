Shader "Fristy/Nature/Maps"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset][Normal]_MainNormal ("Normal", 2D) = "bump" {}
        _uv("uv", float) = 0.5
          _Glossiness ("Smoothness", Range(0,2)) = 0.5
        _MainNormalPow ("Normal Pow", Range(0,3)) = 1
        [Space(20)]
          _Color1 ("Color 1", Color) = (1,1,1,1)
         [NoScaleOffset]_MainTex1 ("Albedo 1 ", 2D) = "white" {}
         [NoScaleOffset][Normal]_MainNormal1 ("Normal 1", 2D) = "bump" {}
        _uv1("uv 2", float) = 1
        _Glossiness1 ("Smoothness 1", Range(0,2)) = 0.5
        _MainNormalPow1 ("Normal Pow 1", Range(0,3)) = 1
        [Space(20)]
          _Color2 ("Color 2 ", Color) = (1,1,1,1)
         [NoScaleOffset]_MainTex2 ("Albedo 2 ", 2D) = "white" {}
         [NoScaleOffset][Normal]_MainNormal2 ("Normal 2", 2D) = "bump" {}
        _uv2("uv 2", float) = 1
            _Glossiness2 ("Smoothness 2", Range(0,2)) = 0.5
        _MainNormalPow2 ("Normal Pow 2", Range(0,3)) = 1
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

        sampler2D _MainTex, _MainTex1, _MainTex2;
        sampler2D _MainNormal, _MainNormal1, _MainNormal2;

        struct Input
        {
            float2 uv_MainTex, uv_MainTex1, uv_MainTex2;
            float4 color: COLOR0;
        };

        half _Glossiness,_Glossiness1, _Glossiness2, _uv, _uv1, _uv2;
        half _Metallic, _MainNormalPow, _MainNormalPow1, _MainNormalPow2;
        fixed4 _Color, _Color1, _Color2;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)




        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);

                ///_Color.rgb = v.color;


        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {


            IN.uv_MainTex *= _uv, _uv;
            IN.uv_MainTex1 *= _uv1, _uv1;
            IN.uv_MainTex2 *= _uv2, _uv2;



            // Albedo comes from a texture tinted by color
            fixed4 c = lerp (tex2D (_MainTex, IN.uv_MainTex) * _Color, _Color, _Color.a);
            fixed3 cn = UnpackNormal(tex2D (_MainNormal, IN.uv_MainTex));
            cn.x *= _MainNormalPow;
            cn.y *= _MainNormalPow;


             fixed4 c1 = lerp (tex2D (_MainTex1, IN.uv_MainTex1) * _Color1, _Color1, _Color1.a);

            fixed3 cn1 = UnpackNormal(tex2D (_MainNormal1, IN.uv_MainTex1));
            cn1.x *= _MainNormalPow1;
            cn1.y *= _MainNormalPow1;


              fixed4 c2 = lerp (tex2D (_MainTex2, IN.uv_MainTex2) * _Color2, _Color2, _Color2.a);


            fixed3 cn3 = UnpackNormal(tex2D (_MainNormal2, IN.uv_MainTex2));

            cn3.x *= _MainNormalPow2;
            cn3.y *= _MainNormalPow2;



            fixed4 cL =  lerp(c1,c,  clamp(IN.color.r, 0,1));
            fixed3 cLn =  lerp(cn1,cn,  clamp(IN.color.r, 0,1));


            fixed4 cL2 =  lerp(cL,c2,  clamp(IN.color.g, 0,1));
            fixed3 cL2n =  lerp(cLn,cn3,  clamp(IN.color.g, 0,1));
        
            fixed cLnG  = lerp(c1*_Glossiness1, c*_Glossiness, clamp(IN.color.r, 0,1));
            fixed clnG2 =  lerp(cLnG, c2*_Glossiness2, clamp(IN.color.g,0,1));




            o.Albedo = cL2.rgb;
            o.Normal = normalize(cL2n);
            // Metallic and smoothness come from slider variables
            
            o.Smoothness = clnG2;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
