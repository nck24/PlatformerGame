using OpenTK.Mathematics;

namespace GameTest
{
    public class RectangularPlayer
    {
        public Rectangle playerRect;
        protected Vector2 size;
        protected Vector4 color;
        public Vector2 speed;
        public const int verteciesLenght = Rectangle.verteciesLenght;
        public const int indiciesLenght = Rectangle.indiciesLenght;
        //private int sensorAmount;
        //private int fov;

        public RectangularPlayer(Rectangle playerRect){
            this.playerRect = playerRect;
            this.speed = new Vector2(0, 0);
            this.size = playerRect.GetSize();
            this.color = playerRect.GetColor();
        }

        public RectangularPlayer(Rectangle playerRect, Vector2 speed){
            this.playerRect = playerRect;
            this.speed = speed;
            this.size = playerRect.GetSize();
            this.color = playerRect.GetColor();
        }

        public void ChangeSpeed(float x, float y){
            speed = new Vector2(x, y);
        }
        
        public void MovePlayer(float deltaT){
            Vector2 change = speed * deltaT;
            playerRect.Move(change);
        }

        public void Teleport(Vector2 newStartPoint){
            playerRect = new Rectangle(newStartPoint, this.size, this.color);
        }

        public VertexPositionColor[] GetVertecies(){
            return playerRect.GetVerticies();
        }
    }

    public class AiRectangularPlayer : RectangularPlayer
    {
        int sensorNum;
        int angle;
        float[] sensorOutputs;

        public AiRectangularPlayer(int sensorNum, int angle, Rectangle playerRect) : base(playerRect){
            this.sensorNum = sensorNum;
            this.angle = angle;
            sensorOutputs = new float[sensorNum];
        }
    }
}