Shader "Frenetic/Anti Mirror&Camera"
{
    Properties
    {
        [Enum(None, 4, Mirrors and Cameras , 0, Cameras, 1, Mirrors, 2, Player, 3)] _HideUIIn ("Hide UI In", Float) = 4
        [Enum(None, 3, Both, 0, Cameras, 1, Mirrors, 2)] _RenderIn ("Render In", Float) = 3
        [Toggle] _Blackout ("Blackout", Float) = 0
        _BlackoutAmt ("Blackout Amount", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Overlay+225375" }

        Pass
        {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            UNITY_DEFINE_INSTANCED_PROP(float, _RenderIn)
            UNITY_DEFINE_INSTANCED_PROP(float, _VRChatCameraMode)
            UNITY_DEFINE_INSTANCED_PROP(float, _VRChatMirrorMode)
            UNITY_DEFINE_INSTANCED_PROP(float, _Blackout)
            UNITY_DEFINE_INSTANCED_PROP(float, _BlackoutAmt)

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                float2 uv = float2((id << 1) & 2, id & 2);
                o.vertex = float4(uv * 2 - 1, 1, 1);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                bool renderInCamera = UNITY_ACCESS_INSTANCED_PROP(_VRChatCameraMode_arr, _VRChatCameraMode);
                bool renderInMirror = UNITY_ACCESS_INSTANCED_PROP(_VRChatMirrorMode_arr, _VRChatMirrorMode);

                if (UNITY_ACCESS_INSTANCED_PROP(_Blackout_arr, _Blackout) >= 0.5)
                    return fixed4(0, 0, 0, UNITY_ACCESS_INSTANCED_PROP(_BlackoutAmt_arr, _BlackoutAmt));
                if (UNITY_ACCESS_INSTANCED_PROP(_RenderIn_arr, _RenderIn) == 3) discard;
                if (UNITY_ACCESS_INSTANCED_PROP(_RenderIn_arr, _RenderIn) == 0 && !(renderInCamera || renderInMirror)) discard;
                if (UNITY_ACCESS_INSTANCED_PROP(_RenderIn_arr, _RenderIn) == 1 && !renderInCamera) discard;
                if (UNITY_ACCESS_INSTANCED_PROP(_RenderIn_arr, _RenderIn) == 2 && !renderInMirror) discard;

                return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }

        // Hide UI
        Pass
        {
            ZWrite On
            Cull Off

            Blend Zero One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "UnityCG.cginc"

            UNITY_DEFINE_INSTANCED_PROP(float, _HideUIIn)
            UNITY_DEFINE_INSTANCED_PROP(float, _VRChatCameraMode)
            UNITY_DEFINE_INSTANCED_PROP(float, _VRChatMirrorMode)

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                float2 uv = float2((id << 1) & 2, id & 2);
                o.vertex = float4(uv * 2 - 1, 1, 1);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                bool renderInCamera = UNITY_ACCESS_INSTANCED_PROP(_VRChatCameraMode_arr, _VRChatCameraMode);
                bool renderInMirror = UNITY_ACCESS_INSTANCED_PROP(_VRChatMirrorMode_arr, _VRChatMirrorMode);

                if (UNITY_ACCESS_INSTANCED_PROP(_HideUIIn_arr, _HideUIIn) == 4) discard;
                if (UNITY_ACCESS_INSTANCED_PROP(_HideUIIn_arr, _HideUIIn) == 0 && !(renderInCamera || renderInMirror)) discard;
                else if (UNITY_ACCESS_INSTANCED_PROP(_HideUIIn_arr, _HideUIIn) == 1 && !renderInCamera) discard;
                else if (UNITY_ACCESS_INSTANCED_PROP(_HideUIIn_arr, _HideUIIn) == 2 && !renderInMirror) discard;

                return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
