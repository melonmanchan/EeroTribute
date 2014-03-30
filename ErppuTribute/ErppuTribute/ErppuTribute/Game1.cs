using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErppuTribute
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public enum GameState { MainMenu, Playing, PlayingVideo, GameOver }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MouseState originalMouseState;
        float leftrightRot = 0;
        float updownRot = 0;

        Camera camera;
        Maze maze;
        Menu menu;
        private Cube cube;
        BasicEffect effect;
        float moveScale = 1.7f;
        float rotateScale = 0.3f;

        SoundEffectInstance bgMusic;

        Video staticVideo;
        VideoPlayer videoplayer;
        Texture2D videoTexture;
        Rectangle videoScreen;
        int counter = 0;
        float countDuration = 1.5f;
        float currentTime = 0f;

        private List<Enemy> enemyList;
        private float enemyTimer = 0f;
        private float enemySpawnRate = 15f;
        private float enemySpeed = 30f;
        private bool isEnemyNear = false;

        public GameState gameState = GameState.MainMenu;
  
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1600;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            enemyList = new List<Enemy>();

            camera = new Camera(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, GraphicsDevice.Viewport.AspectRatio, 0.05f, 100f);

            effect = new BasicEffect(GraphicsDevice);

            maze = new Maze(GraphicsDevice, this.Content);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            bgMusic = Content.Load<SoundEffect>("spookybackgroundmusic").CreateInstance();
            bgMusic.Volume = 0.1f;

            cube = new Cube(this.GraphicsDevice, camera.Position, 10f, Content.Load<Texture2D>("eerominati"), Content.Load<SoundEffect>("ambienthum"));
            enemyList.Add(new Enemy(this.GraphicsDevice, camera.Position, 15.0f, Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum")));

            menu = new Menu(Content, spriteBatch, this);

            cube.emitter.Position = cube.location;
            camera.listener.Position = camera.Position;
            cube.soundEffectInstance.Apply3D(camera.listener, cube.emitter);

            videoplayer = new VideoPlayer();
            videoplayer.IsLooped = false;
            staticVideo = Content.Load<Video>("staticMovie");

            videoScreen = new Rectangle(GraphicsDevice.Viewport.X,
                    GraphicsDevice.Viewport.Y,
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height);


        Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        originalMouseState = Mouse.GetState();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            switch (gameState)
            {
                case GameState.MainMenu:
                    menu.Update();
                    break;
                case GameState.Playing:
                    cube.soundEffectInstance.Play();
                    bgMusic.Play();
                    UpdateGamePlay(gameTime);
                    break;
                case GameState.PlayingVideo:
                    playTransition(gameTime);
                     break;
                case GameState.GameOver:
                    //Peli ohi jutskat
                    break;
            }
            base.Update(gameTime);
        }

        public void playTransition(GameTime gameTime)
        {
            videoplayer.Play(staticVideo);

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

            if (currentTime >= countDuration)
            {
                counter++;
                currentTime -= countDuration; 
              
            }
            if (counter >= 4)
            {
                counter = 0;//Reset the counter;
                videoplayer.Stop();
                gameState = GameState.Playing;
            }
        }

        private void UpdateGamePlay(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();
            float moveAmountForward = 0;
            float moveAmountSideways = 0;



            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;

                leftrightRot -= rotateScale * xDifference * elapsed;
                updownRot -= -rotateScale * yDifference * elapsed;

                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                camera.RotationY = MathHelper.WrapAngle(leftrightRot);
                camera.RotationX = MathHelper.WrapAngle(updownRot);

            }

            if (keyState.IsKeyDown(Keys.A))
            {
                moveAmountSideways = moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                moveAmountSideways = -moveScale * elapsed;
            }

             if (keyState.IsKeyDown(Keys.W))
            {
                moveAmountForward = moveScale * elapsed;;
            }

             if (keyState.IsKeyDown(Keys.S))
            {
                moveAmountForward = -moveScale * elapsed;
            }

             if (keyState.IsKeyDown(Keys.Escape))
             {
                 resetGameLevel();
                 gameState = GameState.MainMenu;
             }

            //normalisoidaan nopeus ettei diagonaalisesti pysty liikkumaan liian nopeasti
            if (moveAmountForward != 0 && moveAmountSideways !=0)
            {
                moveAmountSideways *= 0.7071f;
                moveAmountForward *= 0.7071f;
            }

            if (moveAmountForward != 0)
            {
                Vector3 newLocation = camera.PreviewMove(moveAmountForward);
                bool moveOk = true;
                if (newLocation.X < 0 || newLocation.X > Maze.mazeWidth)
                    moveOk = false;
                if (newLocation.Z < 0 || newLocation.Z > Maze.mazeHeight)
                    moveOk = false;

                foreach (BoundingBox box in maze.GetBoundsForCell((int)newLocation.X, (int)newLocation.Z))
                {
                    if (box.Contains(newLocation) == ContainmentType.Contains)
                        moveOk = false;
                }

                if (moveOk)
                    camera.MoveForward(moveAmountForward);

            }

            if (moveAmountSideways != 0)
            {
                Vector3 newLocation = camera.PreviewMoveSideways(moveAmountSideways);
                bool moveOk = true;
                if (newLocation.X < 0 || newLocation.X > Maze.mazeWidth)
                    moveOk = false;
                if (newLocation.Z < 0 || newLocation.Z > Maze.mazeHeight)
                    moveOk = false;

                foreach (BoundingBox box in maze.GetBoundsForCell((int)newLocation.X, (int)newLocation.Z))
                {
                    if (box.Contains(newLocation) == ContainmentType.Contains)
                        moveOk = false;
                }

                if (moveOk)
                    camera.MoveSideways(moveAmountSideways);
            }

            // TODO: Add your update logic here
            updateAudioCue();

            if (cube.Hitbox.Contains(camera.Position) == ContainmentType.Contains)
            {
                resetGameLevel();
                gameState = GameState.MainMenu;
                return;
            }

            for (int i = 0; i < enemyList.Count; i++)
            {
                //Vihollisvektorin sijainnin suunta, pituus ja normaali kameravektorin sijaintiin
                Vector2 dir = new Vector2(enemyList[i].location.X - camera.Position.X, enemyList[i].location.Z - camera.Position.Z);
                float mag = (float)Math.Sqrt(Math.Abs(Math.Pow(dir.X, 2)) + Math.Abs(Math.Pow(dir.Y, 2)));
                Vector2 normal = new Vector2(dir.X / mag, dir.Y / mag);

                //Siirretään vihollista kameran suuntaan tietyllä nopeudella (20-40 arvot varmaan sopivia)
                enemyList[i].location = new Vector3(enemyList[i].location.X - (normal.X / (100.0f - enemySpeed)), enemyList[i].location.Y, enemyList[i].location.Z - (normal.Y / (100.0f - enemySpeed)));


                if (Vector3.Distance(camera.Position, enemyList[i].location) < 2.5)
                {
                    isEnemyNear = true;
                }

                if (enemyList[i].Hitbox.Contains(camera.Position) == ContainmentType.Contains)
                {
                    resetGameLevel();
                    gameState = GameState.MainMenu;
                    return;
                }

                enemyList[i].Update(gameTime);
            }

            if (isEnemyNear == false)
            {
                maze.drawTexture = maze.normalEero;
                if (maze.fogColor != Color.Black.ToVector3())
                {
                    bgMusic.Pitch = 0;
                    maze.fogColor = Color.Black.ToVector3();
                }
            }

            else if (isEnemyNear == true)
            {
                maze.drawTexture = maze.scaryEero;
                if (maze.fogColor != Color.Red.ToVector3())
                {
                    bgMusic.Pitch = 1;
                    maze.fogColor = Color.Red.ToVector3();
                }
                isEnemyNear = false;
            }

            enemyTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (enemyTimer >= enemySpawnRate)
            {
                randomizeEnemyPositions();
                enemyList.Add(new Enemy(this.GraphicsDevice, camera.Position, 5.0f, Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum")));
                enemyTimer = 0;
            }

            cube.Update(gameTime);
        }


        private void resetGameLevel()
        {
            updownRot = 0;
            leftrightRot = 0;
            bgMusic.Pitch = 0;
            bgMusic.Stop();
            enemyTimer = 0;
            isEnemyNear = false;
            maze.fogColor = Color.Black.ToVector3();
            cube.soundEffectInstance.Stop();
            camera.MoveTo(new Vector3(0.5f, 0.5f, 0.5f), 0, 0);
            maze.GenerateMaze();
            cube.PositionCube(camera.Position, 10f);
            enemyList.Clear();
            enemyList.Add(new Enemy(this.GraphicsDevice, camera.Position, 15.0f, Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum")));
        }

        private void randomizeEnemyPositions()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].PositionEnemy(camera.Position, 2.5f, 4);
            }
        }

        private void updateAudioCue()
        {
            camera.listener.Position = camera.Position;
            cube.soundEffectInstance.Apply3D(camera.listener, cube.emitter);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (gameState == GameState.MainMenu)
            {
                menu.Draw();
            }

            else if (gameState == GameState.PlayingVideo)
            {
                // Only call GetTexture if a video is playing or paused

                 if (videoplayer.State != MediaState.Stopped)
                    videoTexture = videoplayer.GetTexture();

                // Drawing to the rectangle will stretch the 
                // video to fill the screen
                

                // Draw the video, if we have a texture to draw.
                if (videoTexture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(videoTexture, videoScreen, Color.White);
                    spriteBatch.End();
                }
            }

            else if (gameState == GameState.Playing)
            {

                // TODO: Add your drawing code here
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                maze.Draw(camera, effect);
                cube.Draw(camera, effect);
                enemyList.ForEach(enemy => enemy.Draw(camera, effect));
            }
            
            base.Draw(gameTime);
        }

       
    }
}
