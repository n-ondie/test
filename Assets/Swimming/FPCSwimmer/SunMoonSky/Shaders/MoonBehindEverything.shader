Shader "SunMoonSky/Moon Behind Everything" 
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _CutOff("Cut off", Range(0,1)) = 0.1
        _Opacity("Opacity", Range(0,1)) = 1
        _EmissionMap("Emission Map", 2D) = "black" {}
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater [_Cutoff]

        Pass
        {
            ZWrite Off
            CULL Off


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform float _CutOff;
            uniform float _Opacity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _EmissionMap;
            float4 _EmissionColor;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                #if defined(UNITY_REVERSED_Z)
                o.vertex.z = 1.0e-9f;
                #else
                o.vertex.z = o.vertex.w - 1.0e-6f;
                #endif

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            { 
                float4 albedo = tex2D(_MainTex, i.uv);
                if (albedo.a < _CutOff) discard;
                else albedo.a = _Opacity;

                half4 output = half4(albedo.rgb/* * lighting.rgb*/, albedo.a);
                half4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor;
                output.rgb += emission.rgb;

                return output;
            }
            ENDCG
        }
    }
}