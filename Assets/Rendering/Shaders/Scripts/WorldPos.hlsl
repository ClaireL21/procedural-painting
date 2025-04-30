SAMPLER(sampler_point_clamp);

void WorldPos_float(float2 UVout, out float3 OUT)
{
   //float4x4 projInv = UNITY_MATRIX_I_P;
   //float4x4 viewInv = UNITY_MATRIX_I_V;
   //float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
   //
   //
   //float4 clipPos = float4(UV * 2.0 - 1.0, depth, 1.0);
   //float4 viewPos = mul(projInv, clipPos);
   //viewPos /= viewPos.w;
   //
   //float4 worldPos = mul(viewInv, viewPos);
   float3 worldPos = (1, 1, 1);
   OUT = worldPos.xyz;
    
}