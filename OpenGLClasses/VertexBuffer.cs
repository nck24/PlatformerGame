using System;
using OpenTK.Graphics.OpenGL4;

namespace GameTest
{
    public class VertexBuffer
    {
        public static readonly int minVertexCount = 1;
        public static readonly int maxVertexCount = 100000;

        public readonly int vertexBufferHandle;
        public readonly int vertexCount;
        public readonly VertexInfo vertexInfo;
        public readonly bool isStatic;

        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true){
            if (vertexCount < VertexBuffer.minVertexCount || vertexCount > VertexBuffer.maxVertexCount){
                throw new ArgumentOutOfRangeException(nameof(vertexCount));
            }

            this.vertexInfo = vertexInfo;
            this.vertexCount = vertexCount;
            this.isStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StaticDraw;
            if (!this.isStatic){
                hint = BufferUsageHint.StreamDraw;
            }

            int vertexSizeInBytes = VertexPositionColor.vertexInfo.sizeInBytes;

            this.vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexCount * vertexInfo.sizeInBytes, IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetData<T>(T[] data, int count) where T : struct{
            if (typeof(T) != vertexInfo.type){
                throw new ArgumentException("Generic type T does not match the vertex type of the buffer");
            }
            if (data is null){
                throw new ArgumentNullException(nameof(data));
            }
            if (data.Length <= 0){
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            if (vertexCount < VertexBuffer.minVertexCount || vertexCount > VertexBuffer.maxVertexCount){
                throw new ArgumentOutOfRangeException(nameof(vertexCount));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, count * this.vertexInfo.sizeInBytes, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}