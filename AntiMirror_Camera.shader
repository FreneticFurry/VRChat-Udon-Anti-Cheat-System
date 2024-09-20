Shader "Frenetic/Anti Mirror&Camera"
{
    Properties
    {
        [Enum(Mirrors_Cameras, 0, Cameras, 1, Mirrors, 2, Player, 3)] _HideUIIn ("Hide UI In", Float) = 0
        [Enum(Both, 0, Cameras, 1, Mirrors, 2)] _RenderIn ("Render In", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Overlay+999999999" "RenderType"="Overlay+999999999" }

        Pass
        {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _RenderIn;
            float _VRChatCameraMode;
            float _VRChatMirrorMode;

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                float2 uv = float2((id << 1) & 2, id & 2);
                o.vertex = float4(uv * 2 - 1, 1, 1);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                bool renderInCamera = (_VRChatCameraMode != 0.0);
                bool renderInMirror = (_VRChatMirrorMode != 0.0);

                if (_RenderIn == 0 && !(renderInCamera || renderInMirror)) discard;
                if (_RenderIn == 1 && !renderInCamera) discard;
                if (_RenderIn == 2 && !renderInMirror) discard;

                return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }

        // Hide UI
        Pass
        {
            Tags { "Queue" = "Transparent-1" }
            ZWrite On
            Cull Off

            Blend Zero One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _HideUIIn;
            float _VRChatCameraMode;
            float _VRChatMirrorMode;

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                float2 uv = float2((id << 1) & 2, id & 2);
                o.vertex = float4(uv * 2 - 1, 1, 1);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                bool renderInCamera = (_VRChatCameraMode != 0.0);
                bool renderInMirror = (_VRChatMirrorMode != 0.0);

                if (_HideUIIn == 0)
                {
                    if (!(renderInCamera || renderInMirror)) discard;
                }
                else if (_HideUIIn == 1)
                {
                    if (!renderInCamera) discard;
                }
                else if (_HideUIIn == 2)
                {
                    if (!renderInMirror) discard;
                }
                else if (_HideUIIn == 3)
                {}
                else
                {
                    discard;
                }
                return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
