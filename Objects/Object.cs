using OpenTK.Mathematics;

namespace GameTest
{
    public abstract class Object
    {
        private Vector2 startPoint;
        private Vector4 color;
        private VertexPositionColor[] vertecies = Array.Empty<VertexPositionColor>();

        public void ChangeColorObject(Vector4 newColor){
            color = newColor;
        }

        public void WriteStartPoint(){
            Console.WriteLine("Start point : x = {0}  y = {1}", startPoint.X, startPoint.Y);
        }

        public VertexPositionColor[] GetVerticies(){
            return vertecies;
        }
        public Vector4 GetColor(){
            return color;
        }

        public Vector2 GetStartPoint(){
            return startPoint;
        }

        protected void SetStartPoint(Vector2 startPoint){
            this.startPoint = startPoint;
        }

        protected void SetColor(Vector4 color){
            this.color = color;
        }

        protected void SetVertecies(VertexPositionColor[] vertecies){
            this.vertecies = vertecies;
        }

        //public abstract VertexPositionColor[] GetVertecies();

        public abstract void Move(Vector2 change);

        public abstract void ChangeColor(Vector4 newColor);

        public abstract void WriteVertecies();
    }
}