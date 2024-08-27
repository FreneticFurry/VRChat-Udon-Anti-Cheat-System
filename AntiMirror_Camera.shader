Shader "Frenetic/AntiMirror&Camera"
{ // Known Issue: players can use "UI" to cheat and know when to turn around or not but it makes it harder because if a player is on their team it'd show them that the player is cheating if they just suddenly turn around, will try to fix this later so they cant see that either.
    Properties
    {
        _CameraTex ("Camera Texture", 2D) = "white" {}
        _MirrorTex ("Mirror Texture", 2D) = "white" {}
        [Enum(Both, 0, Cameras, 1, Mirrors, 2)] _RenderIn ("Render In", Float) = 0
        [Toggle] _Animate ("Animate", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Overlay+999999999" "RenderType"="Overlay+999999999" }
        LOD 200

        ZTest Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Stencil
            {
                Ref 255
                Comp Always
                Pass Replace
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _CameraTex;
            float4 _CameraTex_ST;
            sampler2D _MirrorTex;
            float4 _MirrorTex_ST;
            float _RenderIn;
            float _VRChatCameraMode;
            float _VRChatMirrorMode;
            float _Animate;

            v2f vert (uint id : SV_VertexID)
            {
                v2f o;

                float2 positions[4] = { float2(-1, -1), float2(1, -1), float2(-1, 1), float2(1, 1) };
                float2 texCoords[4] = { float2(0, 0), float2(1, 0), float2(0, 1), float2(1, 1) };

                if (_VRChatCameraMode != 0.0 && _VRChatMirrorMode == 0.0) 
                {
                    positions[0].y = -positions[0].y;
                    positions[1].y = -positions[1].y;
                    positions[2].y = -positions[2].y;
                    positions[3].y = -positions[3].y;
                }

                o.vertex = float4(positions[id].x, positions[id].y, 0, 1);
                o.uv = texCoords[id];
                
                if (_VRChatMirrorMode != 0.0)
                {
                    o.uv.y = 1.0 - o.uv.y;
                }
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                bool renderInCamera = (_VRChatCameraMode != 0.0);
                bool renderInMirror = (_VRChatMirrorMode != 0.0);

                if (_RenderIn == 0 && !(renderInCamera || renderInMirror)) discard;
                if (_RenderIn == 1 && !renderInCamera) discard;
                if (_RenderIn == 2 && !renderInMirror) discard;

                fixed4 col;
                if (renderInMirror)
                {
                    i.uv = TRANSFORM_TEX(i.uv, _MirrorTex);
                    col = tex2D(_MirrorTex, i.uv);
                }
                else
                {
                    i.uv = TRANSFORM_TEX(i.uv, _CameraTex);
                    col = tex2D(_CameraTex, i.uv);
                }
                
                if (_Animate > 0.5)
                {
                    col *= lerp(fixed4(1, 0, 0, 1), fixed4(1, 1, 1, 1), (sin(_Time.y * 0.75) + 1) * 0.5);
                }
                
                return col;
            }
            ENDCG
        }
    }
}
