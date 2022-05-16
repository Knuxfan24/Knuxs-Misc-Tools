#define DXT1_BITMAP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if DXT1_BITMAP
using System.Drawing;
using System.Runtime.InteropServices;
#endif

namespace DXT1Decompressor
{
    public class DXT1Decompressor
    {
        struct Color
        {
            public Color(float r = 0, float g = 0, float b = 0)
            {
                this.r = r; this.g = g; this.b = b;
            }
            float r, g, b;
            
            // R and B components are swapped
            public byte R
            {
                get => (byte)(b * 255);
            }
            public byte G
            {
                get => (byte)(g * 255);
            }
            public byte B
            {
                get => (byte)(r * 255);
            }
        }
        
        class Block
        {
            public Color[] pixels = new Color[16];
        }
        
        public DXT1Decompressor(int width, int height, byte[] blob)
        {
            int bytes = width * 3 * 4 * height;
            m_rgb_values = new byte[bytes];
            m_width = width;
            m_height = height;
            
            Parallel.For(0, height / 4, by =>
                         Parallel.For(0, width / 4, bx =>
                                      {
                                      var block = DecompressBlock(bx, by, width, blob);
                                      var block_off = by * 4 * width * 3 + bx * 4 * 3;
                                      for (int py = 0; py < 4; py++)
                                      {
                                      var line_off = block_off + py * width * 3;
                                      for (int px = 0; px < 4; px++)
                                      {
                                      var pix_off = line_off + px * 3;
                                      m_rgb_values[pix_off + 0] = block.pixels[py * 4 + px].R;
                                      m_rgb_values[pix_off + 1] = block.pixels[py * 4 + px].G;
                                      m_rgb_values[pix_off + 2] = block.pixels[py * 4 + px].B;
                                      }
                                      }
                                      }
                                      )
                         );
        }
        
        Block DecompressBlock(int x, int y, int width, byte[] blob)
        {
            Block ret = new Block();
            
            int off = (y * width / 4 + x) * (64 / 8);
            
            UInt16 c0, c1;
            int c0_r, c0_g, c0_b;
            int c1_r, c1_g, c1_b;
            int c2_r, c2_g, c2_b;
            int c3_r, c3_g, c3_b;
            UInt16 c0_lo = blob[off + 0];
            UInt16 c0_hi = blob[off + 1];
            UInt16 c1_lo = blob[off + 2];
            UInt16 c1_hi = blob[off + 3];
            c0 = (UInt16)((c0_hi << 8) | c0_lo);
            c1 = (UInt16)((c1_hi << 8) | c1_lo);
            
            c0_r = (c0 & 0b1111100000000000) >> 11;
            c0_g = (c0 & 0b0000011111100000) >> 5;
            c0_b = c0 & 0b0000000000011111;
            c1_r = (c1 & 0b1111100000000000) >> 11;
            c1_g = (c1 & 0b0000011111100000) >> 5;
            c1_b = c1 & 0b0000000000011111;
            
            if (c0 >= c1)
            {
                c2_r = (int)((2.0f * c0_r + c1_r) / 3.0f);
                c2_g = (int)((2.0f * c0_g + c1_g) / 3.0f);
                c2_b = (int)((2.0f * c0_b + c1_b) / 3.0f);
                c3_r = (int)((c0_r + 2.0f * c1_r) / 3.0f);
                c3_g = (int)((c0_g + 2.0f * c1_g) / 3.0f);
                c3_b = (int)((c0_b + 2.0f * c1_b) / 3.0f);
            }
            else
            {
                c2_r = (int)((c0_r + c1_r) / 2.0f);
                c2_g = (int)((c0_g + c1_g) / 2.0f);
                c2_b = (int)((c0_b + c1_b) / 2.0f);
                c3_r = c3_g = c3_b = 0;
            }
            
            Color[] lookup = new Color[]
            {
                new Color(c0_r / 32.0f, c0_g / 64.0f, c0_b / 32.0f),
                new Color(c1_r / 32.0f, c1_g / 64.0f, c1_b / 32.0f),
                new Color(c2_r / 32.0f, c2_g / 64.0f, c2_b / 32.0f),
                new Color(c3_r / 32.0f, c3_g / 64.0f, c3_b / 32.0f),
            };
            
            for (int by = 0; by < 4; by++)
            {
                for (int bx = 0; bx < 4; bx++)
                {
                    var code = (blob[off + by] << (bx * 2)) & 3;
                    ret.pixels[by * 4 + bx] = lookup[code];
                }
            }
            
            return ret;
        }

        public byte[] Data
        {
            get => m_rgb_values;
            private set { }
        }
        
        private byte[] m_rgb_values;
        private int m_width, m_height;
        
#if DXT1_BITMAP
        public Bitmap ToBitmap()
        {
            var ret = new Bitmap(m_width, m_height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var data = ret.LockBits(new Rectangle(0, 0, m_width, m_height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var ptr = data.Scan0;
            var bytes = Math.Abs(data.Stride) * ret.Height;
            Marshal.Copy(m_rgb_values, 0, ptr, bytes);
            ret.UnlockBits(data);
            return ret;
        }
#endif
    }
}
