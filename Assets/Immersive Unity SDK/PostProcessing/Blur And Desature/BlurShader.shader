Shader "Postprocessing/BlurShader"
{
	Properties{
		[HideInInspector]_MainTex("Texture", 2D) = "white" {}
		_BlurSize("Blur Size", Range(0,2)) = 0.05
		[KeywordEnum(Low, Medium, High)] _Samples("Sample amount", Float) = 0
		[PowerSlider(3)]_StandardDeviation("Standard Deviation (Gauss only)", Range(0.00, 0.3)) = 0.005
		_DesaturateRadius("Desaturate Radius", Range(0, 50)) = 12

		_FocusRadius("Focus Radius", Range(0,1)) = 0.5
		_Intensity("Intensity", Range(0, 3)) = 0

		[HideInInspector]_FocusPoint1X("Focus Point 1X", Float) = 0.5
		[HideInInspector]_FocusPoint1Y("Focus Point 1Y", Float) = 0.5
		[HideInInspector]_FocusPoint2X("Focus Point 2X", Float) = 0.5
		[HideInInspector]_FocusPoint2Y("Focus Point 2Y", Float) = 0.5

	}

		SubShader{
			// markers that specify that we don't need culling 
			// or reading/writing to the depth buffer
			Cull Off
			ZWrite Off
			ZTest Always

			//Horizontal Blur
			Pass{
				CGPROGRAM
				//include useful shader functions
				#include "UnityCG.cginc"

				#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
				#pragma shader_feature GAUSS

				//define vertex and fragment shader
				#pragma vertex vert
				#pragma fragment frag

				//texture and transforms of the texture
				sampler2D _MainTex;
				float _BlurSize;
				float _StandardDeviation;
				float _Intensity;

				//Two focus positions are required to account for wrap around display.
				float _FocusPoint1X;
				float _FocusPoint1Y;
				float _FocusPoint2X;
				float _FocusPoint2Y;
				//Points within this radius are have no blur applied.
				float _FocusRadius;


				#define PI 3.14159265359
				#define E 2.71828182846

				#if _SAMPLES_LOW
					#define SAMPLES 10
				#elif _SAMPLES_MEDIUM
					#define SAMPLES 30
				#else
					#define SAMPLES 100
				#endif

				//the object data that's put into the vertex shader
				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				//the data that's used to generate fragments and can be read by the fragment shader
				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//the vertex shader
				v2f vert(appdata v) {
					v2f o;
					//convert the vertex positions from object space to clip space so they can be rendered
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}


				//Calculates the distance to the nearest focus point.
				//To focus points are required for wrap around displays.
				float distanceToFocusCenter(v2f i) {
					float aspectRatio = _ScreenParams.x / _ScreenParams.y;
					float distance;
					float2 centerToPoint1 = float2(i.uv.x * aspectRatio, i.uv.y) - float2(_FocusPoint1X * aspectRatio, _FocusPoint1Y);
					float2 centerToPoint2 = float2(i.uv.x * aspectRatio, i.uv.y) - float2(_FocusPoint2X * aspectRatio, _FocusPoint2Y);

					if (length(centerToPoint1) < length(centerToPoint2)) {
						return length(centerToPoint1);
					}
					else {
						return length(centerToPoint2);
					}
				}

				//the fragment shader
				fixed4 frag(v2f i) : SV_TARGET{
					//failsafe so we can use turn off the blur by setting the deviation to 0
					if (_StandardDeviation == 0 || _Intensity == 0)
						return tex2D(_MainTex, i.uv);

					//init color variable
					float4 col = 0;
					float sum = 0;

					//Calculate Aspect Ratio
					float aspectRatio = _ScreenParams.x / _ScreenParams.y;

					//Calculate blur value based on distance from focus centers
					float distanceValue = (distanceToFocusCenter(i) - _FocusRadius);
					distanceValue = max(0, distanceValue);
					float blurValue = distanceValue * _BlurSize * _Intensity;

					//iterate over blur samples
					for (float index = 0; index < SAMPLES; index++) {
						//get the offset of the sample
						float offset = (index / (SAMPLES - 1) - 0.5) * blurValue * 1.0/aspectRatio;
						//get uv coordinate of sample
						float2 uv = i.uv + float2(offset, 0);
						//calculate the result of the gaussian function
						float stDevSquared = _StandardDeviation * _StandardDeviation;
						float gauss = (1 / sqrt(2 * PI*stDevSquared)) * pow(E, -((offset*offset) / (2 * stDevSquared)));
						//add result to sum
						sum += gauss;
						//multiply color with influence from gaussian function and add it to sum color
						col += tex2D(_MainTex, uv) * gauss;
					}
					//divide the sum of values by the amount of samples
					col = col / sum;
					return col;
				}

				ENDCG
			}

			//Desaturate
			Pass {
				CGPROGRAM

				//include useful shader functions
				#include "UnityCG.cginc"

				#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
				#pragma shader_feature GAUSS

				//define vertex and fragment shader
				#pragma vertex vert
				#pragma fragment frag

				//texture and transforms of the texture
				sampler2D _MainTex;
				float _BlurSize;
				float _DesaturateRadius;
				float _Intensity;

				float _FocusPoint1X;
				float _FocusPoint1Y;
				float _FocusPoint2X;
				float _FocusPoint2Y;
				float _FocusRadius;


				float3 intensityVector = float3(0.3, 0.59, 0.11);


				//the object data that's put into the vertex shader
				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				//the data that's used to generate fragments and can be read by the fragment shader
				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				//the vertex shader
				v2f vert(appdata v) {
					v2f o;
					//convert the vertex positions from object space to clip space so they can be rendered
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				//Calculates the distance to the nearest focus point.
				//To focus points are required for wrap around displays.
				float distanceToFocusCenter(v2f i) {
					float aspectRatio = _ScreenParams.x / _ScreenParams.y;
					float distance;
					float2 centerToPoint1 = float2(i.uv.x * aspectRatio, i.uv.y) - float2(_FocusPoint1X * aspectRatio, _FocusPoint1Y);
					float2 centerToPoint2 = float2(i.uv.x * aspectRatio, i.uv.y) - float2(_FocusPoint2X * aspectRatio, _FocusPoint2Y);

					if (length(centerToPoint1) < length(centerToPoint2)) {
						return length(centerToPoint1);
					}
					else {
						return length(centerToPoint2);
					}
				}

				//the fragment shader
				fixed4 frag(v2f i) : SV_TARGET{
					if (_Intensity == 0)
						return tex2D(_MainTex, i.uv);

					//Get initial colour value
					float4 col = tex2D(_MainTex, i.uv);

					//Calulate how much desaturation this fragment should have
					float desaturationValue = ((distanceToFocusCenter(i) - _FocusRadius) * _BlurSize) * _DesaturateRadius * _Intensity;
					desaturationValue = min(0.9, desaturationValue);
					desaturationValue = max(0, desaturationValue);

					//Calculate the new intensity value
					float intensity = col.x * 0.3 + col.y * 0.59 + col.z * 0.11;
					intensity *= desaturationValue;
					
					//Calculate colour at new intensity value
					col = float4(intensity + col.x*(1 - desaturationValue), intensity + col.y*(1 - desaturationValue), intensity + col.z*(1 - desaturationValue), 1);

					return col;
				}

				ENDCG
			}
	}
}