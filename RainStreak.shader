Shader "Custom/Particles/RainStreak"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.7,0.8,1.0,0.6)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        _StreakLength ("Streak Length", Range(0.5,5.0)) = 2.0
        _StreakWidth ("Streak Width", Range(0.01,0.3)) = 0.08
        _Transparency ("Transparency", Range(0.0,1.0)) = 0.5
        _GlossAmount ("Gloss", Range(0.0,2.0)) = 0.8
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
                float _StreakLength;
                float _StreakWidth;
                float _Transparency;
                float _GlossAmount;

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
                    UNITY_FOG_COORDS(1)
                    #ifdef SOFTPARTICLES_ON
                    float4 projPos : TEXCOORD2;
                    #endif
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
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

                    // Create vertical rain streak
                    float2 uv = i.texcoord;
                    
                    // Center point
                    float2 center = uv - float2(0.5, 0.5);
                    
                    // Create elongated vertical streak shape
                    float verticalDist = abs(center.x) / _StreakWidth;
                    float horizontalDist = abs(center.y) / _StreakLength;
                    
                    // Combine to create streak
                    float streak = 1.0 - saturate(verticalDist);
                    streak *= 1.0 - saturate(horizontalDist);
                    
                    // Taper the ends (more realistic rain drop shape)
                    float topTaper = smoothstep(-0.5, -0.3, center.y);
                    float bottomTaper = smoothstep(0.5, 0.4, center.y);
                    streak *= topTaper * bottomTaper;
                    
                    // Add slight bulge in middle (water surface tension)
                    float bulgeFactor = 1.0 - abs(center.y) * 2.0;
                    bulgeFactor = saturate(bulgeFactor);
                    float bulge = 1.0 + bulgeFactor * 0.3;
                    streak *= bulge;
                    
                    // Add highlight along length (wet surface catching light)
                    float highlight = pow(1.0 - abs(center.x / _StreakWidth), 3.0);
                    highlight *= (1.0 - abs(center.y / _StreakLength));
                    highlight *= _GlossAmount;
                    
                    // Sample texture
                    fixed4 tex = tex2D(_MainTex, uv);
                    
                    // Combine
                    fixed4 col = i.color * _TintColor * tex;
                    col.rgb += highlight * 0.3;
                    col.a *= streak * _Transparency;
                    
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
