using System;

namespace Microsoft.Xna.Framework.Graphics
{
	internal class SpriteBatchItem
	{
		public int TextureID;
		public float Depth;
		public VertexPosition2ColorTexture vertexTL;
		public VertexPosition2ColorTexture vertexTR;
		public VertexPosition2ColorTexture vertexBL;
		public VertexPosition2ColorTexture vertexBR;
		public SpriteBatchItem ()
		{
			vertexTL = new VertexPosition2ColorTexture();
			vertexTR = new VertexPosition2ColorTexture();
			vertexBL = new VertexPosition2ColorTexture();
			vertexBR = new VertexPosition2ColorTexture();
		}
		
		public void Set ( float x, float y, float w, float h, Color color, Vector2 texCoordTL, Vector2 texCoordBR )
		{
			vertexTL.Position = new Vector2(x,y);
			vertexTL.Color = color.GLPackedValue;
			vertexTL.TextureCoordinate = texCoordTL;

			vertexTR.Position = new Vector2(x+w,y);
			vertexTR.Color = color.GLPackedValue;
			vertexTR.TextureCoordinate = new Vector2(texCoordBR.X,texCoordTL.Y);

			vertexBL.Position = new Vector2(x,y+h);
			vertexBL.Color = color.GLPackedValue;
			vertexBL.TextureCoordinate = new Vector2(texCoordTL.X,texCoordBR.Y);

			vertexBR.Position = new Vector2(x+w,y+h);
			vertexBR.Color = color.GLPackedValue;
			vertexBR.TextureCoordinate = texCoordBR;
		}
		public void Set ( float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR )
		{
			float dxcos = dx * cos;
			float dxsin = dx * sin;
			
			float dysin = dy * sin;
			float dycos = dy * cos;
			
			float dxwcos = (dx + w) * cos;
			float dxwsin = (dx + w) * sin;
			
			float dyhsin = (dy + h) * sin;
			float dyhcos = (dy + h) * cos;
				
			vertexTL.Position.X = x + dxcos - dysin;
			vertexTL.Position.Y = y + dxsin + dycos;
			vertexTL.Color = color.GLPackedValue;
			vertexTL.TextureCoordinate = texCoordTL;

			vertexTR.Position.X = x + dxwcos - dysin;
			vertexTR.Position.Y = y + dxwsin + dycos;
			vertexTR.Color = color.GLPackedValue;
			vertexTR.TextureCoordinate.X = texCoordBR.X;
			vertexTR.TextureCoordinate.Y = texCoordTL.Y;

			vertexBL.Position.X = x + dxcos - dyhsin;
			vertexBL.Position.Y = y + dxsin + dyhcos;
			vertexBL.Color = color.GLPackedValue;
			vertexBL.TextureCoordinate.X = texCoordTL.X;
			vertexBL.TextureCoordinate.Y = texCoordBR.Y;

			vertexBR.Position.X = x + dxwcos - dyhsin;
			vertexBR.Position.Y = y + dxwsin + dyhcos;
			vertexBR.Color = color.GLPackedValue;
			vertexBR.TextureCoordinate = texCoordBR;
		}
	}
}

