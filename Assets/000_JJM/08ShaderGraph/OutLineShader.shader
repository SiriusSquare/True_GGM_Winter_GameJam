Shader "Custom/SpriteOutlineGlowURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1
        _GlowSize ("Glow Size", Float) = 2
        _GlowIntensity ("Glow Intensity", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "UniversalMaterialType"="Unlit"
        }

        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            float4 _OutlineColor;
            float4 _GlowColor;
            float _OutlineSize;
            float _GlowSize;
            float _GlowIntensity;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float SampleAlpha(float2 uv, float size)
            {
                float2 t = _MainTex_TexelSize.xy * size;
                float a = 0;
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( t.x, 0)).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-t.x, 0)).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,  t.y)).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -t.y)).a);
                return a;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float baseAlpha = baseCol.a;

                float outlineA = saturate(SampleAlpha(i.uv, _OutlineSize) - baseAlpha);
                float glowA = saturate(SampleAlpha(i.uv, _GlowSize) - baseAlpha);

                float3 col = baseCol.rgb;
                col += _OutlineColor.rgb * outlineA;
                col += _GlowColor.rgb * glowA * _GlowIntensity;

                float alpha = max(baseAlpha, max(outlineA, glowA));

                return float4(col, alpha);
            }
            ENDHLSL
        }
    }
}
