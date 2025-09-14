Shader "Custom/PixelShader"
{
    Properties
    {
        _MainTex ("MainTex (wall RT)", 2D) = "white" {}
        _PixelSize ("Pixel Size (px)", Float) = 16
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _AlphaThreshold ("Alpha Threshold", Float) = 0.1
        _CheckDiagonals ("Check Diagonals (8-neigh)", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _PixelSize;
            float4 _OutlineColor;
            float _AlphaThreshold;
            float _CheckDiagonals;

            float4 frag(v2f_img i) : SV_Target
            {
                // 1) convert to screen pixels and snap to grid
                float2 screenPixels = i.uv * _ScreenParams.xy;
                float2 snappedPixels = floor(screenPixels / _PixelSize) * _PixelSize;
                float2 snappedUV = snappedPixels / _ScreenParams.xy;

                // 2) sample the snapped pixel (center of block)
                float4 col = tex2D(_MainTex, snappedUV);

                // If this block is transparent, just return it (nothing to outline)
                if (col.a <= _AlphaThreshold)
                    return col;

                // 3) check neighbors (in screen pixels) for emptiness
                // neighbor offsets in pixel-space (N, S, E, W and optionally diagonals)
                int emptyNeighbor = 0;
                float2 px = float2(_PixelSize, _PixelSize);

                // cardinal neighbors
                float2 nUV;

                nUV = (snappedPixels + float2(px.x, 0.0)) / _ScreenParams.xy; // right
                emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                nUV = (snappedPixels + float2(-px.x, 0.0)) / _ScreenParams.xy; // left
                emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                nUV = (snappedPixels + float2(0.0, px.y)) / _ScreenParams.xy; // up
                emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                nUV = (snappedPixels + float2(0.0, -px.y)) / _ScreenParams.xy; // down
                emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                // diagonals
                if (_CheckDiagonals > 0.5)
                {
                    nUV = (snappedPixels + float2(px.x, px.y)) / _ScreenParams.xy; // up-right
                    emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                    nUV = (snappedPixels + float2(-px.x, px.y)) / _ScreenParams.xy; // up-left
                    emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                    nUV = (snappedPixels + float2(px.x, -px.y)) / _ScreenParams.xy; // down-right
                    emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;

                    nUV = (snappedPixels + float2(-px.x, -px.y)) / _ScreenParams.xy; // down-left
                    emptyNeighbor += tex2D(_MainTex, nUV).a <= _AlphaThreshold ? 1 : 0;
                }

                if (emptyNeighbor > 0)
                {
                    // full opaque outline color
                    float4 oc = _OutlineColor;
                    oc.a = 1.0;
                    return oc;
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
