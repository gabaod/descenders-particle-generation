Shader "Custom/Particles/WaterMist"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.8,0.95,1.0,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        _MistDensity ("Mist Density", Range(0.0,2.0)) = 1.0
        _Shimmer ("Shimmer Amount", Range(0.0,1.0)) = 0.3
        _ShimmerSpeed ("Shimmer Speed", Range(0.0,5.0)) = 2.0
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
                float _MistDensity;
                float _Shimmer;
                float _ShimmerSpeed;

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

                // Noise function for organic mist
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

                    // Soft circular mist
                    float2 center = i.texcoord - 0.5;
                    float dist = length(center);
                    
                    // Layered noise for organic look
                    float2 noiseUV = i.texcoord * 3.0 + _Time.y * 0.1;
                    float n1 = noise(noiseUV);
                    float n2 = noise(noiseUV * 2.0 + float2(0.5, 0.5));
                    float mistPattern = (n1 * 0.6 + n2 * 0.4);
                    
                    // Shimmer effect (light catching on water droplets)
                    float shimmer = sin(i.texcoord.x * 10.0 + _Time.y * _ShimmerSpeed) * 
                                   sin(i.texcoord.y * 10.0 + _Time.y * _ShimmerSpeed * 1.3);
                    shimmer = pow(saturate(shimmer), 5.0) * _Shimmer;
                    
                    // Soft edge
                    float edgeFalloff = 1.0 - smoothstep(0.3, 0.5, dist);
                    
                    // Combine mist pattern with circular falloff
                    float mist = mistPattern * edgeFalloff * _MistDensity;
                    
                    // Sample texture
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    
                    // Combine
                    fixed4 col = i.color * _TintColor * tex;
                    col.rgb += shimmer;
                    col.a *= mist * edgeFalloff;
                    
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
