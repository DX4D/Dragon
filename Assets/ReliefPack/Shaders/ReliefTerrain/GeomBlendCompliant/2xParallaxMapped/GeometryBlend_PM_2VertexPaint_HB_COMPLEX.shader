// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//
// Relief Terrain  - vertex color blend - 2 Parallax mapped material with height blending
// Tomasz Stobierski 2013-2016
//
// (we're using R vertex channel to blend between 1st and 2nd material)
// (we're using G vertex channel to darken texture)
// (we're using B vertex channel to put stronger gloss)
//
// To speed-up shader we're using combined textures.
// _BumpMapCombined is texture made like RTP terrain shader do. You can make it in RTP inspector and save or:
// 1. open Window/4 to 1 texture channel mixer
// 2. target texture channels are:
//     - R = A from 1st bumpmap
//     - G = G from 1st bumpmap
//     - B = A from 2st bumpmap
//     - A = G from 2st bumpmap
// 3. combined heightmap uses RG channels - you can make it using tool described above
//
// distances have the same meaning like in RTP inspector
//
// we're using position (x,z), size (x,z) and tiling of underlying terrain to get right coords of global perlin map and global colormap
// 
Shader "Relief Pack - GeometryBlend/2 Layers/HeightBlend COMPLEX" {
    Properties {
		_Color ("Color A (RGB)", Color) = (0.5, 0.5, 0.5, 1)	
		_MainTex("Texture A (A chan. - smoothness)", 2D) = "white" {}
		_GlossMin("Gloss Min A", Range(0.0, 1.0)) = 0
		_GlossMax("Gloss Max A", Range(0.0, 1.0)) = 1
		_Metalness("Metalness A", Range(0,1)) = 0

		_Color2 ("Color B (RGB)", Color) = (0.5, 0.5, 0.5, 1)	
		_MainTex2 ("Texture B (A chan. - smoothness)", 2D) = "white" {}
		_GlossMin2("Gloss Min B", Range(0.0, 1.0)) = 0
		_GlossMax2("Gloss Max B", Range(0.0, 1.0)) = 1
		_Metalness2("Metalness B", Range(0,1)) = 0

		_BumpMapCombined ("Bumpmap A+B (RGBA)", 2D) = "bump" {}
		_HeightMapCombined ("Heightmap A+B (RG)", 2D) = "black" {}
		_ExtrudeHeight("Extrude Height", Range(0.001,0.1)) = 0.04
		
		_uvBlendScale ("UV blend scale", Float) = 0.2
		_uvBlendAmount ("UV blend amount", Range(0,1)) = 0.5
		_uvBlendSaturation ("UV blend saturation", Range(0,1)) = 0.5
		
		_VerticalTexture ("Vertical texture", 2D) = "grey" {}
		_VerticalTextureTiling ("Vertical texture tiling", Float) = 30
		_VerticalTextureStrengthA ("Vertical texture amount A", Range(0,1)) = 0.5
		_VerticalTextureStrengthB ("Vertical texture amount B", Range(0,1)) = 0.5
		_VerticalTextureGlobalBumpInfluence ("Vertical texture perlin distorion", Range(0,0.1)) = 0.01
		
		_BumpMapPerlin ("Perlin global (special combined RG)", 2D) = "black" {}
		_BumpMapPerlinBlendA ("Perlin normal A", Range(0,2)) = 0.3
		_BumpMapPerlinBlendB ("Perlin normal B", Range(0,2)) = 0.3
		_BumpMapGlobalScale ("Tiling scale", Range( 0.01,0.25) ) = 0.1
		_TERRAIN_distance_start ("Near distance start", Float) = 2
		_TERRAIN_distance_transition ("Near fade length", Float) = 20
		_TERRAIN_distance_start_bumpglobal ("Far distance start", Float) = 22
		_TERRAIN_distance_transition_bumpglobal ("Far fade length", Float) = 40
		rtp_perlin_start_val ("Perlin start val", Range(0,1)) = 0.3
		
		_ColorMapGlobal ("Global colormap", 2D) = "black" {}
		// uncomment when used w/o ReliefTerrain script component (for example with VoxelTerrains)
		//_GlobalColorMapBlendValues ("Global colormap blending (XYZ)", Vector) = (0.3,0.6,0.8,0)
		//_GlobalColorMapSaturation ("Global colormap saturation", Range(0,1)) = 0.8
		
		_TERRAIN_PosSize ("Terrain pos (xz to XY) & size(xz to ZW)", Vector) = (0,0,1000,1000)
		_TERRAIN_Tiling ("Terrain tiling (XY) & offset(ZW)", Vector) = (3,3,0,0)
		
		_TERRAIN_HeightMap ("Terrain HeightMap (combined)", 2D) = "white" {}
		_TERRAIN_Control ("Terrain splat controlMap", 2D) = "white" {}
		
		// caustics
		TERRAIN_CausticsTex ("Caustics", 2D) = "black" {}
		TERRAIN_CausticsColor ("Caustics color", Color) = (1,1,1,1)
		TERRAIN_CausticsAnimSpeed ("Caustics anim speed", Float) = 1
		TERRAIN_CausticsTilingScale ("Caustics tiling", Float) = 1
		TERRAIN_CausticsWaterLevel ("Caustics water level", Float) = 0
		TERRAIN_CausticsWaterDeepFadeLength ("Caustics deep fade", Float) = 15
		TERRAIN_CausticsWaterShallowFadeLength ("Caustics shallow fade", Float) = 4
		TERRAIN_WetnessDark("Wetness darkening (vertex color G)", Range(0,1)) = 0.7
		TERRAIN_WetnessGloss("Wetness glossiness (vertex color B)", Range(0,1)) = 1
    }
    SubShader {
	Tags {
		"Queue"="Geometry+12"
		"RenderType" = "Opaque"
	}
	
	Offset -1,-1
	ZTest LEqual	
	CGPROGRAM
	#pragma surface surf Standard vertex:vert decal:blend
	#pragma exclude_renderers d3d11_9x gles
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	#pragma target 3.0
	#pragma glsl
	
	// edges can be heightblended
	#define BLENDING_HEIGHT
	
	// we'll use global colormap
	#define COLOR_MAP
	#define RTP_COLOR_MAP_BLEND_MULTIPLY
	// we'll use global perlin
	#define GLOBAL_PERLIN
	// we'll show only global colormap at far distance (no detail)
	//#define SIMPLE_FAR	
	
	// uv blend feature like in terrain, here we're using 1 layer for blending purposes
	#define UV_BLEND
	// blend is realised by color multiplication
	#define UV_BLEND_MULTIPLY
	// unlike in terrain we're blending normals, too
	#define UV_BLEND_NORMALS
	
	// vertical texture 
	#define VERTICAL_TEXTURE
	
	// caustics
	#define RTP_CAUSTICS
	
	float4 _MainTex_ST;
	struct Input {
		float2 tc_MainTex;
		float4 aux;
		float2 globalUV;
		float3 viewDir;
		
		float4 color:COLOR;
	};
	
	half3 _Color, _Color2;
	float _GlossMin, _GlossMax, _Metalness;
	float _GlossMin2, _GlossMax2, _Metalness2;
	float _ExtrudeHeight;
	
	sampler2D _MainTex;
	sampler2D _MainTex2;
	sampler2D _BumpMapCombined;
	sampler2D _HeightMapCombined;
	
	float _uvBlendScale;
	float _uvBlendAmount;
	float _uvBlendSaturation;
	
	sampler2D _VerticalTexture;
	float _VerticalTextureTiling;
	float _VerticalTextureStrengthA;
	float _VerticalTextureStrengthB;
	float _VerticalTextureGlobalBumpInfluence;
	
	sampler2D _ColorMapGlobal;
	sampler2D _BumpMapPerlin;
	float _BumpMapGlobalScale;
	float _BumpMapPerlinBlendA;
	float _BumpMapPerlinBlendB;
	
	float rtp_perlin_start_val;	
	float _TERRAIN_distance_start;
	float _TERRAIN_distance_transition;
	float _TERRAIN_distance_start_bumpglobal;
	float _TERRAIN_distance_transition_bumpglobal;
	
	float4 _TERRAIN_PosSize;
	float4 _TERRAIN_Tiling;
	
	// set globaly by ReliefTerrain script
	float4 _GlobalColorMapBlendValues;
	float _GlobalColorMapSaturation;
	
	sampler2D _TERRAIN_HeightMap;
	sampler2D _TERRAIN_Control;
	
	// caustics
	float TERRAIN_CausticsAnimSpeed;
	half4 TERRAIN_CausticsColor;
	float TERRAIN_CausticsTilingScale;
	sampler2D TERRAIN_CausticsTex;
	float TERRAIN_CausticsWaterLevel;
	float TERRAIN_CausticsWaterDeepFadeLength;
	float TERRAIN_CausticsWaterShallowFadeLength;
	float TERRAIN_WetnessDark;
	float TERRAIN_WetnessGloss;
	float TERRAIN_WetnessSpecularity;	
	
	void vert (inout appdata_full v, out Input o) {
	    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
			UNITY_INITIALIZE_OUTPUT(Input, o);
		#endif
		
		o.tc_MainTex.xy=TRANSFORM_TEX(v.texcoord, _MainTex);
		
		float3 Wpos=mul(unity_ObjectToWorld, v.vertex).xyz;
		o.aux.xy=Wpos.xz-_TERRAIN_PosSize.xy+_TERRAIN_Tiling.zw;
		o.aux.xy/=_TERRAIN_Tiling.xy;
		o.aux.z=length(_WorldSpaceCameraPos.xyz-Wpos);
		
		o.aux.w = Wpos.y;
		
		o.globalUV=Wpos.xz-_TERRAIN_PosSize.xy;
		o.globalUV/=_TERRAIN_PosSize.zw;
		
		//float far=saturate((o._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		//v.normal.xyz=lerp(v.normal.xyz, float3(0,1,0), far*_FarNormalDamp);
	}
	
	void surf (Input IN, inout SurfaceOutputStandard o) {
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN)
		float _uv_Relief_z=saturate((IN.aux.z - _TERRAIN_distance_start) / _TERRAIN_distance_transition);
		_uv_Relief_z=1-_uv_Relief_z;
		float _uv_Relief_w=saturate((IN.aux.z - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		#endif
		
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN)		
		float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.globalUV);
		#ifdef SIMPLE_FAR	
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*1.6+0.3),_uv_Relief_w));
		#else
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
		#endif
		#endif

      	float2 tH=tex2D(_HeightMapCombined, IN.tc_MainTex).rg;
		#if !defined(RTP_SIMPLE_SHADING) 
	      	float2 uv=IN.tc_MainTex + ParallaxOffset(tH.x, _ExtrudeHeight, IN.viewDir.xyz)*(1-IN.color.a);
      		float2 uv2=IN.tc_MainTex + ParallaxOffset(tH.y, _ExtrudeHeight, IN.viewDir.xyz)*(1-IN.color.a);
      	#endif
      	float2 control=float2(IN.color.r, 1-IN.color.r);
      	control*=(tH+0.01);
      	float2 control_orig=control;
      	control*=control;
      	control*=control;
      	control*=control;
      	control/=dot(control, 1);
		#if !defined(RTP_SIMPLE_SHADING) 
	      	float2 uvMixed=lerp(uv, uv2, control.y);
		#else
	      	float2 uvMixed=IN.tc_MainTex;
		#endif

      	fixed4 col=tex2D(_MainTex, uvMixed);
		col.rgb *= _Color * 2;
      	fixed4 col2=tex2D(_MainTex2, uvMixed);
		col2.rgb *= _Color2 * 2;
      	o.Albedo=control.x*col.rgb + control.y*col2.rgb;
      	#ifdef UV_BLEND
			#ifdef UV_BLEND_MULTIPLY
				half3 colBlend=tex2D(_MainTex, uvMixed*_uvBlendScale).rgb;
				colBlend=lerp( (dot(colBlend, 0.3333)).xxx, colBlend, _uvBlendSaturation);
	      		o.Albedo=lerp(o.Albedo, o.Albedo*colBlend*2, _uvBlendAmount);
			#else
	      		o.Albedo=lerp(o.Albedo, tex2D(_MainTex, uvMixed*_uvBlendScale).rgb, _uvBlendAmount);
			#endif      	
      	#endif
      	
		#ifdef SIMPLE_FAR      	
		o.Albedo=lerp(o.Albedo, global_color_value.rgb, _uv_Relief_w);
		#endif      	
		col.a = lerp(_GlossMin, _GlossMax, col.a);
		col2.a = lerp(_GlossMin2, _GlossMax2, col2.a);
		o.Smoothness = control.x*col.a + control.y*col2.a;
		o.Metallic = control.x*_Metalness + control.y*_Metalness2;
      	
      	#ifdef COLOR_MAP
		#ifdef RTP_COLOR_MAP_BLEND_MULTIPLY
			o.Albedo=lerp(o.Albedo, o.Albedo*global_color_value.rgb*2, global_color_blend);
		#else
			o.Albedo=lerp(o.Albedo, global_color_value.rgb, global_color_blend);
		#endif      	
		#endif	
      	
		float3 n;
		float4 normals_combined = tex2D(_BumpMapCombined,uvMixed).rgba*control.xxyy;
		n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
		n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
		#ifdef UV_BLEND_NORMALS
			float3 n2;
			normals_combined.rg = tex2D(_BumpMapCombined,uvMixed*_uvBlendScale).rg;
			n2.xy=normals_combined.rg*2-1;
			n2.z = sqrt(1 - saturate(dot(n2.xy, n2.xy)));
			o.Normal=lerp(n, n2, _uvBlendAmount);
		#else
			o.Normal=n;
		#endif			
      	#ifdef GLOBAL_PERLIN
	      	//float perlinmask=tex2D(_BumpMapGlobal, IN.aux.xy/8).r;
			//float4 global_bump_val=tex2D(_BumpMapPerlin, IN.aux.xy*_BumpMapGlobalScale);
			float4 global_bump_val=tex2D(_BumpMapPerlin, IN.tc_MainTex*_BumpMapGlobalScale);
	
			float3 norm_far;
			norm_far.xy = global_bump_val.rg*2-1;
			norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));      	
			
			o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*dot(control, float2(_BumpMapPerlinBlendA, _BumpMapPerlinBlendB));	
			o.Normal=normalize(o.Normal);
		#endif	
      	
		#ifdef VERTICAL_TEXTURE
			float2 vert_tex_uv=float2(0, IN.aux.w/_VerticalTextureTiling);
			#ifdef GLOBAL_PERLIN
				vert_tex_uv += _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
			#endif
			half3 vert_tex=tex2D(_VerticalTexture, vert_tex_uv).rgb;
			o.Albedo=lerp(o.Albedo, o.Albedo*vert_tex*2, dot(control, float2(_VerticalTextureStrengthA, _VerticalTextureStrengthB)) );
		#endif
      	
		#if defined(BLENDING_HEIGHT)
			float4 terrain_coverage=tex2D(_TERRAIN_Control, IN.globalUV);
			float4 splat_control1=terrain_coverage * tex2D(_TERRAIN_HeightMap, IN.aux.xy) * IN.color.a;
			float4 splat_control2=float4(control_orig, 0, 0) * (1-IN.color.a);
			
			float blend_coverage=dot(terrain_coverage, 1);
			if (blend_coverage>0.1) {
			
				splat_control1*=splat_control1;
				splat_control1*=splat_control1;
				splat_control2*=splat_control2;
				splat_control2*=splat_control2;
				
				float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1);
				splat_control1 /= normalize_sum;
				splat_control2 /= normalize_sum;		
				
				o.Alpha=dot(splat_control2,1);
				o.Alpha=lerp(1-IN.color.a, o.Alpha, saturate((blend_coverage-0.1)*4) );
			} else {
				o.Alpha=1-IN.color.a;
			}
		#else
			o.Alpha=1-IN.color.a;
		#endif
		
		o.Albedo*=1-IN.color.g*TERRAIN_WetnessDark;
		o.Smoothness = lerp(o.Smoothness, TERRAIN_WetnessGloss, IN.color.b);

		#ifdef RTP_CAUSTICS
		{
			float damp_fct_caustics=saturate((IN.aux.w-TERRAIN_CausticsWaterLevel+TERRAIN_CausticsWaterDeepFadeLength)/TERRAIN_CausticsWaterDeepFadeLength);
			float overwater=saturate(-(IN.aux.w-TERRAIN_CausticsWaterLevel-TERRAIN_CausticsWaterShallowFadeLength)/TERRAIN_CausticsWaterShallowFadeLength);
			damp_fct_caustics*=overwater;		
			
			float tim=_Time.x*TERRAIN_CausticsAnimSpeed;
			uvMixed+=IN.aux.xy-IN.tc_MainTex; // topplanar uv+parallax offset
			uvMixed*=TERRAIN_CausticsTilingScale;
			float3 _Emission=tex2D(TERRAIN_CausticsTex, uvMixed+float2(tim, tim) ).rgb;
			_Emission+=tex2D(TERRAIN_CausticsTex, uvMixed+float2(-tim, -tim*0.873) ).rgb;
			_Emission+=tex2D(TERRAIN_CausticsTex, uvMixed*1.1+float2(tim, 0) ).rgb;
			_Emission+=tex2D(TERRAIN_CausticsTex, uvMixed*0.5+float2(0, tim*0.83) ).rgb;
			_Emission=saturate(_Emission-1.6);
			_Emission*=_Emission;
			_Emission*=_Emission;
			_Emission*=damp_fct_caustics;
			_Emission*=TERRAIN_CausticsColor.rgb*3;
			o.Emission+=_Emission;
		} 
		#endif		
		
		#if defined(UNITY_PASS_PREPASSFINAL)
			o.Smoothness*=o.Alpha;
		#endif			
	}
	ENDCG
      
    } 
	//FallBack "Diffuse"
}
