Shader "Custom/Particles/Sparkle"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _SparkleSpeed ("Sparkle Speed", Range(0.0,10.0)) = 3.0
        _SparkleIntensity ("Sparkle Intensity", Range(0.0,5.0)) = 2.0
        _StarPoints ("Star Points", Range(4,12)) = 8
    }

    Category
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha One
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
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;
                float _SparkleSpeed;
                float _SparkleIntensity;
                float _StarPoints;

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
                    float2 uv = i.texcoord - 0.5;
                    float angle = atan2(uv.y, uv.x);
                    float dist = length(uv);
                    
                    // Create star shape
                    float star = cos(angle * _StarPoints) * 0.5 + 0.5;
                    star = pow(star, 2.0);
                    
                    // Create pulsing effect
                    float pulse = sin(_Time.y * _SparkleSpeed + dist * 5.0) * 0.5 + 0.5;
                    
                    // Combine star and distance falloff
                    float shape = (1.0 - dist * 2.0) * star;
                    shape = saturate(shape);
                    
                    // Add bright center
                    float center = 1.0 - smoothstep(0.0, 0.2, dist);
                    
                    // Sample texture
                    fixed4 tex = tex2D(_MainTex, i.texcoord);
                    
                    // Combine everything
                    fixed4 col = i.color * _TintColor * tex;
                    col.rgb *= (shape + center * 2.0) * _SparkleIntensity * pulse;
                    col.a *= shape;
                    
                    UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
                    return col;
                }
                ENDCG
            }
        }
    }
}
