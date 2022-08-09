using OpenTK.Graphics.OpenGL4;

namespace GameTest
{
    public sealed class VertexArray
    {
        public readonly int vertexArrayHandle;
        public readonly VertexBuffer vertexBuffer;
        
        public VertexArray(VertexBuffer vertexBuffer){
            if (vertexBuffer is null){
                throw new ArgumentNullException(nameof(vertexBuffer));
            }
            this.vertexBuffer = vertexBuffer;

            int vertexSizeInBytes = this.vertexBuffer.vertexInfo.sizeInBytes;

            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer.vertexBufferHandle);

            VertexAttribute[] attributes = vertexBuffer.vertexInfo.VertexAttributes;
            foreach (VertexAttribute vt in attributes){
                GL.VertexAttribPointer(vt.index, vt.componentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, vt.offsetInBytes);
                GL.EnableVertexArrayAttrib(this.vertexArrayHandle, vt.index);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}