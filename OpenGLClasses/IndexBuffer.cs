using System;
using OpenTK.Graphics.OpenGL4;

namespace GameTest
{
    public sealed class IndexBuffer
    {
        public readonly int minIndexCount = 1;
        public readonly int maxIndexCount = 250000;

        public int indexBufferHandle;
        public readonly int indexCount;
        public readonly bool isStatic;

        public IndexBuffer(int indexCount, bool isStatic = true){

            if (indexCount < minIndexCount || indexCount > maxIndexCount){
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            }

            this.indexCount =indexCount;
            this.isStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StaticDraw;
            if (!isStatic){
                hint = BufferUsageHint.StreamDraw;
            }

            this.indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.indexCount * sizeof(int), IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void SetData(int[] data, int count){
            if (data is null){
                throw new ArgumentNullException();
            }
            if (data.Length <= 0){
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            if (count < this.minIndexCount || count > this.maxIndexCount){
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}