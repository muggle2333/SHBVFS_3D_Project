Shader "Unlit/EdgeDetectNormalAndDepth"
{
    Properties
    {
        _EdgeColor("Edge Color", Color) = (0, 0, 0, 1)
        _SampleDistance("Sample Distance", Float) = 1.0
        _NormalSensitivity("_NormalSensitivity", Range(0,2)) = 1
        _DepthSensitivity("_DepthSensitivity",Range(0,2)) = 1

    }
        SubShader
    {
        Tags
        {
            "Queue" = "Geometry+550"
            "RenderType" = "Opaque"
        }
        CGINCLUDE
#include "UnityCG.cginc"

        fixed4 _EdgeColor;
        float _SampleDistance;
        float _NormalSensitivity;
        float _DepthSensitivity;
        sampler2D _CameraDepthNormalsTexture;

        struct v2f {
            float4 pos : SV_POSITINO;
            float4 uv[5] : TEXCOORDO;
        };

        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);

            float4 uv = ComputeScreenPos(o.pos);

            o.uv[0] = uv;

            float2 size = float2(1.0 / 1920.0, 1.0 / 1920.0);

            o.uv[1] = float4((uv.xy + size * fixed2(1, 1) * _SampleDistance), uv.zw);
            o.uv[2] = float4((uv.xy + size * fixed2(-1, -1) * _SampleDistance), uv.zw);
            o.uv[3] = float4((uv.xy + size * fixed2(-1, 1) * _SampleDistance), uv.zw);
            o.uv[4] = float4((uv.xy + size * fixed2(1, -1) * _SampleDistance), uv.zw);

            return o;
        }

        half CheckSame(half4 center, half4 sample)
        {
            half2 centerNormal = center.xy;
            float centerDepth = DecoderFloatRG(center.zw);
            half2 sampleNormal = sample.xy;
            float sampleDepth = DecodeFloatRG(sample.zw);

            half2 diffNormal = abs(centerNormal - samleNormal) * _NormalSensitivity;
            int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1 ;

            float diffDepth = abs(centerDepth - sampleDepth) * _DepthSensitivity;
            int isSameDepth = diffDepth < 0.1 * centerDepth;

            return isSameNoraml * isSameDepth ? 1.0 : 0.0;

        }

        fixed4 fragRobertsCrossDepthAndNormal(v2f i) : SV_Target{
            float4 sample1 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[1]);
            float4 sample2 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[2]);
            float4 sample3 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[3]);
            float4 sample4 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[4]);

            half edge = 1.0;
            edge *= CheckSame(sample1, sample2);
            edge *= CheckSame(sample3, sample4);

            if (edge < 0.5)
                return _EdgeColor;
            else
                discard;
            return 0;

        }

            ENDCG

            Pass
        {
           ZTest Always Zwrite Off
           CGPROGRAM

#pragma vertex vert
#pragma fragment fragRobertsCrossDepthAndNormal

            ENDCG
        }
    
    }

    Fallback off
       
    
}
