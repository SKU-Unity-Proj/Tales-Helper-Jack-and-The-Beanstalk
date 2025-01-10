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
                Blend SrcAlpha OneMinusSrcAlpha // ���� ���� �߰�

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                TEXTURE2D(_BaseMap);                 // �ؽ�ó ����
                SAMPLER(sampler_BaseMap);            // ���÷� ����
                float4 _FillColor;                   // ä��� ����
                float _FillProgress;                 // ä��� ���൵

                struct Attributes
                {
                    float4 positionOS : POSITION;    // ������Ʈ ���� ��ǥ
                    float2 uv : TEXCOORD0;           // UV ��ǥ
                };

                struct Varyings
                {
                    float4 positionHCS : SV_POSITION; // Ŭ�� ���� ��ġ
                    float2 uv : TEXCOORD0;           // UV ��ǥ
                };

                Varyings vert(Attributes v)
                {
                    Varyings o;
                    o.positionHCS = TransformObjectToHClip(v.positionOS);
                    o.uv = v.uv; // UV ��ǥ ����
                    return o;
                }

                half4 frag(Varyings i) : SV_Target
                {
                    // �ؽ�ó ���ø�
                    float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                    float2 center = float2(0.5, 0.5); // ���� �߽�
                    float distanceToCenter = distance(i.uv, center);

                    // ���� ä�� Ȯ��
                    if (baseColor.a == 0)
                    {
                        discard; // ���� ���� ����
                    }

                    // �߽ɺ��� ä������ ����
                    if (distanceToCenter < _FillProgress)
                    {
                        return lerp(baseColor, _FillColor, 1.0); // �ؽ�ó�� ä��� ���� ȥ��
                    }
                    else
                    {
                        return baseColor; // ���� �ؽ�ó
                    }
                }
                ENDHLSL
            }
        }
            FallBack "Unlit"
}
