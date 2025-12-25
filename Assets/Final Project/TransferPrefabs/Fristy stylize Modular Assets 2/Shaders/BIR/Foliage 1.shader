// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Fristy/Nature/Leaves 2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
         _InternalColor("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _Cutout("Cutout", Range(0,20)) = 0.5
        _sat("Saturate", Range(0, 4)) = 1
         _sss("SSS Amount", Range(0,4)) = 0.5
           _MainNormal ("Normal", 2D) = "bump" {}
         _MainNormalInt("Normal Intensity", Range(-4, 4)) = 1
         _Maps("Maps", 2D) = "white"{}
      
          
        _windSpeed("WindSpeed", Range(0, 10)) = 1
        _WindDensity("Wind Density", Range(0, 10)) = 1
        _windStrengh("Wind Strength", Range(0, 1)) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _OcclusionPow("Occlusion power", Range(-1,0)) = 1
        _Metallic ("Metallic", Range(0,1)) = 0.0
           Vector1_8838B166("offset", Float) = 0
    }
    SubShader
    {
      Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
      Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        cull off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
         #pragma surface surf Standard vertex:vert fullforwardshadows addshadow nolightmap


        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0


       
        sampler2D _MainTex, _MainNormal, _Maps;

        struct Input
        {
            
            float2 uv_MainTex;
            float3 viewDir;
            float3 lightDir;
            float3 pos: POSITION1; 
            float3 color: COLOR0;
            float3 worldPos: TEXCOORD1;
          
            //float3 WorldSpacePosition: TEXCOORD0;
            //float4 position: POSITION;
           UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        half _Glossiness, _windSpeed, _WindDensity, _windStrengh, Vector1_8838B166, Vector1_41E7E0B7,_MainNormalInt, _sss, _Cutout, _sat;
        half _Metallic, _OcclusionPow;
        fixed4 _Color, _InternalColor, Vector1_4BAB8211;




void Unity_Saturation_float(float3 In, float Saturation, out float3 Out)
{
    float luma = dot(In, float3(0.2126729, 0.7151522, 0.0721750));
    Out =  luma.xxx + Saturation.xxx * (In - luma.xxx);
}



float2 unity_gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float unity_gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
{
    Out = unity_gradientNoise(UV * Scale) + 0.5;
}



    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
    {
        Out = UV * Tiling + Offset;
    }


















		void vert (inout appdata_full v, out Input o) {
        	UNITY_INITIALIZE_OUTPUT(Input,o);
                 o.pos = v.vertex.xyz;
             float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
            float3 _color_ = o.color;
           
           // o.WorldSpacePosition =  mul(unity_ObjectToWorld, vertex).xyz;
            float2 UV;
            float2 time = _Time*_windSpeed;
            Unity_TilingAndOffset_float(worldPos.xz,float2 (1,1), time, UV);


            float noiseOut;
            Unity_GradientNoise_float(UV,_WindDensity, noiseOut);
            float noiseMun = noiseOut-0.2;
            float final = noiseMun*_windStrengh;
         
          
           

            v.vertex.x += lerp(0,final,  v.color.r);
      

        

        
     
    	  }







        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)



        
	float3 SubsurfaceShadingSimple(float3 diffColor, float3 normal, float3 viewDir, float3 thickness, float3 lightDir, float3 lightColor)
        {
            half3 vLTLight = lightDir + normal * 1;
            half  fLTDot = pow(saturate(dot(viewDir, -vLTLight)), 3.5) * 1.5;
            half3 fLT = 1 * (fLTDot + 1.2) * (thickness);
            return diffColor * ((lightColor * fLT) * 0.4);
        }



        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color


//            float as; Unity_SimpleNoise_float(IN.uv_MainTex, 5, as);

                float dist = distance(IN.worldPos, _WorldSpaceCameraPos);

   fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            float3 csat;
            Unity_Saturation_float(c, _sat, csat);
            fixed3 cnorm = UnpackNormal(tex2D(_MainNormal, IN.uv_MainTex));
            cnorm.x *=  _MainNormalInt;
            cnorm.y *=  _MainNormalInt;
            fixed4 c2 = tex2D(_Maps, IN.uv_MainTex);




           
            o.Albedo = lerp(csat*_Color, _Color, _Color.a);
              //o.Albedo = IN.color;
            o.Normal = normalize(cnorm);
           
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission += SubsurfaceShadingSimple(_InternalColor* c, o.Normal, IN.viewDir, c2.r*_sss*IN.color.r, IN.lightDir, _LightColor0)*c.rgb;
            float cip = saturate(dist/_Cutout);
            o.Alpha = c.a;
            clip(c.a-cip);
        }
        ENDCG
    }
   FallBack "TransparentCutout"
}
