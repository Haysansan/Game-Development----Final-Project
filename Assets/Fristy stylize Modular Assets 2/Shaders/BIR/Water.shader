Shader "Fristy/Water"
{
    Properties
    {
        _Color ("water Color", Color) = (1,1,1,1)
        _foamColor ("Foam Color", Color) = (1,1,1,1)
         _foamPower("Foam power", Range(0,1)) = 0.5
         _floamEdge("Foam Edge", Range(0, 10)) = 1

      _uvX("X Scale", float)= 1
      _uvY("Y Scale", float)= 1
       
     
    
        _WaveMoveX("Wave Move X", Range(-10,10)) = 0.5
        _WaveMoveY("Wave Move Y", Range(-10,10)) = 0.5

        _uvX1("X Scale 1", float)= 1
      _uvY1("Y Scale 1", float)= 1
        _WaveMoveX1("Wave Move 1 X", Range(-10,10)) = 0.5
        _WaveMoveY1("Wave Move 1 Y", Range(-10,10)) = 0.5
        _HeightScale("Normal scale", Range(0,20)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
       
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard  fullforwardshadows 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _MainNorm, _GrabTexture;

    

        struct Input
        {
            float2 uv_MainTex: TEXCOORD;
             float3 worldPos;
             float3 ms:POSITION ;
            float3 worldNormal;
             INTERNAL_DATA
        };

        half _Glossiness, _WaveMoveX, _WaveMoveX1, _WaveMoveY, _WaveMoveY1, _uv, _uv1, _HeightScale, _foamPower, _floamEdge;
        float3 wpo;
        half _Metallic; 
        float   _uvY1, _uvX1, _uvX, _uvY;
        fixed4 _Color, _foamColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)



        
inline float unity_noise_randomValue (float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
}

inline float unity_noise_interpolate (float a, float b, float t)
{
    return (1.0-t)*a + (t*b);
}



inline float unity_valueNoise (float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = unity_noise_randomValue(c0);
    float r1 = unity_noise_randomValue(c1);
    float r2 = unity_noise_randomValue(c2);
    float r3 = unity_noise_randomValue(c3);

    float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
    float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
    float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3-0));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3-1));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3-2));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    Out = t;
}





        float3 HeightToNormal(float height, float3 normal, float3 pos)
        {
            float3 worldDirivativeX = ddx(pos);
            float3 worldDirivativeY = ddy(pos);
            float3 crossX = cross(normal, worldDirivativeX);
            float3 crossY = cross(normal, worldDirivativeY);
            float3 d = abs(dot(crossY, worldDirivativeX));
            float3 inToNormal = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
            inToNormal.y *= -1.0;
            return normalize((d * normal) - inToNormal);
        }
 
        float3 WorldToTangentNormalVector(Input IN, float3 normal) {
            float3 t2w0 = WorldNormalVector(IN, float3(1,0,0));
            float3 t2w1 = WorldNormalVector(IN, float3(0,1,0));
            float3 t2w2 = WorldNormalVector(IN, float3(0,0,1));
            float3x3 t2w = float3x3(t2w0, t2w1, t2w2);
            return normalize(mul(t2w, normal));
        }


        
    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
    {
        Out = UV * Tiling + Offset;
    }




        void surf (Input IN, inout SurfaceOutputStandard o)
        {



            _WaveMoveY *=_Time*20;
            _WaveMoveX *=_Time*20;
            _WaveMoveY1 *=_Time*20;
            _WaveMoveX1 *=_Time*20;
            

            //IN.uv_MainTex *= _uvX+_WaveMoveX*_Time.x, _uvY+_WaveMoveY*_Time.y;


            float2 wavemove;
           // float2 tilling = _uv,_uv;

            //wavemove.x = _Time;

          //float2 wav = 

           // float2 wavemove1 = _WaveMoveX1,_WaveMoveY1;
                
                //Unity_TilingAndOffset_float(IN.uv_MainTex ,float2(1, 1),float2(_WaveMoveX+_Time.x,_WaveMoveY+_Time.x), wavemove);

                float noise1, noise2, noise5; 
                Unity_SimpleNoise_float(float2(IN.uv_MainTex.x*_uvX+_WaveMoveX,IN.uv_MainTex.y* _uvY+_WaveMoveY), 10, noise2);
                Unity_SimpleNoise_float(float2(IN.uv_MainTex.x*_uvX1+_WaveMoveX1, IN.uv_MainTex.y*_uvY1+_WaveMoveY1), 20, noise1);
                Unity_SimpleNoise_float(float2(IN.uv_MainTex.x*_uvX1+_WaveMoveX1, IN.uv_MainTex.y*_uvY1+_WaveMoveY1), 100, noise5);
                
                
                IN.worldNormal = WorldNormalVector(IN, float3(0,0,2));

                float noise3 = noise1*noise2;
                float noise4 = noise1+noise2;
                float noisefoam = clamp(noise4-_foamPower, 0,1);
             
                float3 worldNormal = HeightToNormal( noise3*_HeightScale, IN.worldNormal, IN.worldPos);

               
                IN.ms = noise3;
                
               


            // Albedo comes from a texture tinted by color
           fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            //fixed4 c2 = tex2D (_MainTex, IN.uv_MainTex1) * _foamColor;  

            o.Albedo = lerp(lerp(noise3*_Color, _Color, _Color.a), _foamColor*noise5,  noisefoam);
            o.Normal = normalize(WorldToTangentNormalVector(IN, worldNormal));
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
