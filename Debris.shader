Shader "Custom/Particles/Debris"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.4,0.3,1)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Roughness ("Roughness", Range(0.0,1.0)) = 0.8
        _NoiseScale ("Noise Scale", Range(1.0,20.0)) = 5.0
    }

    Category
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite On

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;
                float _Roughness;
                float _NoiseScale;

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
                };

                float4 _MainTex_ST;

                // Simple noise function
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
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    // Generate rough texture
                    float n = noise(i.texcoord * _NoiseScale);
                    n += noise(i.texcoord * _NoiseScale * 2.0) * 0.5;
                    n += noise(i.texcoord * _NoiseScale * 4.0) * 0.25;
                    n /= 1.75;
                    
                    // Sample texture
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    
                    // Apply roughness variation
                    float rough = lerp(0.7, 1.3, n * _Roughness);
                    
                    fixed4 col = i.color * _TintColor * tex;
                    col.rgb *= rough;
                    
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
