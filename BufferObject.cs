using Silk.NET.OpenGL;
using System;

namespace OpenGL_Demo
{
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        uint _handle;
        BufferTargetARB _bufferType;

        public BufferObject(BufferTargetARB bufferType)
        {
            _bufferType = bufferType;
            _handle = Program.GL.GenBuffer();
        }

        public void Bind()
        {
            Program.GL.BindBuffer(_bufferType, _handle);
        }

        public unsafe void BufferData(Span<TDataType> data, BufferUsageARB bufferUsage = BufferUsageARB.StaticDraw)
        {
            fixed (void* d = &data[0])
            {
                Program.GL.BufferData(_bufferType, (nuint)(data.Length * sizeof(TDataType)), d, bufferUsage);
            }
        }

        public unsafe void Dispose()
        {
            Program.GL.DeleteBuffer(_handle);
        }
    }
}