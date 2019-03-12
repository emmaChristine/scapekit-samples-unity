Shader "TransparentBG"
{
    Properties {

        _alpha ("Alpha", Range(0.0, 1.0)) = 0.7
        _color ("Color", Color) = (0.9, 0.6, 0.4, 1.0)
    }
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            float _alpha;
            float4 _color;

            sampler2D _BackgroundTexture;

            half4 frag(v2f i) : SV_Target
            {
                half4 output = tex2Dproj(_BackgroundTexture, i.grabPos);
                return lerp(output, _color, _alpha);
            }
            ENDCG
        }

    }
}