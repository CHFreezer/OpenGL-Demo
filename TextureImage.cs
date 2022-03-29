﻿using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace OpenGL_Demo
{
    public class TextureImage : Texture
    {
        static Configuration customConfig;

        static TextureImage()
        {
            customConfig = Configuration.Default.Clone();
            customConfig.PreferContiguousImageBuffers = true;
        }

        public TextureImage(string path)
        {
            _handle = Program.GL.GenTexture();
            Bind();

            using (var img = Image.Load<Rgba32>(customConfig, path))
            {
                img.Mutate(x => x.Flip(FlipMode.Vertical));

                if (!img.DangerousTryGetSinglePixelMemory(out var memory))
                {
                    throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
                }

                Load(memory.Span, (uint)img.Width, (uint)img.Height);
            }
            Setup();
        }

        public TextureImage(byte[] buffer)
        {
            _handle = Program.GL.GenTexture();
            Bind();

            using (var img = Image.Load<Rgba32>(customConfig, buffer))
            {
                img.Mutate(x => x.Flip(FlipMode.Vertical));

                if (!img.DangerousTryGetSinglePixelMemory(out var memory))
                {
                    throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
                }

                Load(memory.Span, (uint)img.Width, (uint)img.Height);
            }
            Setup();
        }

        public TextureImage(Image<Rgba32> img)
        {
            _handle = Program.GL.GenTexture();
            Bind();

            if (!img.DangerousTryGetSinglePixelMemory(out var memory))
            {
                throw new Exception("This can only happen with multi-GB images or when PreferContiguousImageBuffers is not set to true.");
            }

            Load(memory.Span, (uint)img.Width, (uint)img.Height);

            Setup();
        }

        public TextureImage(Span<byte> data, uint width, uint height)
        {
            _handle = Program.GL.GenTexture();
            Bind();
            Load(data, width, height);
            Setup();
        }

        unsafe void Load(Span<byte> data, uint width, uint height)
        {
            fixed (void* d = &data[0])
            {
                Program.GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
            }
        }

        unsafe void Load(Span<Rgba32> data, uint width, uint height)
        {
            fixed (void* d = &data[0])
            {
                Program.GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
            }
        }

        void Setup()
        {
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            Program.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            Program.GL.TexParameter(TextureTarget.Texture2D, GLEnum.TextureMaxAnisotropy, maxAnisotropy);
            Program.GL.GenerateMipmap(TextureTarget.Texture2D);
            Program.GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}