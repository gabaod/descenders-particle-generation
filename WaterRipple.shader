Shader "Custom/Particles/WaterRipple"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.7,0.85,1.0,0.6)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _RingThickness ("Ring Thickness", Range(0.01,0.3)) = 0.08
        _RingCount ("Number of Rings", Range(1,5)) = 3
        _RingSpeed ("Ring Speed", Range(0.5,5.0)) = 2.0
        _RingFade ("Ring Fade", Range(0.0,2.0)) = 1.2
        _Distortion ("Distortion Amount", Range(0.0,0.5)) = 0.1
        _ReflectionStrength ("Reflection Strength", Range(0.0,1.0)) = 0.5
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
                float _RingThickness;
                float _RingCount;
                float _RingSpeed;
                float _RingFade;
                float _Distortion;
                float _ReflectionStrength;

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
                    float particleAge : TEXCOORD1;
                    UNITY_FOG_COORDS(2)
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                    
                    // Calculate particle age (0 to 1 over lifetime)
                    // Using vertex color alpha as a simple approximation
                    o.particleAge = 1.0 - v.color.a;
                    
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    // Center point
                    float2 center = i.texcoord - 0.5;
                    float dist = length(center) * 2.0;
                    
                    // Time-based expansion
                    float time = _Time.y * _RingSpeed;
                    
                    // Create multiple expanding rings
                    float rings = 0.0;
                    float ringAlpha = 0.0;
                    
                    for (int ringNum = 0; ringNum < _RingCount; ringNum++)
                    {
                        // Offset each ring's start time
                        float ringOffset = float(ringNum) / _RingCount;
                        float ringTime = time - ringOffset * 0.5;
                        
                        // Expanding ring radius
                        float ringRadius = frac(ringTime) * 2.0;
                        
                        // Distance from current ring
                        float ringDist = abs(dist - ringRadius);
                        
                        // Create ring with thickness
                        float ring = 1.0 - smoothstep(0.0, _RingThickness, ringDist);
                        
                        // Fade out as ring expands
                        float fade = 1.0 - frac(ringTime);
                        fade = pow(fade, _RingFade);
                        
                        // Add to total
                        rings += ring * fade;
                        ringAlpha = max(ringAlpha, ring * fade);
                    }
                    
                    // Clamp rings
                    rings = saturate(rings);
                    ringAlpha = saturate(ringAlpha);
                    
                    // Add subtle distortion (water surface wobble)
                    float2 distortedUV = i.texcoord;
                    distortedUV += float2(
                        sin(dist * 10.0 - time * 2.0) * _Distortion * rings,
                        cos(dist * 10.0 - time * 2.0) * _Distortion * rings
                    ) * 0.1;
                    
                    // Center highlight (initial splash point)
                    float centerSplash = 1.0 - smoothstep(0.0, 0.2, dist);
                    centerSplash *= exp(-time * 2.0); // Quick fade
                    
                    // Reflection/highlight on ring edges
                    float reflection = rings * _ReflectionStrength;
                    
                    // Add caustic-like pattern
                    float caustic = sin(dist * 15.0 - time * 3.0) * 
                                   sin(dist * 12.0 - time * 2.5);
                    caustic = pow(saturate(caustic), 5.0) * 0.2 * rings;
                    
                    // Sample texture
                    fixed4 tex = tex2D(_MainTex, distortedUV);
                    
                    // Combine everything
                    fixed4 col = i.color * _TintColor * tex;
                    
                    // Add ring brightness
                    col.rgb += rings * 0.3;
                    
                    // Add center splash
                    col.rgb += centerSplash * 0.5;
                    
                    // Add reflection
                    col.rgb += reflection;
                    
                    // Add caustics
                    col.rgb += caustic;
                    
                    // Alpha from rings
                    col.a *= ringAlpha + centerSplash * 0.5;
                    
                    // Fade entire effect over particle lifetime
                    col.a *= i.color.a;
                    
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
