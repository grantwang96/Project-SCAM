Shader "Custom/HealthBar" {

	Properties {
		_RimColor ("Rim color", Color) = (0.5, 0.1, 0.1)
		_RimPower ("Rim power", Range(0.5, 8.0)) = 3.0
	}

	SubShader {

		ZWrite Off

		CGPROGRAM
		#pragma surface surf Lambert

		struct Input {
			float3 viewDir;
		};

		float4 _RimColor;
		float _RimPower;

		void surf(Input IN, inout SurfaceOutput o) {
			half rim = 1-saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}


		ENDCG

	}

	FallBack "Diffuse"
}
