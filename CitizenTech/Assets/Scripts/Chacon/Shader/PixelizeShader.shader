Shader "Custom/Pixelize"
{
    Properties
    {
        _PixelHeight ("Pixel Screen Height", Float) = 144
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _PixelHeight;

            half4 frag(Varyings IN) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 blockCount = float2(round(_PixelHeight * aspect), _PixelHeight);
                float2 blockPos = floor(IN.texcoord * blockCount);
                float2 blockCenter = (blockPos + 0.5) / blockCount;
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, blockCenter);
            }
            ENDHLSL
        }
    }
}