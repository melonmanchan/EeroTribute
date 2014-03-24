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

        Camera camera;
        Maze maze;
        Menu menu;
        private Cube cube;
        BasicEffect effect;
        Texture2D groundTexture;
        float moveScale = 1.7f;
        float rotateScale = MathHelper.PiOver2;

        Video staticVideo;
        VideoPlayer videoplayer;
        Texture2D videoTexture;
        Rectangle videoScreen;
        int counter = 0;
        float countDuration = 1.5f;
        float currentTime = 0f;

        private Enemy enemy;

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
            camera = new Camera(new Vector3(0.5f, 0.5f, 0.5f), 0, GraphicsDevice.Viewport.AspectRatio, 0.05f, 100f);

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
            groundTexture = Content.Load<Texture2D>("erp");

            List<Texture2D> buttons = new List<Texture2D>();
            List<Texture2D> selectedbuttons = new List<Texture2D>();
            selectedbuttons.Add(Content.Load<Texture2D>("newButton"));
            buttons.Add(Content.Load<Texture2D>("newButtonJpn"));
            selectedbuttons.Add(Content.Load<Texture2D>("quitButton"));
            buttons.Add(Content.Load<Texture2D>("quitButtonJpn"));

            cube = new Cube(this.GraphicsDevice, camera.Position, 10f, Content.Load<Texture2D>("eerominati"), Content.Load<SoundEffect>("ambienthum"));
            enemy = new Enemy(this.GraphicsDevice, camera.Position, 15.0f, Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum"));
            menu = new Menu(Content.Load<Texture2D>("Eerobg"), Content.Load<Texture2D>("pointer"),buttons, selectedbuttons,spriteBatch, Content.Load<SoundEffect>("selectionChange"), Content.Load<SoundEffect>("boom"),this);


            cube.emitter.Position = cube.location;
            camera.listener.Position = camera.Position;
            cube.soundEffectInstance.Apply3D(camera.listener, cube.emitter);

            videoplayer = new VideoPlayer();
            videoplayer.IsLooped = false;
            staticVideo = Content.Load<Video>("staticMovie");

           // cube.soundEffectInstance.Play();

            videoScreen = new Rectangle(GraphicsDevice.Viewport.X,
                    GraphicsDevice.Viewport.Y,
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height);
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

        private void resetGameLevel()
        {
            cube.soundEffectInstance.Stop();
            camera.MoveTo(new Vector3(0.5f, 0.5f, 0.5f),0);
            maze.GenerateMaze();
            cube.PositionCube(camera.Position, 10f);
        }

        private void UpdateGamePlay(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();
            float moveAmountForward = 0;
            float moveAmountSideways = 0;

            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                camera.Rotation = MathHelper.WrapAngle(camera.Rotation - (rotateScale * elapsed));
                //  moveAmountSideways = -moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                camera.Rotation = MathHelper.WrapAngle(camera.Rotation + (rotateScale * elapsed));

                //   moveAmountSideways = moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                //camera.MoveForward(moveScale * elapsed);
                moveAmountForward = moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
            {
                //camera.MoveForward(-moveScale * elapsed);
                moveAmountForward = -moveScale * elapsed;
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
            }

            
            //Vihollisvektorin sijainnin suunta, pituus ja normaali kameravektorin sijaintiin
            Vector2 dir = new Vector2(enemy.location.X - camera.Position.X, enemy.location.Z - camera.Position.Z);
            float mag = (float)Math.Sqrt(Math.Abs(Math.Pow(dir.X, 2)) + Math.Abs(Math.Pow(dir.Y, 2)));
            Vector2 normal = new Vector2(dir.X / mag, dir.Y / mag);

            //Siirret‰‰n vihollista kameran suuntaan tietyll‰ nopeudella (20-40 arvot varmaan sopivia)
            float speed = 25.0f;
            enemy.location = new Vector3(enemy.location.X - (normal.X / (100.0f - speed)), enemy.location.Y, enemy.location.Z - (normal.Y / (100.0f - speed)));

            if(enemy.Hitbox.Contains(camera.Position) == ContainmentType.Contains)
            {
                
            }

            cube.Update(gameTime);
            enemy.Update(gameTime);
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
                effect.Texture = groundTexture;
                maze.Draw(camera, effect);
                cube.Draw(camera, effect);
                enemy.Draw(camera, effect);
            }
            
            base.Draw(gameTime);
        }

       
    }
}
