Shader "Custom/CompositeBlend"
{
    Properties
    {
        _MainTex ("SceneTex", 2D) = "white" {}
        _Overlay ("Overlay (walls)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;    // scene
            sampler2D _Overlay;    // pixelated walls

            float4 frag(v2f_img i) : SV_Target
            {
                float4 sceneCol = tex2D(_MainTex, i.uv);
                float4 overlay = tex2D(_Overlay, i.uv);

                // alpha composite: overlay over scene using overlay alpha
                float a = saturate(overlay.a);
                float3 outCol = lerp(sceneCol.rgb, overlay.rgb, a);
                return float4(outCol, 1.0);
            }
            ENDCG
        }
    }
    FallBack Off
}
