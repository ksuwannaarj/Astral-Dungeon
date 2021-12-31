// Normal map and flow map retrieved from https://catlikecoding.com/unity/tutorials/flow/texture-distortion/

Shader "Custom/Starry night" {
    // Noise Tiling: X 1 Y 30
    // Distortion Tiling: X 1 Y 4
    Properties {
        _Color ("Color", Color) = (0.01324314, 0.06016522, 0.1132075, 1)
        _Noise("Noise", 2D) = "white" {} // For the perlin noise
        _Cutoff("Noise Cutoff", Range(0, 1)) = 0.88 // For how many "stars" (white dots) the screen will show
        _Scroll("Noise Scroll Amount", Vector) = (0, 0.01, 0, 0) // Speed for the horizontal scroll animation to the entire plane
        _Distortion("Distortion", 2D) = "white" {} // Distortion for the surface noise using a normal map with only two channels
        _DistortionAmount("SDistortion Amount", Range(0, 1)) = 0.55 // Strength of the distortion
    }
    SubShader {
        Tags {
            "RenderType" = "transparent"
        }
        Pass {
            Cull Off // Since the water is "transparent" we want to see everything
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" // For using TRANSFORM_TEX() later

            float4 _Color, _Noise_ST, _Distortion_ST;
            float _Cutoff, _DistortionAmount;
            float2 _Scroll;
            sampler2D _Noise, _Distortion;

            struct vertIn {
                float4 vertex: POSITION;
                float4 uv : TEXCOORD0;
            };

            struct vertOut {
                float4 pos : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;
            };

            vertOut vert(vertIn v) {
                vertOut o;

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, worldPos); 
                // Apply the perlin noise here
                o.noiseUV = TRANSFORM_TEX(v.uv, _Noise); 
                // Apply the distortion to the noise
                o.distortUV = TRANSFORM_TEX(v.uv, _Distortion);

                return o;
            }

            float4 frag(vertOut v) : COLOR {
                // Sample the distortion texture while changing the dimensional vectors to a range between -1 and 1
                float2 distortSample = (tex2D(_Distortion, v.distortUV).xy * 2 - 1) * _DistortionAmount;
                float2 noiseUV = float2((v.noiseUV.x + _Time.y * _Scroll.x) + distortSample.x, (v.noiseUV.y + _Time.y * _Scroll.y) + distortSample.y);

                // Sampling the noise texture
                float noiseSample = tex2D(_Noise, noiseUV).r; 
                // Apply the cutoff threshold to limit the brightness
                float noise = noiseSample > _Cutoff ? 1 : 0; 

                // Combine the sky color to the noise
                return _Color + noise;
            }
            ENDCG
        }
    }
}
