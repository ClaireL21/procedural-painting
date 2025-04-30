// Upgrade NOTE: commented out 'float4x4 _CameraToWorld', a built-in variable
// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

Shader"CustomRenderTexture/WorldPosition"
{
    Properties
    {
        _MainTex("InputTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
ZTest Always
Cull Off
ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _CameraDepthTexture;

float4x4 _CameraInverseProjection;
// float4x4 _CameraToWorld;

struct appdata_t
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
};

v2f vert(appdata_t IN)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(IN.vertex);
    o.uv = IN.texcoord;
    return o;
}

float3 ReconstructWorldPos(float2 uv)
{
                // 1. Sample depth
    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);

                // 2. Reconstruct clip space position
    float4 clipPos = float4(uv * 2 - 1, depth, 1);

                // 3. Inverse projection to view space
    float4 viewPos = mul(_CameraInverseProjection, clipPos);
    viewPos.xyz /= viewPos.w;

                // 4. View to world space
    float4 worldPos = mul(unity_CameraToWorld, viewPos);
    return worldPos.xyz;
}

fixed4 frag(v2f i) : SV_Target
{
    float3 worldPos = ReconstructWorldPos(i.uv);
                
                // Example: visualize world position (normalized)
    return float4(worldPos * 0.05, 1.0); // Scale down to visualize
}
            ENDCG
        }
    }
}
