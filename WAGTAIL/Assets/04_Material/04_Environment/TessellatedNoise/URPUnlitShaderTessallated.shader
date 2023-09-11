// This shader adds tessellation in URP
Shader "Example/URPUnlitShaderTessallated"
{
	// The properties block of the Unity shader. In this example this block is empty
	// because the output color is predefined in the fragment shader code.
	Properties
	{
		_Tess("Tessellation", Range(1, 32)) = 20
		_MaxTessDistance("Max Tess Distance", float) = 20
		_FirstNoise("FirstNoise", 2D) = "gray" {}
		_SecondNoise("SecondNoise",2D) = "gray" {}
		_Weight("Displacement Amount", float) = 0
		[HDR] _ColorHigh("High Color",Color) = (1,1,1,1)
		[HDR] _ColorLow("Low Color",Color) = (0,0,0,0)
		_XScroll("X Scroll Speed",float) = 0		
		_YScroll("Y Scroll Speed",float) = 0
	}
 
	// The SubShader block containing the Shader code. 
	SubShader
	{
		// SubShader Tags define when and under which conditions a SubShader block or
		// a pass is executed.
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
 
		Pass
		{
			Tags{ "LightMode" = "UniversalForward" }
 
 
			// The HLSL code block. Unity SRP uses the HLSL language.
			HLSLPROGRAM
			// The Core.hlsl file contains definitions of frequently used HLSL
			// macros and functions, and also contains #include references to other
			// HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"    
			#include "CustomTessellation.hlsl"
 
			// This line defines the name of the hull shader. 
			#pragma hull hull
			// This line defines the name of the domain shader. 
			#pragma domain domain
			// This line defines the name of the vertex shader. 
			#pragma vertex TessellationVertexProgram
			// This line defines the name of the fragment shader. 
			#pragma fragment frag
 
			sampler2D _FirstNoise;
			float4 _FirstNoise_ST;
			sampler2D _SecondNoise;
			float4 _SecondNoise_ST;
			float _Weight;

			float _XScroll;
			float _YScroll;

			float4 _ColorHigh;
			float4 _ColorLow;
 
			// pre tesselation vertex program
			ControlPoint TessellationVertexProgram(Attributes v)
			{
				ControlPoint p;
 
				p.vertex = v.vertex;
				p.uv = v.uv;
				p.normal = v.normal;
				p.color = v.color;
 
				return p;
			}
 
			// after tesselation
			Varyings vert(Attributes input)
			{
				Varyings output;
				float2 transformedUV = TRANSFORM_TEX(input.uv,_FirstNoise);
				float4 Noise = tex2Dlod(_FirstNoise, float4(transformedUV + float2(_Time.x * _XScroll,_Time.x * _YScroll),0, 0));
				transformedUV = TRANSFORM_TEX(input.uv,_SecondNoise);
				Noise += tex2Dlod(_SecondNoise, float4(transformedUV + float2(_Time.x * -_XScroll,_Time.x * -_YScroll),0, 0));

				Noise = saturate(Noise/2);

				input.vertex.xyz += normalize(input.normal) *  Noise.r * _Weight;
				output.vertex = TransformObjectToHClip(input.vertex.xyz);
				output.color = input.color;
				output.normal = input.normal;
				output.uv = input.uv;
 
				return output;
			}
 
			[UNITY_domain("tri")]
			Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
			{
				Attributes v;
				// interpolate the new positions of the tessellated mesh
				Interpolate(vertex)
				Interpolate(uv)
				Interpolate(color)
				Interpolate(normal)
 
				return vert(v);
			}
 
			// The fragment shader definition.            
			half4 frag(Varyings IN) : SV_Target
			{
				float2 transformedUV = TRANSFORM_TEX(IN.uv,_FirstNoise);
				float4 Noise = tex2D(_FirstNoise, transformedUV + float2(_Time.x * _XScroll,_Time.x * _YScroll)).r;
				transformedUV = TRANSFORM_TEX(IN.uv,_SecondNoise);
				Noise += tex2D(_SecondNoise, transformedUV + float2(_Time.x * -_XScroll,_Time.x * -_YScroll)).r;
				Noise = lerp(_ColorLow,_ColorHigh, saturate(Noise/2));
				return Noise;
			}
			ENDHLSL
		}
	}

	CustomEditor "NoiseTessellationGUI"
}