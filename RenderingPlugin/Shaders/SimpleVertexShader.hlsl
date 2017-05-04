
cbuffer MyCB : register(b0) 
{
	float4x4 worldMatrix;
}
void VS (float3 pos : POSITION, float4 color : COLOR, out float4 ocolor : COLOR, out float4 opos : SV_Position) 
{
	opos = mul (worldMatrix, float4(pos,1));
	ocolor = color;
}