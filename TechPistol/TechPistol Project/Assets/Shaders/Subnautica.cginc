#include "UnityStandardCore.cginc"

float _Shininess;
float _Fresnel;
float _SpecInt;
float _MarmoSpecEnum;
float _EnableGlow;

float _GlowStrength;
float _GlowStrengthNight;
float _EmissionLM;
float _EmissionLMNight;

sampler2D _SIGMap;
sampler2D _SpecTex;
sampler2D _Illum;

half3 ProxyEmission(float2 uv)
{
#ifndef _EMISSION
   return 0;
#else
   return tex2D(_Illum, uv).rgb * _EmissionColor.rgb;
 #endif
}

half4 SpecularProxyGloss(float2 uv)
{
    half4 sg;
    half4 sig = tex2D(_SIGMap, uv);
    sg.rgb = _SpecColor.rgb * sig.r * _SpecInt;
    sg.a = sig.b * (_Shininess * 0.1);
    return sg;
}

inline FragmentCommonData SpecularProxySetup (float4 i_tex)
{
    half4 specGloss = SpecularProxyGloss(i_tex.xy);
    half3 specColor = specGloss.rgb;
    half smoothness = specGloss.a;

    half oneMinusReflectivity;
    half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

#ifdef UNITY_SETUP_BRDF_INPUT
#undef UNITY_SETUP_BRDF_INPUT
#define UNITY_SETUP_BRDF_INPUT SpecularProxySetup
#endif