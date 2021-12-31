Shader "Custom/Wave" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Speed ("Speed", Float) = 2
		_Amplitude ("Amplitude", Float) = 1
	}
	SubShader {
		Pass {
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			float _Speed, _Amplitude;

			uniform sampler2D _MainTex;	

			struct vertIn {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertOut {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			vertOut vert(vertIn v) {
				float k = v.vertex.x + _Time.y * _Speed;
				float4 displacement = float4(0.0f, sin(k) * _Amplitude, 0.0f, 0.0f); 
				v.vertex += displacement;

				vertOut o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag(vertOut v) : SV_Target {
				fixed4 col = tex2D(_MainTex, v.uv);
				return col;
			}
			ENDCG
		}
	}
}
