using OpenTK.Mathematics;

namespace GameTest
{
    public readonly struct VertexAttribute
    {
        public readonly string name;
        public readonly int index;
        public readonly int componentCount;
        public readonly int offsetInBytes;

        public VertexAttribute(string name, int index, int componentCount, int offset){
            this.name = name;
            this.index = index;
            this.componentCount = componentCount;
            this.offsetInBytes = offset;
        }
    }

    public sealed class VertexInfo
    {
        public readonly Type type;
        public readonly int sizeInBytes;
        public readonly VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] attributes){
            this.type = type;
            this.sizeInBytes = 0;
            this.VertexAttributes = attributes;

            for (int i = 0; i < VertexAttributes.Length; i++){
                VertexAttribute attribute = this.VertexAttributes[i];
                this.sizeInBytes += attribute.componentCount * sizeof(float);
            }
        }
    }

    public readonly struct VertexPositionColor
    {
        public readonly Vector2 position;
        public readonly Vector4 color;

        public static readonly VertexInfo vertexInfo = new VertexInfo(
            typeof(VertexPositionColor), 
            new VertexAttribute("Position", 0, 2, 0), 
            new VertexAttribute("Color", 1, 4, 2 * sizeof(float))
            );

        public VertexPositionColor(Vector2 position, Vector4 color){
            this.position = position;
            this.color = color;
        }
    }

    public readonly struct VertexPositionTexture
    {
        public readonly Vector2 position;
        public readonly Vector2 texCoord;

        public static readonly VertexInfo vertexInfo = new VertexInfo(
            typeof(VertexPositionTexture),
            new VertexAttribute("position", 0, 2, 0),
            new VertexAttribute("texture", 1, 2, 2 * sizeof(float))
        );

        public VertexPositionTexture(Vector2 position, Vector2 texCoord){
            this.position = position;
            this.texCoord = texCoord;
        }
    }
}