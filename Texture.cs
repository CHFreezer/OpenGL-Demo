using Silk.NET.OpenGL;
using System;

namespace OpenGL_Demo
{
    public class Texture : IDisposable
    {
        protected const int maxAnisotropy = 16;

        protected uint _handle;

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            Program.GL.ActiveTexture(textureSlot);
            Program.GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Dispose()
        {
            Program.GL.DeleteTexture(_handle);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Texture t = (Texture)obj;
                return _handle == t._handle;
            }
        }

        public override int GetHashCode()
        {
            return _handle.GetHashCode();
        }
    }
}