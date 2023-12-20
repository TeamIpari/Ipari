Shader "Custom/SandWaveMat" {
    
    Properties {
        _SubTex         ("Texture2", 2D)                = "white" {}
        _NormalizedTime ("NormalizedTime", Range(0,1) ) = 0
        _TotalFrame     ("TotalFrame", Float)           = 11
        _TimeScale      ("TimeScale", Float)            = 0
        _Width          ( "Width", Float)               = 11
        _Height         ( "Height", Float)              = 11
    }

    SubShader {
        // No culling or depth
        Tags { 
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "AlphaTest"
        }
        LOD 100

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _SubTex;
            fixed     _TotalFrame;
            fixed     _Width;
            fixed     _Height;
            fixed     _NormalizedTime;
            fixed     _TimeScale;


            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            

            fixed4 frag(v2f i) : SV_Target{

                fixed frame = floor((_Time.y % 1)*_TotalFrame);

                const static fixed div13 = (1.f/_Width);
                const static fixed div5  = (1.f/_Height);

                fixed xOffset = (i.uv.x * div13);
                fixed yOffset = (i.uv.y * div5);

                fixed xPos   = (floor(frame % _Width) * div13) + xOffset;
                fixed yPos   = ((1.f-div5) - (floor(frame * div13) * div5)) + yOffset;
                fixed4 color = tex2D(_SubTex, fixed2(xPos, yPos));
                return color;
            }
            ENDCG
        }
    }
}
