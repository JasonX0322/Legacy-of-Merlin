// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Card"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_Front("Front", 2D) = "white" {}
		_Back("Back", 2D) = "white" {}
		_Outline("Outline", 2D) = "white" {}
		_OutlineColor("OutlineColor", Color) = (0.9907571,1,0,1)
		_OutlineAlpha("OutlineAlpha", Range( 0 , 1)) = 0
		_BurnRate("BurnRate", Range( -0.01 , 1)) = -0.01
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _OutlineColor;
			uniform sampler2D _Outline;
			uniform float4 _Outline_ST;
			uniform float _OutlineAlpha;
			uniform sampler2D _Front;
			uniform float4 _Front_ST;
			uniform sampler2D _Back;
			uniform float _BurnRate;
			//https://www.shadertoy.com/view/XdXGW8
			float2 GradientNoiseDir( float2 x )
			{
				const float2 k = float2( 0.3183099, 0.3678794 );
				x = x * k + k.yx;
				return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
			}
			
			float GradientNoise( float2 UV, float Scale )
			{
				float2 p = UV * Scale;
				float2 i = floor( p );
				float2 f = frac( p );
				float2 u = f * f * ( 3.0 - 2.0 * f );
				return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
						lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
						dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
			}
			

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN , half ase_vface : VFACE ) : SV_Target
			{
				float2 uv_Outline = IN.texcoord.xy * _Outline_ST.xy + _Outline_ST.zw;
				float4 tex2DNode22 = tex2D( _Outline, uv_Outline );
				float4 outline33 = ( _OutlineColor * tex2DNode22 * tex2DNode22.a * _OutlineAlpha );
				float2 uv_Front = IN.texcoord.xy * _Front_ST.xy + _Front_ST.zw;
				float2 uv09 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult11 = (float2(( 1.0 - uv09.x ) , uv09.y));
				float4 switchResult7 = (((ase_vface>0)?(tex2D( _Front, uv_Front )):(tex2D( _Back, appendResult11 ))));
				float4 cardImg30 = switchResult7;
				float2 uv051 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv066 = IN.texcoord.xy * float2( 15,15 ) + float2( 0,0 );
				float gradientNoise67 = GradientNoise(uv066,1.0);
				gradientNoise67 = gradientNoise67*0.5 + 0.5;
				float temp_output_68_0 = ( ( uv051.y + 0.0 ) * gradientNoise67 );
				float temp_output_58_0 = step( ( _BurnRate + 0.01 ) , temp_output_68_0 );
				float BurnAlpha72 = temp_output_58_0;
				float4 color65 = IsGammaSpace() ? float4(1,0.06774927,0,1) : float4(1,0.005725843,0,1);
				float4 BurnEdge69 = ( ( step( _BurnRate , temp_output_68_0 ) - temp_output_58_0 ) * color65 );
				
				half4 color = ( ( ( outline33 + ( ( 1.0 - outline33.a ) * cardImg30 ) ) * BurnAlpha72 ) + BurnEdge69 );
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
0;0;1920;1139;2993.29;-989.8491;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;29;-3112.26,220.5627;Inherit;False;1443.255;800.7515;卡面;7;10;9;30;7;6;8;11;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;32;-3089.556,-496.773;Inherit;False;1109.3;568.6996;边框;5;33;22;25;23;24;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-3081.26,795.4197;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;-3039.556,-250.0736;Inherit;True;Property;_Outline;Outline;2;0;Create;True;0;0;False;0;-1;cac139a2ff183c44e80b9dcf8e34e01f;cac139a2ff183c44e80b9dcf8e34e01f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;10;-2842.203,738.5118;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-3016.657,-30.07374;Inherit;False;Property;_OutlineAlpha;OutlineAlpha;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-2692.457,-446.7729;Inherit;False;Property;_OutlineColor;OutlineColor;3;0;Create;True;0;0;False;0;0.9907571,1,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;70;-3129.511,1182.112;Inherit;False;2285.784;844.7324;燃烧;14;55;58;59;63;65;51;52;68;67;66;57;56;69;72;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-2442.257,-167.5739;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-2675.064,819.0929;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-3079.511,1773.228;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;15,15;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-3060.253,1366.973;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-2490.136,791.3141;Inherit;True;Property;_Back;Back;1;0;Create;True;0;0;False;0;-1;f7f3dd2d5f212c8429a97a383b46b82f;f7f3dd2d5f212c8429a97a383b46b82f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-2188.964,-177.1524;Inherit;False;outline;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-2487.781,270.5626;Inherit;True;Property;_Front;Front;0;0;Create;True;0;0;False;0;-1;d3a3638067ec6f64297857b9fd72a256;219131eb52979c543bdff02e38212bd3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;-2470.147,1232.112;Inherit;False;Property;_BurnRate;BurnRate;5;0;Create;True;0;0;False;0;-0.01;0.2823105;-0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-2820.365,1411.477;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;67;-2761.355,1767.845;Inherit;True;Gradient;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-2512.433,1507.036;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;7;-2146.943,516.3196;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-2254.111,1706.514;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-552.1008,-158.4782;Inherit;False;33;outline;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;55;-2044.417,1398.7;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;58;-2042.111,1628.514;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-1887.845,513.9315;Inherit;False;cardImg;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;35;-338.101,-20.87823;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;65;-1639.643,1244.884;Inherit;False;Constant;_Color1;Color 1;6;0;Create;True;0;0;False;0;1,0.06774927,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;27;-56.23402,49.18841;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-109.1775,276.9756;Inherit;False;30;cardImg;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;-1718.111,1435.514;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-1334.643,1393.884;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;104.9659,132.6884;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;36;221.3344,-68.03013;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-1697.752,1753.166;Inherit;False;BurnAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;484.2471,106.6813;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;593.7242,489.4536;Inherit;False;72;BurnAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-1063.728,1387.358;Inherit;False;BurnEdge;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;969.2783,572.3179;Inherit;True;69;BurnEdge;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;867.3933,272.6043;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;1225.615,274.1087;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;14;1473.519,118.2928;Float;False;True;-1;2;ASEMaterialInspector;0;4;Card;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;10;0;9;1
WireConnection;24;0;23;0
WireConnection;24;1;22;0
WireConnection;24;2;22;4
WireConnection;24;3;25;0
WireConnection;11;0;10;0
WireConnection;11;1;9;2
WireConnection;8;1;11;0
WireConnection;33;0;24;0
WireConnection;52;0;51;2
WireConnection;67;0;66;0
WireConnection;68;0;52;0
WireConnection;68;1;67;0
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;57;0;56;0
WireConnection;55;0;56;0
WireConnection;55;1;68;0
WireConnection;58;0;57;0
WireConnection;58;1;68;0
WireConnection;30;0;7;0
WireConnection;35;0;34;0
WireConnection;27;0;35;3
WireConnection;59;0;55;0
WireConnection;59;1;58;0
WireConnection;63;0;59;0
WireConnection;63;1;65;0
WireConnection;28;0;27;0
WireConnection;28;1;31;0
WireConnection;36;0;34;0
WireConnection;72;0;58;0
WireConnection;21;0;36;0
WireConnection;21;1;28;0
WireConnection;69;0;63;0
WireConnection;48;0;21;0
WireConnection;48;1;73;0
WireConnection;49;0;48;0
WireConnection;49;1;71;0
WireConnection;14;0;49;0
ASEEND*/
//CHKSM=136FE5D6249FAFF4F256CD66B2B6007D8A6D2ED5