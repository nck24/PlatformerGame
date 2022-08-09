using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Security.Cryptography;
using GameTest.AI_code;

namespace GameTest
{
    public class Game : GameWindow
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private VertexArray vertexArray;
        private ShaderProgram shaderProgram;

        VertexBuffer sensorLines;
        VertexArray linesArray;

        private int verteciesLenght;
        private int indiciesLenght;

        Settings settings;
        bool isTraining;

        /*
        int colorFactorUniformLocation;

        private float colorFactor = 0f;
        private float deltaColorFactor = 1f / 512f;
        */

        private Random rand = new Random();
        bool isOver = false;
        bool canSpeedChange = true;
        bool stopSpeedChange = false;

        private RectangularPlayer player;
        private float basicVerticalPlayerSpeed = 100f;
        private float verticalPlayerSpeed;
        private float downBoost = 1.5f;
        private float horizontalPlayerSpeed = 0f;
        private float basicPlayerHorizontalSpeed = 120f;
        float playerSpeedChange;

        private Obsticle[] Platforms;
        List<Obsticle> LowerPlatforms;
        private float verticalPlatformSpeed = -75f;
        private float absoluteHorizontalPlatformSpeed = 40f;
        private const float platformHeight = 50;
        private const int platformMaxWidth = 400;
        private const int platformMinWidth = 250;
        private float startPointMultiplayer = 0;
        float platformSpeedChange;

        public Score score;

        public NeuralNetwork ai;
        float[] inputs;
        bool[] outputs;



        public Game(Settings settings, int width = 1280, int height = 720, string title = "Score : 0")
            : base(new GameWindowSettings(){
                RenderFrequency = 120,
                UpdateFrequency = 300,
                //IsMultiThreaded = false
            },
                new NativeWindowSettings(){
                Title = title,
                Size = new Vector2i(width, height),
                WindowBorder = WindowBorder.Fixed,
                StartVisible = false,
                StartFocused = false,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(3, 3)
                })
                {
            this.CenterWindow();
            this.settings = settings;
        }

        protected override void OnLoad()
        {
            this.RenderFrequency = this.settings.frameRate;
            this.UpdateFrequency = this.settings.updateFrequency;
            this.platformSpeedChange = settings.platformSpeedChange;
            this.playerSpeedChange = settings.playerSpeedChange;
            this.isTraining = settings.isAiTraining;

            if (this.isTraining){
                this.inputs = new float[this.ai.GetInputsNum()];
                this.outputs = new bool[this.ai.GetOutputsNum()];
                ai.SetEndFuncton( value => MathF.Max(0f, value) );
                ai.SetEndOutputFuncton( value => value );
                ai.SetActionFunction( (value) => {
                    if (value > 0){
                        return true;
                    }else{
                        return false;
                    }
                });
            }

            GL.ClearColor(Color4.LightSkyBlue);

            score = new Score();

            LowerPlatforms = new List<Obsticle>();

            int playerStartX = rand.Next(80, this.Size.X - 64);
            Rectangle playerRect = new Rectangle(new Vector2(playerStartX, 50), new Vector2(64, 64), new Vector4(1f, 0f, 0f, 1f));

            player = new RectangularPlayer(playerRect, new Vector2(horizontalPlayerSpeed, verticalPlayerSpeed));
            verticalPlayerSpeed = basicVerticalPlayerSpeed;
            Platforms = new Obsticle[4];
            float platformY = 300;
            for (int i = 0; i < Platforms.Length; i++){
                float platformLen = RandomNumberGenerator.GetInt32(platformMinWidth, platformMaxWidth);
                float platformX = (this.Size.X - platformLen) * startPointMultiplayer;
                startPointMultiplayer += -1;
                startPointMultiplayer = MathF.Abs(startPointMultiplayer);
                Vector2 startPoint = new Vector2(platformX, platformY);
                Vector2 size = new Vector2(platformLen, platformHeight);
                Vector4 color = new Vector4(.5f, .3f, .7f, 1f);
                Platforms[i] = new Obsticle(new Rectangle(startPoint, size, color));
                platformY += 200;
                LowerPlatforms.Add(Platforms[i]);
            }

            foreach (Obsticle obst in Platforms){
                obst.speed = new Vector2(0f, verticalPlatformSpeed);
            }

            verteciesLenght = Platforms.Length * Obsticle.verteciesLenght + RectangularPlayer.verteciesLenght;
            VertexPositionColor[] vertecies = new VertexPositionColor[verteciesLenght];
            player.playerRect.GetVerticies().CopyTo(vertecies, 0);
            int offset = RectangularPlayer.verteciesLenght;
            for (int i = 0; i < Platforms.Length; i++){
                Platforms[i].obsticleRect.GetVerticies().CopyTo(vertecies, offset);
                offset += Obsticle.verteciesLenght;
            }
            
            indiciesLenght = Platforms.Length * Obsticle.indiciesLenght + RectangularPlayer.indiciesLenght;
            int[] indicies = new int[indiciesLenght];
            player.playerRect.GetIndicies(0).CopyTo(indicies, 0);
            offset = RectangularPlayer.indiciesLenght;
            int indiciesStartIndex = 4;
            for (int i = 0; i < Platforms.Length; i++){
                Platforms[i].obsticleRect.GetIndicies(indiciesStartIndex).CopyTo(indicies, offset);
                offset += Obsticle.indiciesLenght;
                indiciesStartIndex += 4;
            }

            this.vertexBuffer = new VertexBuffer(VertexPositionColor.vertexInfo, vertecies.Length, false);
            this.vertexBuffer.SetData(vertecies, vertecies.Length);

            this.indexBuffer = new IndexBuffer(indicies.Length, true);
            this.indexBuffer.SetData(indicies, indicies.Length);

            this.vertexArray = new VertexArray(this.vertexBuffer);

            Shader vertexShader = new Shader(ShaderType.VertexShader, "vertexShader", "Shaders/VertexShader.glsl");
            Shader fragmentShader = new Shader(ShaderType.FragmentShader, "fragmentShader", "Shaders/FragmentShader.glsl");

            Shader[] shaders = {vertexShader, fragmentShader};

            shaderProgram = new ShaderProgram(shaders);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            GL.UseProgram(this.shaderProgram.shaderProgramHandle);
            int viewportSizeUniformLocation = GL.GetUniformLocation(this.shaderProgram.shaderProgramHandle, "ViewportSize");
            GL.Uniform2(viewportSizeUniformLocation, (float)viewport[2], (float)viewport[3]);

            /*
            colorFactorUniformLocation = GL.GetUniformLocation(this.shaderProgram.shaderProgramHandle, "colorFactor");
            GL.Uniform1(colorFactorUniformLocation, colorFactor);
            */

            GL.UseProgram(0);

            this.IsVisible = true;
            this.Focus();

            base.OnLoad();

            score.StartTimer();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            /*
            colorFactor += deltaColorFactor;

            if (colorFactor >= 1f){
                colorFactor = 1f;
                deltaColorFactor = deltaColorFactor * -1f;
            }

            if (colorFactor <= 0f){
                colorFactor = 0f;
                deltaColorFactor = deltaColorFactor * -1f;
            }

            GL.Uniform1(colorFactorUniformLocation, colorFactor);
            */
            if (this.isOver && this.IsAnyKeyDown){
                return;
            }
            if (isOver){
                this.Close();
            }

            float deltaT = (float)args.Time;
            MovePlatforms(deltaT);
            PlayerMovement();
            player.ChangeSpeed(horizontalPlayerSpeed, verticalPlayerSpeed);

            player.MovePlayer(deltaT);

            PlayerCollision();
            GameOverCheck();
            PlatformTeleportation();
            PlatformsSideCollision();

            VertexPositionColor[] vertecies = new VertexPositionColor[verteciesLenght];
            player.playerRect.GetVerticies().CopyTo(vertecies, 0);
            int offset = RectangularPlayer.verteciesLenght;
            for (int i = 0; i < Platforms.Length; i++){
                Platforms[i].obsticleRect.GetVerticies().CopyTo(vertecies, offset);
                offset += Obsticle.verteciesLenght;
            }
            vertexBuffer.SetData(vertecies, vertecies.Length);
            
            ChangeScore();
            ChangesBasedOnScore();
            //Console.WriteLine("speed : {0}   vektorsko : {1}", speed, MathF.Sqrt((player.speed.X * player.speed.X) + 
            //    (player.speed.Y * player.speed.Y)));
            horizontalPlayerSpeed = 0;

            base.OnUpdateFrame(args);
            
            //Console.WriteLine("OnUpdateFrame time = {0}", args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.shaderProgram.shaderProgramHandle);
            GL.BindVertexArray(this.vertexArray.vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer.vertexBufferHandle); 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.indexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, this.indexBuffer.indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
            //Console.WriteLine("OnRenderFrame");
        }
        
        void MovePlatforms(float deltaT){
            foreach (Obsticle obst in Platforms){
                obst.Move(deltaT);
            }
        }

        void PlatformTeleportation(){
            foreach (Obsticle obst in Platforms){
                if (obst.obsticleRect.GetDownY() <= 0){
                    obst.speed.X = 0f; //set horizontal speed to 0
                    float lowestPlatformY = Platforms.Max( o => o.obsticleRect.GetUpY() );
                    float platformLen = (float)RandomNumberGenerator.GetInt32(platformMinWidth, platformMaxWidth);
                    float newY = lowestPlatformY + 200f;
                    float newX = (this.Size.X - platformLen) * startPointMultiplayer;
                    startPointMultiplayer += -1f;
                    startPointMultiplayer = MathF.Abs(startPointMultiplayer);
                    Vector2 speed = new Vector2(0f, verticalPlatformSpeed);
                    // 1 in 3 platforms will be around the middle of the screen
                    if (RandomNumberGenerator.GetInt32(2) == 0){
                        if (platformLen > 390){
                            platformLen = 390;
                        }
                        newX = RandomNumberGenerator.GetInt32(this.Size.X - (int)platformLen);
                        // 1 in 2 "middle" platforms will be moving
                        if (RandomNumberGenerator.GetInt32(2) == 0){
                            float multiplayer = 1f;
                            float speedMultiplayer = rand.NextSingle();
                            if (speedMultiplayer < 0.3f){
                                speedMultiplayer = 1f;
                            }
                            if (RandomNumberGenerator.GetInt32(2) == 0){
                                multiplayer *= -1f;
                            }
                            speed.X = absoluteHorizontalPlatformSpeed * multiplayer * speedMultiplayer;
                        }
                    }
                    Vector2 newSize = new Vector2(platformLen, platformHeight);
                    Vector2 newStartPoint = new Vector2(newX, newY);
                    Vector4 color = obst.obsticleRect.GetColor();
                    obst.Teleport(newStartPoint);
                    obst.ChangeSize(newSize);
                    obst.speed = speed;
                }
            }
        }

        void PlatformsSideCollision(){
            foreach (Obsticle platform in Platforms){
                if (platform.obsticleRect.GetRightX() > this.Size.X){
                    platform.speed.X *= -1f;
                    Vector2 newStartPoint = (this.Size.X - platform.obsticleRect.GetSize().X, platform.obsticleRect.GetUpY());
                    platform.Teleport(newStartPoint);
                }
                if (platform.obsticleRect.GetLeftX() < 0){
                    platform.speed.X *= -1f;
                    Vector2 newStartPoint = (0, platform.obsticleRect.GetUpY());
                    platform.Teleport(newStartPoint);
                }
            }
        }

        void PlayerMovement(){
            if (!this.isTraining){
                if (this.KeyboardState.IsKeyDown(Keys.Down) && !this.KeyboardState.WasKeyDown(Keys.Down)){
                    verticalPlayerSpeed *= downBoost;
                }
                if (this.KeyboardState.IsKeyReleased(Keys.Down)){
                    verticalPlayerSpeed = basicVerticalPlayerSpeed;
                }
                if (this.KeyboardState.IsKeyDown(Keys.Right)){
                    horizontalPlayerSpeed = basicPlayerHorizontalSpeed;
                }
                if (this.KeyboardState.IsKeyDown(Keys.Left)){
                    horizontalPlayerSpeed = - basicPlayerHorizontalSpeed;
                }
                if (this.KeyboardState.IsKeyDown(Keys.Right) && this.KeyboardState.IsKeyDown(Keys.Left)){
                    horizontalPlayerSpeed = 0f;
                }
            }
            if (this.isTraining){

            }
        }

        void PlayerCollision(){
            if (player.playerRect.GetDownY() >= this.Size.Y){
                float x = player.playerRect.GetLeftX();
                player.Teleport(new Vector2(x, this.Size.Y - player.playerRect.GetSize().Y));
            }
            if (player.playerRect.GetLeftX() <= 0){
                float y = player.playerRect.GetUpY();
                player.Teleport(new Vector2(0, y));
            }
            if (player.playerRect.GetRightX() >= this.Size.X){
                float y = player.playerRect.GetUpY();
                player.Teleport(new Vector2(this.Size.X - player.playerRect.GetSize().X, y));
            }
            
            foreach (Obsticle platform in Platforms){
                if (platform.IsColliding(player.playerRect)){
                    CollisionResolvment? collSolution = platform.GetRectVsRectCollisionResolvment(player.playerRect);
                    //Console.WriteLine(collSolution);
                    if (collSolution == CollisionResolvment.Up){
                        float newX = player.playerRect.GetLeftX();
                        float newY = platform.obsticleRect.GetUpY() - player.playerRect.GetSize().Y;
                        Vector2 newStartPoint = new Vector2(newX, newY);
                        player.Teleport(newStartPoint);
                    }
                    if (collSolution == CollisionResolvment.Down){
                        float newX = player.playerRect.GetLeftX();
                        float newY = platform.obsticleRect.GetDownY();
                        Vector2 newStartPoint = new Vector2(newX, newY);
                        player.Teleport(newStartPoint);
                    }
                    if (collSolution == CollisionResolvment.Left){
                        float newX = platform.obsticleRect.GetLeftX() - player.playerRect.GetSize().X;
                        float newY = player.playerRect.GetUpY();
                        Vector2 newStartPoint = new Vector2(newX, newY);
                        player.Teleport(newStartPoint);
                    }
                    if (collSolution == CollisionResolvment.Right){
                        float newX = platform.obsticleRect.GetRightX();
                        float newY = player.playerRect.GetUpY();
                        Vector2 newStartPoint = new Vector2(newX, newY);
                        player.Teleport(newStartPoint);
                    }
                }
            }
        }

        void ChangeScore(){
            for (int i = 0; i < LowerPlatforms.Count; i++){
                if (LowerPlatforms[i].obsticleRect.GetDownY() <= player.playerRect.GetUpY()){
                    score.IncrementScoreBy1();
                    this.Title = "Score : " + score.GetScore();
                }
            }
            GetLowerPlatforms();
        }

        void GetLowerPlatforms(){
            LowerPlatforms.Clear();
            for (int i = 0; i < Platforms.Length; i++){
                if (Platforms[i].obsticleRect.GetDownY() >= player.playerRect.GetUpY()){
                    LowerPlatforms.Add(Platforms[i]);
                }
            }
        }

        void ChangesBasedOnScore(){
            if (score.GetScore() % 10 == 0 && score.GetScore() != 0 && canSpeedChange && !stopSpeedChange){
                //Increase speed
                Console.WriteLine("Speed increased");
                verticalPlatformSpeed *= platformSpeedChange;
                absoluteHorizontalPlatformSpeed *= platformSpeedChange;
                basicVerticalPlayerSpeed *= playerSpeedChange;
                basicPlayerHorizontalSpeed *= playerSpeedChange;
                foreach (Obsticle platform in Platforms){
                    platform.speed *= platformSpeedChange;
                }
                canSpeedChange = false;
            }
            if (score.GetScore() % 10 != 0){
                canSpeedChange = true;
            }
            if (MathF.Abs(verticalPlatformSpeed) > 400 && !stopSpeedChange){
                stopSpeedChange = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Speed won't increase any more");
                Console.ResetColor();
            }
        }

        void GameOverCheck(){
            if (player.playerRect.GetDownY() <= 0 && !isOver){
                Console.WriteLine("Release all keys");
                isOver = true;
                score.StopTimer();
                score.SaveElapsedTime();
            }
        }
    }
}