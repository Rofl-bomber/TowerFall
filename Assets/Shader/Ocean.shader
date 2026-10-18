Shader "Custom/Ocean"
{
    Properties
    {
        _BaseMap ("Ocean Texture", 2D) = "white" {}
        _NormalMap ("Wave Normal", 2D) = "bump" {}

        _Color ("Water Tint", Color) = (0.0,0.35,0.5,1)

        _NormalStrength ("Normal Strength", Range(0,2)) = 0.8

        _WaveSpeed1 ("Wave Speed 1", Float) = 0.03
        _WaveSpeed2 ("Wave Speed 2", Float) = -0.015

        _WaveScale1 ("Wave Scale 1", Float) = 2
        _WaveScale2 ("Wave Scale 2", Float) = 8

        _Smoothness ("Smoothness", Range(0,1)) = 0.7

        _FresnelPower ("Fresnel Power", Range(0.1,10)) = 3

        _ReflectionStrength ("Reflection Strength", Range(0,1)) = 0.35

        _ReflectionTint ("Reflection Tint", Color) = (0.5,0.7,0.8,1)
    }


    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }


        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };


            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };


            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);


            CBUFFER_START(UnityPerMaterial)

            float4 _Color;
            float4 _ReflectionTint;

            float _NormalStrength;

            float _WaveSpeed1;
            float _WaveSpeed2;

            float _WaveScale1;
            float _WaveScale2;

            float _Smoothness;

            float _FresnelPower;
            float _ReflectionStrength;

            CBUFFER_END



            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 world =
                    TransformObjectToWorld(IN.positionOS.xyz);

                OUT.positionCS =
                    TransformWorldToHClip(world);

                OUT.worldPos = world;
                OUT.uv = IN.uv;

                OUT.viewDir =
                    normalize(
                        _WorldSpaceCameraPos - world
                    );

                return OUT;
            }



            float3 GetNormal(float2 uv)
            {
                float t = _Time.y;


                float2 uv1 =
                    uv * _WaveScale1 +
                    float2(
                        t * _WaveSpeed1,
                        t * _WaveSpeed1 * 0.5
                    );


                float2 uv2 =
                    uv * _WaveScale2 +
                    float2(
                        t * _WaveSpeed2,
                        -t * _WaveSpeed2
                    );


                float3 n1 =
                    UnpackNormal(
                        SAMPLE_TEXTURE2D(
                            _NormalMap,
                            sampler_NormalMap,
                            uv1
                        )
                    );


                float3 n2 =
                    UnpackNormal(
                        SAMPLE_TEXTURE2D(
                            _NormalMap,
                            sampler_NormalMap,
                            uv2
                        )
                    );


                float3 n =
                    normalize(n1+n2);


                n.xy *= _NormalStrength;

                return normalize(n);
            }



            half4 frag(Varyings IN) : SV_Target
            {

                float3 normal =
                    GetNormal(IN.uv);



                float3 albedo =
                    SAMPLE_TEXTURE2D(
                        _BaseMap,
                        sampler_BaseMap,
                        IN.uv
                    ).rgb;


                albedo *= _Color.rgb;



                float fresnel =
                    pow(
                        1 -
                        saturate(
                            dot(
                                normal,
                                IN.viewDir
                            )
                        ),
                        _FresnelPower
                    );



                float3 reflection =
                    SAMPLE_TEXTURECUBE(
                        unity_SpecCube0,
                        samplerunity_SpecCube0,
                        reflect(
                            -IN.viewDir,
                            normal
                        )
                    ).rgb;


                reflection *= _ReflectionTint.rgb;



                float reflectionAmount =
                    fresnel *
                    _ReflectionStrength;



                float3 color =
                    lerp(
                        albedo,
                        reflection,
                        reflectionAmount
                    );


                return float4(color,1);
            }

            ENDHLSL
        }
    }
}