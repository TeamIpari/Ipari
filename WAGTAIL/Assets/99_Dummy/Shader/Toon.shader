Shader "Roystan/Toon"
{

	//tags
	//{
	//	"lightmode" = "forwardbase"
	//	"passflags" = "onlydriectional"
	//}

	Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}	
		_AmbientColor("AmBient Color", Color) = (0.4, 0.4, .4, 1)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _AmbientColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			float4 _Color;
#include "Lighting.cginc"	
			float4 frag (v2f i) : SV_Target
			{
				float4 sample = tex2D(_MainTex, i.uv);
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);	// 사실적인 조명 스타일 만들기.
				// WorldSpaceLightPos의 값을 0 ~ 1사이로 받아 냄;

				float lightIntensity = NdotL > 0 ? 1 : 0;	// Light 와 Dark 두가지의 대역으로 나눌 경우.

				float4 light = lightIntensity * _LightColor0;

				return _Color * sample * (_AmbientColor + light);
			}
			ENDCG
		}
	}
}