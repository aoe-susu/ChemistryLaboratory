Shader "Unity Shaders Book/Chapter 10/Glass Refraction"
{
	Properties
	{
		_MainTex("Main Tex",2D) = "white"{}
		_BumpMap("Normal Map",2D) = "bump"{}
		_Cubemap("Environment Cubemap",Cube) = "_Skybox"{}
		//控制模拟折射时图像的扭曲程度
		_Distortion("Distortion",Range(0,100)) = 10
			//控制折射程度，0时只包含反射效果，1时只包含折射效果
			_RefractAmount("Refract Amount",Range(0.0,1.0)) = 1.0
	}
		SubShader
		{
			//
			//渲染队列必须是Transparent,确保该物体渲染时其他不透明物体都已经被渲染到屏幕上了
			Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }

			//通过关键字“GrabPass”定义抓取屏幕的pass，定义字符串决定了图像会被存入哪个纹理中
			GrabPass{"_RefractionTex"}

			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }

				CGPROGRAM
			//确保光源变量可以被正确赋值
			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag


			//包含引用的内置文件  
			 #include "Lighting.cginc"  
			 #include "UnityCG.cginc"

			//声明properties中定义的属性  
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			samplerCUBE _Cubemap;
			float _Distortion;
			fixed _RefractAmount;
			sampler2D _RefractionTex;
			float4 _RefractionTex_TexelSize;

		struct a2v
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float4 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 scrPos : TEXCOORD0;
		float4 uv :  TEXCOORD1;
		float4 TtoW0 :  TEXCOORD2;
		float4 TtoW1 :  TEXCOORD3;
		float4 TtoW2 :  TEXCOORD4;
	};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//使用ComputeGrabScreenPos(UnityCG.cginc)得到对应被抓取的屏幕图像的采样坐标
				o.scrPos = ComputeGrabScreenPos(o.pos);

				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

				//计算从切线空间到世界空间的变换矩阵
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				//将该矩阵的每一行存储在TtoW0、TtoW1、TtoW2的xyz分量中，w轴被用于存储世界空间下的顶点坐标
				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//通过TtW0等值的w分量得到世界坐标
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				//得到该片元对应的视角方向
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				//对法线纹理进行采样，得到切线空间下法线方向
				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				//对屏幕图像采样的坐标进行偏移，模拟折射效果，使用切线空间的法线可以反应顶点局部空间下的法线方向
				float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize.xy;
				//对scrPos透视除法得到真正的屏幕坐标
				i.scrPos.xy = offset + i.scrPos.xy;
				//使用该坐标对抓取的屏幕图像进行采样，得到模拟的折射颜色
				fixed3 refrCol = tex2D(_RefractionTex, i.scrPos.xy / i.scrPos.w).rgb;
				//把法线方向从切线空间转换到世界空间下
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
				//得到视角方向相对于法线方向的反射方向
				fixed3 reflDir = reflect(-worldViewDir, bump);
				//对cubemap采样，并与主纹理颜色相乘后得到反射颜色
				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				fixed3 reflCol = texCUBE(_Cubemap, reflDir).rgb * texColor.rgb;
				//混合反射与折射颜色，并输出
				fixed3 finalColor = reflCol * (1 - _RefractAmount) + refrCol * _RefractAmount;
				return fixed4(finalColor, 1);


			}
			ENDCG
		}
		}
			Fallback"Diffuse"
}