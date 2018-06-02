Shader "VertexColors"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white"{}//color por defecto, base RGB
	}
	
	SubShader
	{
		Pass
		{
			Lighting On//Activar iluminacion
			ColorMaterial AmbientAndDiffuse//tipo ambiental y difusa
			SetTexture [_MainTex]
			{
				combine texture * primary DOUBLE
			}
		}
	}
}