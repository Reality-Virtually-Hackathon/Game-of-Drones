﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Meta/RGBFeed"
{
    Properties
	{
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
	{
        Pass
		{
            Name "NoLightingOrZ"
            Tags{ "RenderType" = "Opaque" }
            LOD 200
            ZWrite Off

            CGPROGRAM

            #pragma vertex vert 
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _MainTex;

            struct ShaderData
			{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            ShaderData vert(appdata_tan v)
			{
                ShaderData o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag(ShaderData f) : COLOR
			{
                return tex2D(_MainTex, f.uv);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
