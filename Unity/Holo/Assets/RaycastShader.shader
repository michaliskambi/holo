﻿Shader "RaycastShader"
{
    Properties
    {
        _MainTex ("Texture", 3D) = "white" {}
	    _LeftEye("LeftEye", Vector) = (0,0,0,0)
		_RightEye("RightEye", Vector) = (0,0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uvw : TEXCOORD0;
            };

            struct v3f
            {
                float3 uvw : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler3D _MainTex;
			float4 _LeftEye;
			float4 _RightEye;
			float4 _MainTex_ST;

            v3f vert (appdata v) {
                v3f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvw = v.vertex.xyz;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v3f i) : SV_Target{
				const int iter = 50;
				float3 acc = 0;
				float3 origin = i.uvw;
				float3 dir = normalize(origin - (unity_StereoEyeIndex ? _RightEye.xyz : _LeftEye.xyz));
				float z_div = max(abs(dir.z), 0.1) * 25.0;
				dir /= z_div;
				float opacity = 1.0;
				for (int it = 0; it < iter; ++it) {
					acc += 0.3 * tex3D(_MainTex, origin + float3(0.5, 0.5, 0.5)).rgb;

//					acc += 0.01 * v * opacity;
					//acc *= (1.0 - opacity);
//					opacity *= 1.0 - (0.01 * v);
					origin += dir;
					if (any(origin > float3(0.5, 0.5, 0.5))) break;
					if (any(origin < float3(-0.5, -0.5, -0.5))) break;
				//	if (opacity < 0.01) break;
				}

//				float v = acc;
				fixed4 col = fixed4(acc, 1.0);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
