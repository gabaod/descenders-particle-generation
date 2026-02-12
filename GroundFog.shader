Shader "Custom/Particles/GroundFog"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.9,0.95,1.0,0.15)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        _FogDensity ("Fog Density", Range(0.0,2.0)) = 0.8
        _EdgeSoftness ("Edge Softness", Range(0.0,1.0)) = 0.9
        _WetReflection ("Wet Reflection", Range(0.0,1.0)) = 0.3
        _VerticalFade ("Vertical Fade", Range(0.0,2.0)) = 1.0
    }

    Category
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_particles
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;
                float _FogDensity;
                float _EdgeSoftness;
                float _WetReflection;
                float _VerticalFade;

                #ifdef SOFTPARTICLES_ON
                sampler2D_float _CameraDepthTexture;
                float _InvFade;
                #endif

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    UNITY_FOG_COORDS(2)
                    #ifdef SOFTPARTICLES_ON
                    float4 projPos : TEXCOORD3;
                    #endif
                };

                float4 _MainTex_ST;

                // Multi-octave noise for organic fog
                float hash(float2 p)
                {
                    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
                }

                float noise(float2 p)
                {
                    float2 i = floor(p);
                    float2 f = frac(p);
                    f = f * f * (3.0 - 2.0 * f);
                    
                    float a = hash(i);
                    float b = hash(i + float2(1.0, 0.0));
                    float c = hash(i + float2(0.0, 1.0));
                    float d = hash(i + float2(1.0, 1.0));
                    
                    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
                }

                // Layered noise for realistic fog
                float layeredNoise(float2 p)
                {
                    float n = 0.0;
                    n += noise(p) * 0.5;
                    n += noise(p * 2.0) * 0.25;
                    n += noise(p * 4.0) * 0.125;
                    n += noise(p * 8.0) * 0.0625;
                    return n / 0.9375;
                }

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    #ifdef SOFTPARTICLES_ON
                    o.projPos = ComputeScreenPos (o.vertex);
                    COMPUTE_EYEDEPTH(o.projPos.z);
                    #endif
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    #ifdef SOFTPARTICLES_ON
                    float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    float partZ = i.projPos.z;
                    float fade = saturate (_InvFade * (sceneZ-partZ));
                    i.color.a *= fade;
                    #endif

                    // Create very soft, blended fog
                    float2 center = i.texcoord - 0.5;
                    float dist = length(center);
                    
                    // Extremely soft radial falloff for blending
                    float radialFade = 1.0 - smoothstep(0.0, 0.5 + _EdgeSoftness, dist);
                    
                    // Multi-layered noise for organic fog pattern
                    float2 fogUV = i.texcoord * 2.0 + _Time.y * 0.03;
                    float fogPattern = layeredNoise(fogUV);
                    
                    // Add slow movement variation
                    float2 flowUV = i.texcoord + float2(_Time.y * 0.02, _Time.y * 0.015);
                    float flow = noise(flowUV * 3.0);
                    fogPattern = lerp(fogPattern, flow, 0.3);
                    
                    // Vertical gradient fade (thinner at top)
                    float verticalGrad = smoothstep(0.0, 0.5, i.texcoord.y);
                    verticalGrad = pow(verticalGrad, _VerticalFade);
                    
                    // Combine fog layers
                    float fogMask = radialFade * fogPattern;
                    fogMask *= (1.0 - verticalGrad * 0.5); // Denser at bottom
                    
                    // Wet reflection effect (subtle highlights)
                    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                    float wetHighlight = pow(saturate(dot(float3(0, 1, 0), viewDir)), 8.0);
                    wetHighlight *= fogMask * _WetReflection;
                    
                    // Very subtle shimmer (wet ground catching light)
                    float shimmer = sin(i.worldPos.x * 3.0 + _Time.y) * 
                                   sin(i.worldPos.z * 3.0 + _Time.y * 1.3);
                    shimmer = pow(saturate(shimmer), 10.0) * 0.1 * fogMask;
                    
                    // Sample texture
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    
                    // Combine everything
                    fixed4 col = i.color * _TintColor * tex;
                    col.rgb += wetHighlight + shimmer;
                    col.a *= fogMask * _FogDensity;
                    
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
