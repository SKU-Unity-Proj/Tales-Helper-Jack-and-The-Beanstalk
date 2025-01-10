Shader "Custom/URP_FillingSymbol_Sprite"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _FillColor("Fill Color", Color) = (1,1,1,1)
        _FillProgress("Fill Progress", Range(0,1)) = 0
    }
        SubShader
        {
            Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Transparent" }
            LOD 100

            Pass
            {
                Name "UniversalForward"
                Tags { "LightMode" = "UniversalForward" }
                Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 추가

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                TEXTURE2D(_BaseMap);                 // 텍스처 선언
                SAMPLER(sampler_BaseMap);            // 샘플러 선언
                float4 _FillColor;                   // 채우기 색상
                float _FillProgress;                 // 채우기 진행도

                struct Attributes
                {
                    float4 positionOS : POSITION;    // 오브젝트 공간 좌표
                    float2 uv : TEXCOORD0;           // UV 좌표
                };

                struct Varyings
                {
                    float4 positionHCS : SV_POSITION; // 클립 공간 위치
                    float2 uv : TEXCOORD0;           // UV 좌표
                };

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    o.positionHCS = TransformObjectToHClip(v.positionOS);
                    o.uv = v.uv; // UV 좌표 전달
                    return o;
                }

                half4 frag(Varyings i) : SV_Target
                {
                    // 텍스처 샘플링
                    float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                    float2 center = float2(0.5, 0.5); // 문양 중심
                    float distanceToCenter = distance(i.uv, center);

                    // 알파 채널 확인
                    if (baseColor.a == 0)
                    {
                        discard; // 투명 영역 무시
                    }

                    // 중심부터 채워지는 로직
                    if (distanceToCenter < _FillProgress)
                    {
                        return lerp(baseColor, _FillColor, 1.0); // 텍스처와 채우기 색상 혼합
                    }
                    else
                    {
                        return baseColor; // 원본 텍스처
                    }
                }
                ENDHLSL
            }
        }
            FallBack "Unlit"
}
