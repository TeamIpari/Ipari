Shader "Whale_Shader Outline"
{
    Properties
    {
        _emissive("emissive", Float) = 1
        _dots_fresnel("dots fresnel", Float) = 3.4
        _fresnel("fresnel", Float) = 0.9
        _fresnel_color("fresnel color", Color) = (0.8396226, 0.8396226, 0.8396226, 0)
        
         _OutlineWidth ("Width", Float ) = 0.5
        [HDR] _OutlineColor ("Color", Color) = (0.0,0.0,0.0,1.0)
        [Enum(Normal,0,Origin,1)] _OutlineExtrudeMethod("Outline Extrude Method", int) = 0
        _OutlineOffset ("Outline Offset", Vector) = (0,0,0)
        _OutlineZPostionInCamera ("Outline Z Position In Camera", Float) = 0.0
        [ToggleOff] _AlphaBaseCutout ("Alpha Base Cutout", Float ) = 1.0

        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1("Texture2D", 2D) = "white" {}
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1("Texture2D", 2D) = "white" {}
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Lit"
            "Queue"="Geometry"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }

        Pass
        {

            Name"Outline"
            
            Tags
            {
                "LightMode"="SRPDefaultUnlit"
            }


            Cull Front

            HLSLPROGRAM
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
                float2 uvLM : TEXCOORD1;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionWSAndFogFactor : TEXCOORD2;
                float4 projPos : TEXCOORD7;
                float4 posWorld : TEXCOORD8;
                float4 vertexColor : COLOR;
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                uniform half _OutlineWidth;
                uniform int _OutlineExtrudeMethod;
                uniform half3 _OutlineOffset;
                uniform half _OutlineZPostionInCamera;
                uniform half _Cutout;
                uniform half _AlphaBaseCutout;
                uniform half4 _OutlineColor;
            CBUFFER_END


            Varyings LitPassVertex(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = input.uv;
                output.vertexColor = input.vertexColor;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                float2 _1283_skew = output.uv + 0.2127 + output.uv.x * 0.3713 * output.uv.y;
                float2 _1283_rnd = 4.789 * sin(489.123 * _1283_skew);
                half _1283 = frac(_1283_rnd.x * _1283_rnd.y * (1 + _1283_skew.x));

                float3 _OEM;

                // Normal과 Origin 중 선택
                if (!_OutlineExtrudeMethod)
                    _OEM = input.normalOS;
                else
                    _OEM = normalize(input.positionOS.xyz);

                half RTD_OL = _OutlineWidth * 0.01;

                output.positionCS = mul(GetWorldToHClipMatrix(),
                                        mul(GetObjectToWorldMatrix(),
   float4(
       input.positionOS.xyz + _OutlineOffset.xyz *
       0.01 + _OEM * RTD_OL,
       1.0)));

                output.positionCS.z -= _OutlineZPostionInCamera * 0.0005;

                output.posWorld = float4(vertexInput.positionWS, 1.0);
                output.projPos = ComputeScreenPos(output.positionCS);
                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);

                return output;
            }

            void LitPassFragment(Varyings input, out half4 outColor : SV_Target0)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half3 color = 1.0;
                
                clip(1 - 0.5);


                float fogFactor = input.positionWSAndFogFactor.w;

                #ifdef UNITY_COLORSPACE_GAMMA
				_OutlineColor = float4(LinearToGamma22(_OutlineColor.rgb), _OutlineColor.a);
                #endif

                half3 finalRGBA = _OutlineColor.rgb;
                color = MixFog(finalRGBA, fogFactor);

                outColor = half4(color, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_SHADOW_COORD
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            #define _FOG_FRAGMENT 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float4 tangentWS;
                float4 texCoord0;
                float3 viewDirectionWS;
                #if defined(LIGHTMAP_ON)
                     float2 staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                     float2 dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh;
                #endif
                float4 fogFactorAndVertexLight;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                     float4 shadowCoord;
                #endif
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 TangentSpaceNormal;
                float3 WorldSpaceViewDirection;
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if defined(LIGHTMAP_ON)
                     float2 staticLightmapUV : INTERP0;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                     float2 dynamicLightmapUV : INTERP1;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh : INTERP2;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                     float4 shadowCoord : INTERP3;
                #endif
                float4 tangentWS : INTERP4;
                float4 texCoord0 : INTERP5;
                float4 fogFactorAndVertexLight : INTERP6;
                float3 positionWS : INTERP7;
                float3 normalWS : INTERP8;
                float3 viewDirectionWS : INTERP9;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                    output.staticLightmapUV = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = input.shadowCoord;
                #endif
                output.tangentWS.xyzw = input.tangentWS;
                output.texCoord0.xyzw = input.texCoord0;
                output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
                output.positionWS.xyz = input.positionWS;
                output.normalWS.xyz = input.normalWS;
                output.viewDirectionWS.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                    output.staticLightmapUV = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = input.shadowCoord;
                #endif
                output.tangentWS = input.tangentWS.xyzw;
                output.texCoord0 = input.texCoord0.xyzw;
                output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
                output.positionWS = input.positionWS.xyz;
                output.normalWS = input.normalWS.xyz;
                output.viewDirectionWS = input.viewDirectionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Saturate_float4(float4 In, out float4 Out)
            {
                Out = saturate(In);
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float3 NormalTS;
                float3 Emission;
                float Metallic;
                float Smoothness;
                float Occlusion;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_R_4 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.r;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_G_5 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.g;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_B_6 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.b;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_A_7 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.a;
                float _Float_555cf4c4456443a68e341fb84c46bc2f_Out_0 = 1;
                float4 _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0,
                                         (_Float_555cf4c4456443a68e341fb84c46bc2f_Out_0.xxxx),
                                         _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2);
                float4 _Property_8425e5cd9b9343f29331b07245faccdd_Out_0 = _fresnel_color;
                float _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 3.01,
                    _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3);
                float4 _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2;
                Unity_Multiply_float4_float4(_Property_8425e5cd9b9343f29331b07245faccdd_Out_0,
                    (_FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3.xxxx),
                    _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2);
                float _Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0 = _fresnel;
                float4 _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
                             (_Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0.
                                 xxxx),
                             _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2);
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.r;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_G_5 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.g;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_B_6 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.b;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_A_7 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.a;
                float4 _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2,
                                                                    (_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4
                                                                        .xxxx),
                                                                    _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2);
                float _Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0 = 0.72;
                float2 _Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0 = float2(-0.025, -0.05);
                float2 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2;
                Unity_Multiply_float2_float2(_Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0, (IN.TimeParameters.x.xx),
                                                            _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2);
                float2 _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(3.5, 3.5),
          _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2,
          _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3);
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    GetTransformedUV(_TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3));
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.r;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_G_5 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.g;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_B_6 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.b;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_A_7 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.a;
                float _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2;
                Unity_Multiply_float_float(_Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0,
                                                             _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4,
                                                             _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2);
                float _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0 = 22.81;
                float _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2;
                Unity_Multiply_float_float(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4,
                                        _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0,
                                        _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2);
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.r;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_G_5 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.g;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_B_6 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.b;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_A_7 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.a;
                float _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2;
                Unity_Multiply_float_float(_Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2,
           _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4,
           _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2);
                float _Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0 = _dots_fresnel;
                float4 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
                                                   (_Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0.xxxx),
                                                   _Multiply_2e84e13913da43cf9201736bb595735e_Out_2);
                float4 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2;
                Unity_Multiply_float4_float4((_Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2.xxxx),
                                                                              _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                                                              _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2);
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.r;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_G_5 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.g;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_B_6 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.b;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_A_7 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.a;
                float _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1;
                Unity_OneMinus_float(_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4,
               _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1);
                float4 _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2;
                Unity_Multiply_float4_float4(_Multiply_f5a58908183541c6bba1b10da96ab027_Out_2,
                    (_OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1.xxxx),
                    _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2);
                float4 _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2;
                Unity_Multiply_float4_float4((_Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2.xxxx),
                _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2,
                _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2);
                float4 _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1;
                Unity_Saturate_float4(_Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2,
                                   _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1);
                float _Float_338327c01392428cbd8d5c04d29e17fd_Out_0 = 0.3;
                float4 _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2;
                Unity_Multiply_float4_float4(_Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1,
                                                              (_Float_338327c01392428cbd8d5c04d29e17fd_Out_0.xxxx),
                                                              _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2);
                float _Float_d698972aee194773aaf0c86baa0a63ec_Out_0 = 1.82;
                float4 _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2;
                Unity_Multiply_float4_float4((_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4.xxxx),
                                        _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                        _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2);
                float _Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0 = 1;
                float _Float_76f4d333507440e98b7828dd7005efee_Out_0 = 0.05;
                float _Multiply_7855554395974be9b86dae7390f79b91_Out_2;
                Unity_Multiply_float_float(_Float_76f4d333507440e98b7828dd7005efee_Out_0, IN.TimeParameters.x,
                     _Multiply_7855554395974be9b86dae7390f79b91_Out_2);
                float2 _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(2, 2),
                               (_Multiply_7855554395974be9b86dae7390f79b91_Out_2.xx),
                               _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3);
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    GetTransformedUV(_TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3));
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.r;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_G_5 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.g;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_B_6 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.b;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_A_7 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.a;
                float _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2;
                Unity_Multiply_float_float(_Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0,
        _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4,
        _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2);
                float4 _Multiply_834c2eaa87114b35845c865966815e9e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2,
                      (_Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2.xxxx),
                      _Multiply_834c2eaa87114b35845c865966815e9e_Out_2);
                float4 _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2;
                Unity_Multiply_float4_float4((_Float_d698972aee194773aaf0c86baa0a63ec_Out_0.xxxx),
                                                                   _Multiply_834c2eaa87114b35845c865966815e9e_Out_2,
                                                                   _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2);
                float4 _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2;
                Unity_Add_float4(_Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2,
                _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2, _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2);
                float4 _Add_36539599808a45a29342eb243ea8a786_Out_2;
                Unity_Add_float4(_Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2,
            _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2, _Add_36539599808a45a29342eb243ea8a786_Out_2);
                float4 _Add_33590f087cce40bb90f683745a35cb9b_Out_2;
                Unity_Add_float4(_Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2,
    _Add_36539599808a45a29342eb243ea8a786_Out_2, _Add_33590f087cce40bb90f683745a35cb9b_Out_2);
                float _Property_b06ced12d3084b4a9086885bea885d88_Out_0 = _emissive;
                float4 _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2;
                Unity_Multiply_float4_float4(_Add_33590f087cce40bb90f683745a35cb9b_Out_2,
     (_Property_b06ced12d3084b4a9086885bea885d88_Out_0.xxxx),
     _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2);
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_R_4 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.r;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_G_5 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.g;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_B_6 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.b;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_A_7 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.a;
                float _Float_34ba91092575432ea86fb00ca74874c7_Out_0 = 1.6;
                float4 _Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0,
                                                              (_Float_34ba91092575432ea86fb00ca74874c7_Out_0.xxxx),
                                                              _Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2);
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                surface.NormalTS = IN.TangentSpaceNormal;
                surface.Emission = (_Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2.xyz);
                surface.Metallic = (_Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2).x;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);


                output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
                // we want a unit length Normal Vector node in shader graph
                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_SHADOW_COORD
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_GBUFFER
            #define _FOG_FRAGMENT 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float4 tangentWS;
                float4 texCoord0;
                float3 viewDirectionWS;
                #if defined(LIGHTMAP_ON)
                     float2 staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                     float2 dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh;
                #endif
                float4 fogFactorAndVertexLight;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                     float4 shadowCoord;
                #endif
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 TangentSpaceNormal;
                float3 WorldSpaceViewDirection;
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if defined(LIGHTMAP_ON)
                     float2 staticLightmapUV : INTERP0;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                     float2 dynamicLightmapUV : INTERP1;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh : INTERP2;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                     float4 shadowCoord : INTERP3;
                #endif
                float4 tangentWS : INTERP4;
                float4 texCoord0 : INTERP5;
                float4 fogFactorAndVertexLight : INTERP6;
                float3 positionWS : INTERP7;
                float3 normalWS : INTERP8;
                float3 viewDirectionWS : INTERP9;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                    output.staticLightmapUV = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = input.shadowCoord;
                #endif
                output.tangentWS.xyzw = input.tangentWS;
                output.texCoord0.xyzw = input.texCoord0;
                output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
                output.positionWS.xyz = input.positionWS;
                output.normalWS.xyz = input.normalWS;
                output.viewDirectionWS.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                    output.staticLightmapUV = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = input.shadowCoord;
                #endif
                output.tangentWS = input.tangentWS.xyzw;
                output.texCoord0 = input.texCoord0.xyzw;
                output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
                output.positionWS = input.positionWS.xyz;
                output.normalWS = input.normalWS.xyz;
                output.viewDirectionWS = input.viewDirectionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Saturate_float4(float4 In, out float4 Out)
            {
                Out = saturate(In);
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float3 NormalTS;
                float3 Emission;
                float Metallic;
                float Smoothness;
                float Occlusion;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_R_4 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.r;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_G_5 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.g;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_B_6 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.b;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_A_7 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.a;
                float _Float_555cf4c4456443a68e341fb84c46bc2f_Out_0 = 1;
                float4 _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0,
                                         (_Float_555cf4c4456443a68e341fb84c46bc2f_Out_0.xxxx),
                                         _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2);
                float4 _Property_8425e5cd9b9343f29331b07245faccdd_Out_0 = _fresnel_color;
                float _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 3.01,
                    _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3);
                float4 _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2;
                Unity_Multiply_float4_float4(_Property_8425e5cd9b9343f29331b07245faccdd_Out_0,
                    (_FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3.xxxx),
                    _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2);
                float _Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0 = _fresnel;
                float4 _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
             (_Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0
                 .xxxx),
             _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2);
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.r;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_G_5 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.g;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_B_6 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.b;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_A_7 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.a;
                float4 _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2,
                                                                 (_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4.
                                                                     xxxx),
                                                                 _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2);
                float _Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0 = 0.72;
                float2 _Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0 = float2(-0.025, -0.05);
                float2 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2;
                Unity_Multiply_float2_float2(_Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0, (IN.TimeParameters.x.xx),
                                _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2);
                float2 _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(3.5, 3.5),
              _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2,
              _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3);
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    GetTransformedUV(_TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3));
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.r;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_G_5 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.g;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_B_6 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.b;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_A_7 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.a;
                float _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2;
                Unity_Multiply_float_float(_Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0,
                                                      _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4,
                                                      _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2);
                float _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0 = 22.81;
                float _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2;
                Unity_Multiply_float_float(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4,
                                                      _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0,
                                                      _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2);
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.r;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_G_5 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.g;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_B_6 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.b;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_A_7 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.a;
                float _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2;
                Unity_Multiply_float_float(_Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2,
                             _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4,
                             _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2);
                float _Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0 = _dots_fresnel;
                float4 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
              (_Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0.xxxx),
              _Multiply_2e84e13913da43cf9201736bb595735e_Out_2);
                float4 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2;
                Unity_Multiply_float4_float4((_Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2.xxxx),
                          _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                          _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2);
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.r;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_G_5 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.g;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_B_6 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.b;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_A_7 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.a;
                float _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1;
                Unity_OneMinus_float(_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4,
                                                 _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1);
                float4 _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2;
                Unity_Multiply_float4_float4(_Multiply_f5a58908183541c6bba1b10da96ab027_Out_2,
      (_OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1.xxxx),
      _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2);
                float4 _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2;
                Unity_Multiply_float4_float4((_Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2.xxxx),
                    _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2, _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2);
                float4 _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1;
                Unity_Saturate_float4(_Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2,
                                                             _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1);
                float _Float_338327c01392428cbd8d5c04d29e17fd_Out_0 = 0.3;
                float4 _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2;
                Unity_Multiply_float4_float4(_Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1,
   (_Float_338327c01392428cbd8d5c04d29e17fd_Out_0.
       xxxx),
   _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2);
                float _Float_d698972aee194773aaf0c86baa0a63ec_Out_0 = 1.82;
                float4 _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2;
                Unity_Multiply_float4_float4((_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4.xxxx),
                                                     _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                                     _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2);
                float _Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0 = 1;
                float _Float_76f4d333507440e98b7828dd7005efee_Out_0 = 0.05;
                float _Multiply_7855554395974be9b86dae7390f79b91_Out_2;
                Unity_Multiply_float_float(_Float_76f4d333507440e98b7828dd7005efee_Out_0, IN.TimeParameters.x,
                _Multiply_7855554395974be9b86dae7390f79b91_Out_2);
                float2 _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(2, 2),
      (_Multiply_7855554395974be9b86dae7390f79b91_Out_2.
          xx),
      _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3);
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    GetTransformedUV(_TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3));
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.r;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_G_5 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.g;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_B_6 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.b;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_A_7 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.a;
                float _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2;
                Unity_Multiply_float_float(_Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0,
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4,
                    _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2);
                float4 _Multiply_834c2eaa87114b35845c865966815e9e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2,
                                                                     (_Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2.
                                                                         xxxx),
                                                                     _Multiply_834c2eaa87114b35845c865966815e9e_Out_2);
                float4 _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2;
                Unity_Multiply_float4_float4((_Float_d698972aee194773aaf0c86baa0a63ec_Out_0.xxxx),
                     _Multiply_834c2eaa87114b35845c865966815e9e_Out_2,
                     _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2);
                float4 _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2;
                Unity_Add_float4(_Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2,
                    _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2, _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2);
                float4 _Add_36539599808a45a29342eb243ea8a786_Out_2;
                Unity_Add_float4(_Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2,
                    _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2, _Add_36539599808a45a29342eb243ea8a786_Out_2);
                float4 _Add_33590f087cce40bb90f683745a35cb9b_Out_2;
                Unity_Add_float4(_Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2,
                    _Add_36539599808a45a29342eb243ea8a786_Out_2, _Add_33590f087cce40bb90f683745a35cb9b_Out_2);
                float _Property_b06ced12d3084b4a9086885bea885d88_Out_0 = _emissive;
                float4 _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2;
                Unity_Multiply_float4_float4(_Add_33590f087cce40bb90f683745a35cb9b_Out_2,
                    (_Property_b06ced12d3084b4a9086885bea885d88_Out_0.xxxx),
                    _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2);
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_R_4 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.r;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_G_5 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.g;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_B_6 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.b;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_A_7 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.a;
                float _Float_34ba91092575432ea86fb00ca74874c7_Out_0 = 1.6;
                float4 _Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0,
                                     (_Float_34ba91092575432ea86fb00ca74874c7_Out_0.xxxx),
                                     _Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2);
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                surface.NormalTS = IN.TangentSpaceNormal;
                surface.Emission = (_Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2.xyz);
                surface.Metallic = (_Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2).x;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);


                output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
                // we want a unit length Normal Vector node in shader graph
                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : INTERP0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.normalWS.xyz = input.normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.normalWS = input.normalWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALS
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv1 : TEXCOORD1;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS;
                float4 tangentWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 TangentSpaceNormal;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 tangentWS : INTERP0;
                float3 normalWS : INTERP1;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.tangentWS.xyzw = input.tangentWS;
                output.normalWS.xyz = input.normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.tangentWS = input.tangentWS.xyzw;
                output.normalWS = input.normalWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 NormalTS;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                surface.NormalTS = IN.TangentSpaceNormal;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma shader_feature _ EDITOR_VISUALIZATION
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_META
            #define _FOG_FRAGMENT 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS;
                float4 texCoord0;
                float4 texCoord1;
                float4 texCoord2;
                float3 viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 WorldSpaceViewDirection;
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                float4 texCoord1 : INTERP1;
                float4 texCoord2 : INTERP2;
                float3 normalWS : INTERP3;
                float3 viewDirectionWS : INTERP4;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                output.texCoord1.xyzw = input.texCoord1;
                output.texCoord2.xyzw = input.texCoord2;
                output.normalWS.xyz = input.normalWS;
                output.viewDirectionWS.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                output.texCoord1 = input.texCoord1.xyzw;
                output.texCoord2 = input.texCoord2.xyzw;
                output.normalWS = input.normalWS.xyz;
                output.viewDirectionWS = input.viewDirectionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Saturate_float4(float4 In, out float4 Out)
            {
                Out = saturate(In);
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float3 Emission;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_R_4 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.r;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_G_5 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.g;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_B_6 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.b;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_A_7 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.a;
                float _Float_555cf4c4456443a68e341fb84c46bc2f_Out_0 = 1;
                float4 _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0,
                                         (_Float_555cf4c4456443a68e341fb84c46bc2f_Out_0.xxxx),
                                         _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2);
                float4 _Property_8425e5cd9b9343f29331b07245faccdd_Out_0 = _fresnel_color;
                float _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 3.01,
                    _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3);
                float4 _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2;
                Unity_Multiply_float4_float4(_Property_8425e5cd9b9343f29331b07245faccdd_Out_0,
                                        (_FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3
                                            .
                                            xxxx),
                                        _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2);
                float _Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0 = _fresnel;
                float4 _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
                                                                        (_Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0
                                                                            .xxxx),
                                                                        _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2);
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.r;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_G_5 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.g;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_B_6 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.b;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_A_7 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.a;
                float4 _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2,
                                       (_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4.xxxx),
                                       _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2);
                float _Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0 = 0.72;
                float2 _Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0 = float2(-0.025, -0.05);
                float2 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2;
                Unity_Multiply_float2_float2(_Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0, (IN.TimeParameters.x.xx),
                 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2);
                float2 _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(3.5, 3.5),
             _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2,
             _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3);
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    GetTransformedUV(_TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3));
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.r;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_G_5 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.g;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_B_6 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.b;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_A_7 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.a;
                float _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2;
                Unity_Multiply_float_float(_Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0,
                  _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4,
                  _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2);
                float _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0 = 22.81;
                float _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2;
                Unity_Multiply_float_float(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4,
              _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0, _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2);
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.r;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_G_5 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.g;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_B_6 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.b;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_A_7 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.a;
                float _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2;
                Unity_Multiply_float_float(_Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2,
        _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4,
        _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2);
                float _Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0 = _dots_fresnel;
                float4 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
            (_Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0.xxxx),
            _Multiply_2e84e13913da43cf9201736bb595735e_Out_2);
                float4 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2;
                Unity_Multiply_float4_float4((_Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2.xxxx),
                                                                 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                                                 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2);
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.r;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_G_5 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.g;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_B_6 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.b;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_A_7 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.a;
                float _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1;
                Unity_OneMinus_float(_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4,
  _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1);
                float4 _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2;
                Unity_Multiply_float4_float4(_Multiply_f5a58908183541c6bba1b10da96ab027_Out_2,
        (_OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1.xxxx),
        _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2);
                float4 _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2;
                Unity_Multiply_float4_float4((_Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2.xxxx),
                    _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2,
                    _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2);
                float4 _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1;
                Unity_Saturate_float4(_Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2,
                                              _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1);
                float _Float_338327c01392428cbd8d5c04d29e17fd_Out_0 = 0.3;
                float4 _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2;
                Unity_Multiply_float4_float4(_Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1,
                    (_Float_338327c01392428cbd8d5c04d29e17fd_Out_0.xxxx),
                    _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2);
                float _Float_d698972aee194773aaf0c86baa0a63ec_Out_0 = 1.82;
                float4 _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2;
                Unity_Multiply_float4_float4((_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4.xxxx),
                                                                               _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                                                               _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2);
                float _Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0 = 1;
                float _Float_76f4d333507440e98b7828dd7005efee_Out_0 = 0.05;
                float _Multiply_7855554395974be9b86dae7390f79b91_Out_2;
                Unity_Multiply_float_float(_Float_76f4d333507440e98b7828dd7005efee_Out_0, IN.TimeParameters.x,
   _Multiply_7855554395974be9b86dae7390f79b91_Out_2);
                float2 _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(2, 2),
                               (_Multiply_7855554395974be9b86dae7390f79b91_Out_2.xx),
                               _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3);
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    GetTransformedUV(_TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3));
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.r;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_G_5 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.g;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_B_6 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.b;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_A_7 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.a;
                float _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2;
                Unity_Multiply_float_float(_Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0,
                                _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4,
                                _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2);
                float4 _Multiply_834c2eaa87114b35845c865966815e9e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2,
                                              (_Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2.xxxx),
                                              _Multiply_834c2eaa87114b35845c865966815e9e_Out_2);
                float4 _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2;
                Unity_Multiply_float4_float4((_Float_d698972aee194773aaf0c86baa0a63ec_Out_0.xxxx),
                              _Multiply_834c2eaa87114b35845c865966815e9e_Out_2,
                              _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2);
                float4 _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2;
                Unity_Add_float4(_Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2,
                                                           _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2,
                                                           _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2);
                float4 _Add_36539599808a45a29342eb243ea8a786_Out_2;
                Unity_Add_float4(_Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2,
                                                                          _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2,
                                                                          _Add_36539599808a45a29342eb243ea8a786_Out_2);
                float4 _Add_33590f087cce40bb90f683745a35cb9b_Out_2;
                Unity_Add_float4(_Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2,
   _Add_36539599808a45a29342eb243ea8a786_Out_2,
   _Add_33590f087cce40bb90f683745a35cb9b_Out_2);
                float _Property_b06ced12d3084b4a9086885bea885d88_Out_0 = _emissive;
                float4 _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2;
                Unity_Multiply_float4_float4(_Add_33590f087cce40bb90f683745a35cb9b_Out_2,
                    (_Property_b06ced12d3084b4a9086885bea885d88_Out_0.xxxx),
                    _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2);
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                surface.Emission = (_Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2.xyz);
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);


                output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
                // we want a unit length Normal Vector node in shader graph


                output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define SCENESELECTIONPASS 1
            #define ALPHA_CLIP_THRESHOLD 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }

            // Render State
            Cull Back

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define SCENEPICKINGPASS 1
            #define ALPHA_CLIP_THRESHOLD 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_2D
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float4 uv0;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                output.uv0 = input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Lit"
            "Queue"="Geometry"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define VARYINGS_NEED_SHADOW_COORD
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_FORWARD
            #define _FOG_FRAGMENT 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float4 tangentWS;
                float4 texCoord0;
                float3 viewDirectionWS;
                #if defined(LIGHTMAP_ON)
                     float2 staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                     float2 dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh;
                #endif
                float4 fogFactorAndVertexLight;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                     float4 shadowCoord;
                #endif
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 TangentSpaceNormal;
                float3 WorldSpaceViewDirection;
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if defined(LIGHTMAP_ON)
                     float2 staticLightmapUV : INTERP0;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                     float2 dynamicLightmapUV : INTERP1;
                #endif
                #if !defined(LIGHTMAP_ON)
                float3 sh : INTERP2;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                     float4 shadowCoord : INTERP3;
                #endif
                float4 tangentWS : INTERP4;
                float4 texCoord0 : INTERP5;
                float4 fogFactorAndVertexLight : INTERP6;
                float3 positionWS : INTERP7;
                float3 normalWS : INTERP8;
                float3 viewDirectionWS : INTERP9;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                    output.staticLightmapUV = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = input.shadowCoord;
                #endif
                output.tangentWS.xyzw = input.tangentWS;
                output.texCoord0.xyzw = input.texCoord0;
                output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
                output.positionWS.xyz = input.positionWS;
                output.normalWS.xyz = input.normalWS;
                output.viewDirectionWS.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if defined(LIGHTMAP_ON)
                    output.staticLightmapUV = input.staticLightmapUV;
                #endif
                #if defined(DYNAMICLIGHTMAP_ON)
                    output.dynamicLightmapUV = input.dynamicLightmapUV;
                #endif
                #if !defined(LIGHTMAP_ON)
                output.sh = input.sh;
                #endif
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    output.shadowCoord = input.shadowCoord;
                #endif
                output.tangentWS = input.tangentWS.xyzw;
                output.texCoord0 = input.texCoord0.xyzw;
                output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
                output.positionWS = input.positionWS.xyz;
                output.normalWS = input.normalWS.xyz;
                output.viewDirectionWS = input.viewDirectionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Saturate_float4(float4 In, out float4 Out)
            {
                Out = saturate(In);
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float3 NormalTS;
                float3 Emission;
                float Metallic;
                float Smoothness;
                float Occlusion;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_R_4 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.r;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_G_5 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.g;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_B_6 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.b;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_A_7 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.a;
                float _Float_555cf4c4456443a68e341fb84c46bc2f_Out_0 = 1;
                float4 _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0,
                                         (_Float_555cf4c4456443a68e341fb84c46bc2f_Out_0.xxxx),
                                         _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2);
                float4 _Property_8425e5cd9b9343f29331b07245faccdd_Out_0 = _fresnel_color;
                float _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 3.01,
                    _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3);
                float4 _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2;
                Unity_Multiply_float4_float4(_Property_8425e5cd9b9343f29331b07245faccdd_Out_0,
                    (_FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3.xxxx),
                    _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2);
                float _Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0 = _fresnel;
                float4 _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
                     (_Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0
                         .xxxx),
                     _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2);
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.r;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_G_5 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.g;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_B_6 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.b;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_A_7 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.a;
                float4 _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2,
(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4.xxxx),
_Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2);
                float _Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0 = 0.72;
                float2 _Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0 = float2(-0.025, -0.05);
                float2 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2;
                Unity_Multiply_float2_float2(_Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0, (IN.TimeParameters.x.xx),
                                                    _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2);
                float2 _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(3.5, 3.5),
                    _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2,
                    _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3);
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    GetTransformedUV(_TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3));
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.r;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_G_5 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.g;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_B_6 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.b;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_A_7 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.a;
                float _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2;
                Unity_Multiply_float_float(_Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0,
                                        _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4,
                                        _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2);
                float _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0 = 22.81;
                float _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2;
                Unity_Multiply_float_float(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4,
                                                                 _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0,
                                                                 _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2);
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.r;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_G_5 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.g;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_B_6 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.b;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_A_7 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.a;
                float _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2;
                Unity_Multiply_float_float(_Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2,
     _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4,
     _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2);
                float _Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0 = _dots_fresnel;
                float4 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
                                 (_Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0.xxxx),
                                 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2);
                float4 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2;
                Unity_Multiply_float4_float4((_Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2.xxxx),
                        _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                        _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2);
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.r;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_G_5 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.g;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_B_6 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.b;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_A_7 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.a;
                float _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1;
                Unity_OneMinus_float(_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4,
                                                     _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1);
                float4 _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2;
                Unity_Multiply_float4_float4(_Multiply_f5a58908183541c6bba1b10da96ab027_Out_2,
        (_OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1.xxxx),
        _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2);
                float4 _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2;
                Unity_Multiply_float4_float4((_Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2.xxxx),
                                                             _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2,
                                                             _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2);
                float4 _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1;
                Unity_Saturate_float4(_Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2,
                                                       _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1);
                float _Float_338327c01392428cbd8d5c04d29e17fd_Out_0 = 0.3;
                float4 _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2;
                Unity_Multiply_float4_float4(_Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1,
                                                   (_Float_338327c01392428cbd8d5c04d29e17fd_Out_0.xxxx),
                                                   _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2);
                float _Float_d698972aee194773aaf0c86baa0a63ec_Out_0 = 1.82;
                float4 _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2;
                Unity_Multiply_float4_float4((_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4.xxxx),
                                      _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                      _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2);
                float _Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0 = 1;
                float _Float_76f4d333507440e98b7828dd7005efee_Out_0 = 0.05;
                float _Multiply_7855554395974be9b86dae7390f79b91_Out_2;
                Unity_Multiply_float_float(_Float_76f4d333507440e98b7828dd7005efee_Out_0, IN.TimeParameters.x,
           _Multiply_7855554395974be9b86dae7390f79b91_Out_2);
                float2 _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(2, 2),
           (_Multiply_7855554395974be9b86dae7390f79b91_Out_2.xx),
           _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3);
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    GetTransformedUV(_TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3));
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.r;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_G_5 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.g;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_B_6 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.b;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_A_7 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.a;
                float _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2;
                Unity_Multiply_float_float(_Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0,
                                                                _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4,
                                                                _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2);
                float4 _Multiply_834c2eaa87114b35845c865966815e9e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2,
      (_Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2.
          xxxx),
      _Multiply_834c2eaa87114b35845c865966815e9e_Out_2);
                float4 _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2;
                Unity_Multiply_float4_float4((_Float_d698972aee194773aaf0c86baa0a63ec_Out_0.xxxx),
        _Multiply_834c2eaa87114b35845c865966815e9e_Out_2,
        _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2);
                float4 _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2;
                Unity_Add_float4(_Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2,
                    _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2,
                    _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2);
                float4 _Add_36539599808a45a29342eb243ea8a786_Out_2;
                Unity_Add_float4(_Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2,
                                                   _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2,
                                                   _Add_36539599808a45a29342eb243ea8a786_Out_2);
                float4 _Add_33590f087cce40bb90f683745a35cb9b_Out_2;
                Unity_Add_float4(_Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2,
    _Add_36539599808a45a29342eb243ea8a786_Out_2, _Add_33590f087cce40bb90f683745a35cb9b_Out_2);
                float _Property_b06ced12d3084b4a9086885bea885d88_Out_0 = _emissive;
                float4 _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2;
                Unity_Multiply_float4_float4(_Add_33590f087cce40bb90f683745a35cb9b_Out_2,
            (_Property_b06ced12d3084b4a9086885bea885d88_Out_0.xxxx),
            _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2);
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_R_4 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.r;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_G_5 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.g;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_B_6 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.b;
                float _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_A_7 =
                    _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0.a;
                float _Float_34ba91092575432ea86fb00ca74874c7_Out_0 = 1.6;
                float4 _Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_RGBA_0,
                                                   (_Float_34ba91092575432ea86fb00ca74874c7_Out_0.xxxx),
                                                   _Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2);
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                surface.NormalTS = IN.TangentSpaceNormal;
                surface.Emission = (_Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2.xyz);
                surface.Metallic = (_Multiply_ea0e71dea869424e81b4cfa1cf9779c0_Out_2).x;
                surface.Smoothness = 0.5;
                surface.Occlusion = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);


                output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
                // we want a unit length Normal Vector node in shader graph
                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_NORMAL_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : INTERP0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.normalWS.xyz = input.normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.normalWS = input.normalWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // Render State
            Cull Back
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TANGENT_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHNORMALS
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv1 : TEXCOORD1;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS;
                float4 tangentWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 TangentSpaceNormal;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 tangentWS : INTERP0;
                float3 normalWS : INTERP1;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.tangentWS.xyzw = input.tangentWS;
                output.normalWS.xyz = input.normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.tangentWS = input.tangentWS.xyzw;
                output.normalWS = input.normalWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 NormalTS;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                surface.NormalTS = IN.TangentSpaceNormal;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma shader_feature _ EDITOR_VISUALIZATION
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_META
            #define _FOG_FRAGMENT 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS;
                float4 texCoord0;
                float4 texCoord1;
                float4 texCoord2;
                float3 viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 WorldSpaceViewDirection;
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                float4 texCoord1 : INTERP1;
                float4 texCoord2 : INTERP2;
                float3 normalWS : INTERP3;
                float3 viewDirectionWS : INTERP4;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                output.texCoord1.xyzw = input.texCoord1;
                output.texCoord2.xyzw = input.texCoord2;
                output.normalWS.xyz = input.normalWS;
                output.viewDirectionWS.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                output.texCoord1 = input.texCoord1.xyzw;
                output.texCoord2 = input.texCoord2.xyzw;
                output.normalWS = input.normalWS.xyz;
                output.viewDirectionWS = input.viewDirectionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Saturate_float4(float4 In, out float4 Out)
            {
                Out = saturate(In);
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float3 Emission;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_R_4 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.r;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_G_5 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.g;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_B_6 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.b;
                float _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_A_7 =
                    _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0.a;
                float _Float_555cf4c4456443a68e341fb84c46bc2f_Out_0 = 1;
                float4 _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2;
                Unity_Multiply_float4_float4(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_RGBA_0,
                                         (_Float_555cf4c4456443a68e341fb84c46bc2f_Out_0.xxxx),
                                         _Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2);
                float4 _Property_8425e5cd9b9343f29331b07245faccdd_Out_0 = _fresnel_color;
                float _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 3.01,
                    _FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3);
                float4 _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2;
                Unity_Multiply_float4_float4(_Property_8425e5cd9b9343f29331b07245faccdd_Out_0,
                                        (_FresnelEffect_90dc49e214b4477e85bee9bfb7c6f2b6_Out_3
                                            .
                                            xxxx),
                                        _Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2);
                float _Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0 = _fresnel;
                float4 _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
                                                                        (_Property_a0c9cb67a81a434f9c6666a1f0dce3a7_Out_0
                                                                            .xxxx),
                                                                        _Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2);
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.r;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_G_5 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.g;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_B_6 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.b;
                float _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_A_7 =
                    _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_RGBA_0.a;
                float4 _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_2a16978692d04966bfa62500f8fa71cf_Out_2,
                                       (_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4.xxxx),
                                       _Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2);
                float _Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0 = 0.72;
                float2 _Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0 = float2(-0.025, -0.05);
                float2 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2;
                Unity_Multiply_float2_float2(_Vector2_10f7f5f32afe4d889c65f82a572f406a_Out_0, (IN.TimeParameters.x.xx),
                 _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2);
                float2 _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(3.5, 3.5),
             _Multiply_eba0d0305c654db0a0886c484beab49d_Out_2,
             _TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3);
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1).
                    GetTransformedUV(_TilingAndOffset_e3b194d12b744d28899eb90d472a24fe_Out_3));
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.r;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_G_5 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.g;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_B_6 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.b;
                float _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_A_7 =
                    _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_RGBA_0.a;
                float _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2;
                Unity_Multiply_float_float(_Float_7e74eeacee1b4860bef4dacf89d538b7_Out_0,
                  _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_R_4,
                  _Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2);
                float _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0 = 22.81;
                float _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2;
                Unity_Multiply_float_float(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_R_4,
              _Float_dd6fbcb8f0404546b6e02a95503edbf3_Out_0, _Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2);
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.r;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_G_5 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.g;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_B_6 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.b;
                float _SampleTexture2D_b524524c3732450fb5b44441370b3477_A_7 =
                    _SampleTexture2D_b524524c3732450fb5b44441370b3477_RGBA_0.a;
                float _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2;
                Unity_Multiply_float_float(_Multiply_0a941cbb3e434cd1a9cecb1613b99e8b_Out_2,
        _SampleTexture2D_b524524c3732450fb5b44441370b3477_R_4,
        _Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2);
                float _Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0 = _dots_fresnel;
                float4 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_a72414a1c1fc42a581e665ffe1408f06_Out_2,
            (_Property_d4741ceda9f44b0492e0c3381b32c6ba_Out_0.xxxx),
            _Multiply_2e84e13913da43cf9201736bb595735e_Out_2);
                float4 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2;
                Unity_Multiply_float4_float4((_Multiply_10233e58adeb4ce2b769a0ca968559a7_Out_2.xxxx),
                                                                 _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                                                 _Multiply_f5a58908183541c6bba1b10da96ab027_Out_2);
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.r;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_G_5 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.g;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_B_6 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.b;
                float _SampleTexture2D_b392423c441e457cb7df83337a58c334_A_7 =
                    _SampleTexture2D_b392423c441e457cb7df83337a58c334_RGBA_0.a;
                float _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1;
                Unity_OneMinus_float(_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4,
  _OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1);
                float4 _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2;
                Unity_Multiply_float4_float4(_Multiply_f5a58908183541c6bba1b10da96ab027_Out_2,
        (_OneMinus_4573b32c789c4492872c0b91eb60f2b2_Out_1.xxxx),
        _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2);
                float4 _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2;
                Unity_Multiply_float4_float4((_Multiply_9f4707e8a6ec4c6ca16c83b2f73b3de5_Out_2.xxxx),
                    _Multiply_fb74cdd4e6f84001955b173603c452d8_Out_2,
                    _Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2);
                float4 _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1;
                Unity_Saturate_float4(_Multiply_b53f64e5f0d046f8a036f747f94bf480_Out_2,
                                              _Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1);
                float _Float_338327c01392428cbd8d5c04d29e17fd_Out_0 = 0.3;
                float4 _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2;
                Unity_Multiply_float4_float4(_Saturate_e44a7dbbec92454f9ea86b697ae4b6a3_Out_1,
                    (_Float_338327c01392428cbd8d5c04d29e17fd_Out_0.xxxx),
                    _Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2);
                float _Float_d698972aee194773aaf0c86baa0a63ec_Out_0 = 1.82;
                float4 _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2;
                Unity_Multiply_float4_float4((_SampleTexture2D_b392423c441e457cb7df83337a58c334_R_4.xxxx),
                                                                               _Multiply_2e84e13913da43cf9201736bb595735e_Out_2,
                                                                               _Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2);
                float _Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0 = 1;
                float _Float_76f4d333507440e98b7828dd7005efee_Out_0 = 0.05;
                float _Multiply_7855554395974be9b86dae7390f79b91_Out_2;
                Unity_Multiply_float_float(_Float_76f4d333507440e98b7828dd7005efee_Out_0, IN.TimeParameters.x,
   _Multiply_7855554395974be9b86dae7390f79b91_Out_2);
                float2 _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2(2, 2),
                               (_Multiply_7855554395974be9b86dae7390f79b91_Out_2.xx),
                               _TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3);
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1).
                    GetTransformedUV(_TilingAndOffset_fbc62c31e3e644128193290c001aaf85_Out_3));
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.r;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_G_5 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.g;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_B_6 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.b;
                float _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_A_7 =
                    _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_RGBA_0.a;
                float _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2;
                Unity_Multiply_float_float(_Float_52c3885d8fc04717860f2b6d2c1f184e_Out_0,
                                _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_R_4,
                                _Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2);
                float4 _Multiply_834c2eaa87114b35845c865966815e9e_Out_2;
                Unity_Multiply_float4_float4(_Multiply_e781bf35a48c4c98bc484182bc5bc714_Out_2,
                                              (_Multiply_a682775ba82644a1b1d2d3fc3a3e388e_Out_2.xxxx),
                                              _Multiply_834c2eaa87114b35845c865966815e9e_Out_2);
                float4 _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2;
                Unity_Multiply_float4_float4((_Float_d698972aee194773aaf0c86baa0a63ec_Out_0.xxxx),
                              _Multiply_834c2eaa87114b35845c865966815e9e_Out_2,
                              _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2);
                float4 _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2;
                Unity_Add_float4(_Multiply_e9746d3f2d9145cabafb9167abd47b4a_Out_2,
                                                           _Multiply_49ac49f0a4174d2cb51411a62b0e2c38_Out_2,
                                                           _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2);
                float4 _Add_36539599808a45a29342eb243ea8a786_Out_2;
                Unity_Add_float4(_Multiply_2d33e70368f04490bf7cb11d44a3a73e_Out_2,
                                                                          _Add_8b3deb29b1c7494ab96efb62013c1392_Out_2,
                                                                          _Add_36539599808a45a29342eb243ea8a786_Out_2);
                float4 _Add_33590f087cce40bb90f683745a35cb9b_Out_2;
                Unity_Add_float4(_Multiply_9f7cdd34fd9346d0894a9d436b724664_Out_2,
   _Add_36539599808a45a29342eb243ea8a786_Out_2,
   _Add_33590f087cce40bb90f683745a35cb9b_Out_2);
                float _Property_b06ced12d3084b4a9086885bea885d88_Out_0 = _emissive;
                float4 _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2;
                Unity_Multiply_float4_float4(_Add_33590f087cce40bb90f683745a35cb9b_Out_2,
                    (_Property_b06ced12d3084b4a9086885bea885d88_Out_0.xxxx),
                    _Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2);
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                surface.Emission = (_Multiply_aa57406abab249d7ad50a59e6a18312a_Out_2.xyz);
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
                float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);


                output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;
                // we want a unit length Normal Vector node in shader graph


                output.WorldSpaceViewDirection = normalize(input.viewDirectionWS);
                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define SCENESELECTIONPASS 1
            #define ALPHA_CLIP_THRESHOLD 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }

            // Render State
            Cull Back

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define SCENEPICKINGPASS 1
            #define ALPHA_CLIP_THRESHOLD 1
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define _NORMALMAP 1
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_2D
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float4 uv0;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1_TexelSize;
                float4 _SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1_TexelSize;
                float4 _SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1_TexelSize;
                float4 _SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1_TexelSize;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1_TexelSize;
                float4 _SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1_TexelSize;
                float4 _SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1_TexelSize;
                float4 _SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1_TexelSize;
                float _emissive;
                float _dots_fresnel;
                float _fresnel;
                float4 _fresnel_color;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            SAMPLER(sampler_SampleTexture2D_016f1b331731498ea31cfa6ba1ce9222_Texture_1);
            TEXTURE2D(_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            SAMPLER(sampler_SampleTexture2D_3f13a828b79546dfba2ea52470db1b18_Texture_1);
            TEXTURE2D(_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            SAMPLER(sampler_SampleTexture2D_5798e82b928c4efdb19a74cfe732920d_Texture_1);
            TEXTURE2D(_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            SAMPLER(sampler_SampleTexture2D_6ac9faba29a6418990029073a0fc8752_Texture_1);
            TEXTURE2D(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1);
            TEXTURE2D(_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            SAMPLER(sampler_SampleTexture2D_a4ce4d3657ae4b0386d94a0fb4c277e4_Texture_1);
            TEXTURE2D(_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b392423c441e457cb7df83337a58c334_Texture_1);
            TEXTURE2D(_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);
            SAMPLER(sampler_SampleTexture2D_b524524c3732450fb5b44441370b3477_Texture_1);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0 = SAMPLE_TEXTURE2D(
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).tex,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    samplerstate,
                    UnityBuildTexture2DStructNoScale(_SampleTexture2D_a221363772c74bd197752822f22d8c51_Texture_1).
                    GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_R_4 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.r;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_G_5 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.g;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_B_6 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.b;
                float _SampleTexture2D_a221363772c74bd197752822f22d8c51_A_7 =
                    _SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.a;
                surface.BaseColor = (_SampleTexture2D_a221363772c74bd197752822f22d8c51_RGBA_0.xyz);
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                // FragInputs from VFX come from two places: Interpolator or CBuffer.
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                output.uv0 = input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }


            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}