// #region License
// /*
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright © 2009 The MonoGame Team
// 
// All rights reserved.
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
// U.S. copyright law.
// 
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// */
// #endregion License
// 
using System;
using System.IO;

namespace Microsoft.Xna.Framework.Graphics
{
	internal static class DxtUtil
	{
		internal static byte[] DecompressDxt1(byte[] imageData, int width, int height)
        {
            using (MemoryStream imageStream = new MemoryStream(imageData))
                return DecompressDxt1(imageStream, width, height);
        }

        internal static byte[] DecompressDxt1(Stream imageStream, int width, int height)
        {
            byte[] imageData = new byte[width * height * 4];

            using (BinaryReader imageReader = new BinaryReader(imageStream))
            {
                int blockCountX = (width + 3) / 4;
                int blockCountY = (height + 3) / 4;
				
				byte[] colorBuffer = new byte[4];
				byte[] color0 =  new byte[3];
				byte[] color1 =  new byte[3];
                
                for (int y = 0; y < blockCountY; y++)
                {
                    for (int x = 0; x < blockCountX; x++)
                    {
						DecompressDxt1Block(imageReader, x, y, blockCountX, imageData,
						                    colorBuffer, color0, color1);
					}
                }
            }

            return imageData;
        }

        private static void DecompressDxt1Block(BinaryReader imageReader, int x, int y, int width, byte[] imageData,
		                                        byte[] colorBuffer, byte[] color0, byte[] color1)
        {
            ushort c0 = imageReader.ReadUInt16();
            ushort c1 = imageReader.ReadUInt16();

            ConvertRgb565ToRgb888(color0, c0);
            ConvertRgb565ToRgb888(color1, c1);

            uint lookupTable = imageReader.ReadUInt32();
			
            for (int blockY = 0; blockY < 4; blockY++)
            {
                for (int blockX = 0; blockX < 4; blockX++)
                {
                    uint index = (lookupTable >> 2 * (4 * blockY + blockX)) & 0x03;
                    
                    if (c0 > c1)
                    {
                        switch (index)
                        {
                            case 0: GetRgba(colorBuffer, color0[0], color0[1], color0[2]); break;
                            case 1: GetRgba(colorBuffer, color1[0], color1[1], color1[2]); break;
                            case 2: GetRgba(colorBuffer, (byte)((2 * color0[0] + color1[0]) / 3), (byte)((2 * color0[1] + color1[1]) / 3), (byte)((2 * color0[2] + color1[2]) / 3)); break;
                            case 3: GetRgba(colorBuffer, (byte)((color0[0] + 2 * color1[0]) / 3), (byte)((color0[1] + 2 * color1[1]) / 3), (byte)((color0[2] + 2 * color1[2]) / 3)); break;
                        }
                    }
                    else
                    {
                        switch (index)
                        {
                            case 0: GetRgba(colorBuffer, color0[0], color0[1], color0[2]); break;
                            case 1: GetRgba(colorBuffer, color1[0], color1[1], color1[2]); break;
                            case 2: GetRgba(colorBuffer, (byte)((color0[0] + color1[0]) / 2), (byte)((color0[1] + color1[1]) / 2), (byte)((color0[2] + color1[2]) / 2)); break;
                            case 3: GetRgba(colorBuffer, 0, 0, 0); break;
                        }
                    }
					
					imageData[y * width * 64 + blockY * width * 16 + x * 16 + blockX * 4] = colorBuffer[0];
					imageData[y * width * 64 + blockY * width * 16 + x * 16 + blockX * 4 + 1] = colorBuffer[1];
					imageData[y * width * 64 + blockY * width * 16 + x * 16 + blockX * 4 + 2] = colorBuffer[2];
					imageData[y * width * 64 + blockY * width * 16 + x * 16 + blockX * 4 + 3] = colorBuffer[3];
                }
			}
        }
        
        internal static byte[] DecompressDxt3(byte[] imageData, int width, int height)
        {
            using (MemoryStream imageStream = new MemoryStream(imageData))
                return DecompressDxt3(imageStream, width, height);
        }

        internal static byte[] DecompressDxt3(Stream imageStream, int width, int height)
        {
            byte[] imageData = new byte[width * height * 4];

            using (BinaryReader imageReader = new BinaryReader(imageStream))
            {
                int blockCountX = (width + 3) / 4;
                int blockCountY = (height + 3) / 4;
				
				byte[] colorBuffer = new byte[4];
				byte[] alphaBuffer = new byte[2];
				byte[] color0 =  new byte[3];
				byte[] color1 =  new byte[3];

                for (int y = 0; y < blockCountY; y++)
                {
                    for (int x = 0; x < blockCountX; x++)
                    {
                        DecompressDxt3Block(imageReader, x, y, blockCountX, imageData, 
						                    colorBuffer, alphaBuffer, color0, color1);
					}
                }
            }

            return imageData;
        }

        private static void DecompressDxt3Block(BinaryReader imageReader, int x, int y, int width, byte[] imageData, 
		                                        byte[] colorBuffer, byte[] alphaBuffer, byte[] color0, byte[] color1)
        {
            byte[] alpha = imageReader.ReadBytes(8);
            
            ushort c0 = imageReader.ReadUInt16();
            ushort c1 = imageReader.ReadUInt16();

            ConvertRgb565ToRgb888(color0, c0);
            ConvertRgb565ToRgb888(color1, c1);

            uint lookupTable = imageReader.ReadUInt32();
			
            for (int blockY = 0; blockY < 4; blockY++)
            {
                for (int blockX = 0; blockX < 4; blockX++)
                {
					int blockOffset = 4 * blockY + blockX;
					Convert8BitTo4Bit(alphaBuffer, alpha[blockOffset / 2]);
					
					uint index = (lookupTable >> 2 * blockOffset) & 0x03;
                    switch (index)
                    {
                        case 0: GetRgba(colorBuffer, color0[0], color0[1], color0[2], alphaBuffer[blockOffset % 2]); break;
                        case 1: GetRgba(colorBuffer, color1[0], color1[1], color1[2], alphaBuffer[blockOffset % 2]); break;
                        case 2: GetRgba(colorBuffer, (byte)((2 * color0[0] + color1[0]) / 3), (byte)((2 * color0[1] + color1[1]) / 3), (byte)((2 * color0[2] + color1[2]) / 3), alphaBuffer[blockOffset % 2]); break;
                        case 3: GetRgba(colorBuffer, (byte)((color0[0] + 2 * color1[0]) / 3), (byte)((color0[1] + 2 * color1[1]) / 3), (byte)((color0[2] + 2 * color1[2]) / 3), alphaBuffer[blockOffset % 2]); break;
                    }
					
					int imageDataIndex = y * width * 64 + blockY * width * 16 + x * 16 + blockX * 4;
					imageData[imageDataIndex] = colorBuffer[0];
					imageData[imageDataIndex + 1] = colorBuffer[1];
					imageData[imageDataIndex + 2] = colorBuffer[2];
					imageData[imageDataIndex + 3] = colorBuffer[3];
                }
            }
        }

        private static void ConvertRgb565ToRgb888(byte[] buffer, ushort color)
        {
            int temp = (color >> 11) * 255 + 16;
            buffer[0] = (byte)((temp / 32 + temp) / 32);
            temp = ((color & 0x07E0) >> 5) * 255 + 32;
            buffer[1] = (byte)((temp / 64 + temp) / 64);
            temp = (color & 0x001F) * 255 + 16;
            buffer[2] = (byte)((temp / 32 + temp) / 32);
        }

        private static byte[] Convert8BitTo4Bit(byte[] buffer, byte value)
        {
            byte low = (byte)(value & 0x0F);
            byte high = (byte)(value & 0xF0);

            buffer[0] = (byte)(low | (low << 4));
            buffer[1] = (byte)(high | (high >> 4));

            return buffer;
        }

        private static void GetRgba(byte[] buffer, byte r, byte g, byte b)
        {
			buffer[0] = r;
			buffer[1] = g;
			buffer[2] = b;
			buffer[3] = 255;
        }

        private static void GetRgba(byte[] buffer, byte r, byte g, byte b, byte a)
        {
            buffer[0] = r;
			buffer[1] = g;
			buffer[2] = b;
			buffer[3] = a;
        }
	}
}

