 Shader "Custom/AlphaMask"
 {
 Properties {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
     _AlphaMap ("Additional Alpha Map (Greyscale)", 2D) = "white" {}
 }
  
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 200

     Pass
     {
     	ZWrite On
     	ColorMask 0
     }

 CGPROGRAM
 #pragma surface surf Lambert alpha
  
 sampler2D _MainTex;
 sampler2D _AlphaMap;
 float4 _Color;
  
 struct Input {
     float2 uv_MainTex;
 };
  
 void surf (Input IN, inout SurfaceOutput o) {
     half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
     o.Emission = c.rgb;
     o.Alpha = c.a * tex2D(_AlphaMap, IN.uv_MainTex).r;
 }
 ENDCG
 }
  
 Fallback "Transparent/VertexLit"
 }