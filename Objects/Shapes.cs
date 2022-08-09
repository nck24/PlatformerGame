using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameTest
{
    public class Rectangle : Object
    {
        private Vector2 startPoint;
        private VertexPositionColor[] vertecies;
        private Vector2 size;
        private Vector4 color;

        public const int verteciesLenght = 4;
        public const int indiciesLenght = 6;

        public Rectangle(Vector2 startPoint, Vector2 size ,Vector4 color){
            vertecies = new VertexPositionColor[4];
            this.startPoint = startPoint;
            vertecies[0] = new VertexPositionColor(startPoint, color);
            vertecies[1] = new VertexPositionColor(new Vector2(startPoint.X + size.X, startPoint.Y), color);
            vertecies[2] = new VertexPositionColor(new Vector2(startPoint.X + size.X, startPoint.Y + size.Y), color);
            vertecies[3] = new VertexPositionColor(new Vector2(startPoint.X, startPoint.Y + size.Y), color);

            this.size = size;
            this.color = color;
            this.SetColor(this.color);
            this.SetStartPoint(this.startPoint);
            this.SetVertecies(this.vertecies);
        }

        public override void WriteVertecies(){
            foreach(VertexPositionColor v in vertecies){
                Console.WriteLine("Position : x = {0}  y = {1}   color : r = {2} g = {3} b = {4}", v.position.X, v.position.Y,
                    v.color.X, v.color.Y, v.color.W);
            }
        }

        ///<summary>
        ///Moves all the vertecies for a vector
        ///<summary>
        public override void Move(Vector2 change){
            VertexPositionColor[] newVertecies = new VertexPositionColor[4];
            for (int i = 0; i < 4; i++){
                newVertecies[i] = new VertexPositionColor(vertecies[i].position + change, color);
            }
            vertecies = newVertecies;
            startPoint = vertecies[0].position;
            this.SetVertecies(vertecies);
            this.SetStartPoint(startPoint);
        }

        public override void ChangeColor(Vector4 newColor)
        {
            VertexPositionColor[] newVertecies = new VertexPositionColor[4];
            for (int i = 0; i < 4; i++){
                newVertecies[i] = new VertexPositionColor(vertecies[i].position, newColor);
            }
            vertecies = newVertecies;
            this.SetVertecies(vertecies);
        }

        public static VertexPositionColor[] ReturnVPCArray(Rectangle[] array)
        {
            VertexPositionColor[] VPCarray = new VertexPositionColor[array.Length * verteciesLenght];
            int k = 0;
            foreach (Rectangle r in array){
                VertexPositionColor[] rectVert = r.GetVerticies();
                foreach (VertexPositionColor vpc in rectVert){
                    VPCarray[k] = vpc;
                    k++;
                }
            }
            return VPCarray;
        }

        public float GetUpY(){
            return vertecies[0].position.Y;
        }

        public float GetDownY(){
            return GetUpY() + size.Y;
        }

        public float GetLeftX(){
            return vertecies[0].position.X;
        }

        public float GetRightX(){
            return GetLeftX() + size.X;
        }

        public Vector2 GetSize(){
            return this.size;
        }

        public Vector2[] GetPoints(){
            Vector2[] points = new Vector2[4];
            for (int i = 0; i < 4; i++){
                points[i] = new Vector2(this.vertecies[i].position.X, this.vertecies[i].position.Y);
            }
            return points;
        }

        public int[] GetIndicies(int startIndex){
            return new int[] {startIndex, startIndex + 1, startIndex + 2, startIndex + 3, startIndex, startIndex + 2};
        }
    }

    public class Line : Object
    {
        private Vector2 startPoint;
        private Vector2 endPoint;
        private VertexPositionColor[] vertecies;
        private Vector4 color;

        public Line(Vector2 startPoint, Vector2 endPoint, Vector4 color){
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.color = color;

            this.vertecies = new VertexPositionColor[2];
            this.vertecies[0] = new VertexPositionColor(startPoint, color);
            this.vertecies[1] = new VertexPositionColor(endPoint, color);

            this.SetColor(this.color);
            this.SetStartPoint(this.startPoint);
            this.SetVertecies(this.vertecies);
        }

        public override void ChangeColor(Vector4 newColor)
        {
            VertexPositionColor[] newVertecies = new VertexPositionColor[4];
            for (int i = 0; i < 4; i++){
                newVertecies[i] = new VertexPositionColor(vertecies[i].position, newColor);
            }
            this.vertecies = newVertecies;
            this.SetVertecies(vertecies);
        }

        public override void Move(Vector2 change)
        {
            VertexPositionColor[] newVertecies = new VertexPositionColor[4];
            for (int i = 0; i < 4; i++){
                newVertecies[i] = new VertexPositionColor(vertecies[i].position + change, color);
            }
            this.vertecies = newVertecies;
            this.startPoint = vertecies[0].position;
            this.endPoint = vertecies[1].position;
            this.SetVertecies(vertecies);
            this.SetStartPoint(startPoint);
        }

        public override void WriteVertecies()
        {
            foreach(VertexPositionColor v in vertecies){
                Console.WriteLine("Position : x = {0}  y = {1}   color : r = {2} g = {3} b = {4}", v.position.X, v.position.Y,
                    v.color.X, v.color.Y, v.color.W);
            }
        }
    }
}