/****************************************************
 * Class: Game
 * Description: "Main" class of the game
 * Author(s): Matti Jokitulppo, Jonah Ahvonen
 * Date: April 1, 2014
****************************************************/

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
    
    public enum GameState { MainMenu, Playing, PlayingVideo }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public int screenWidth = 1600;
        public int screenHeight = 1000;

        private MouseState originalMouseState;
        float leftrightRot = 0;
        float updownRot = 0;
        private Random rnd = new Random();

        private Camera camera;
        private Maze maze;
        private Menu menu;
        private Cube cube;
        private float cubeCollisionRadius;
        private BasicEffect effect;
        private float moveScale = 1.7f;
        private float rotateScale = 0.3f;

        private SoundEffectInstance bgMusic;

        private List<SoundEffectInstance> eeroShouts;

        private Video staticVideo;
        private VideoPlayer videoplayer;
        private Texture2D videoTexture;
        private Rectangle videoScreen;
        private bool hasVideoPlayed = false;

        private int counter = 0;
        private float countDuration = 1.5f;
        private float currentTime = 0f;

        private List<Enemy> enemyList;
        private float enemyTimer = 0f;
        private float enemySpawnRate = 15f;
        private float enemySpeed = 30f;
        private float enemyCollisionRadius = 0.5f;
        private float enemyspawnmin = 15;
        private float enemyneardistance = 2.5f;
        private bool isEnemyNear = false;

        public GameState gameState = GameState.MainMenu;

        private ConfigHandler configHandler;

        private float cubespawnmin = 10;
        private float backgroundMusicVolume = 0.2f;

        private Keys forwardKey = Keys.W;
        private Keys leftKey = Keys.A;
        private Keys backwardKey = Keys.S;
        private Keys rightKey = Keys.D;
        private Keys shoutKey = Keys.E;

        #endregion
        #region Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            configHandler = new ConfigHandler();
            loadConfig();
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;

            Content.RootDirectory = "Content";

          var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);

         form.Location = new System.Drawing.Point(100, 10);


        }
        #endregion
        #region Load Game Content
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            enemyList = new List<Enemy>();

            loadConfig();

            camera = new Camera(new Vector3(0.5f, 0.5f, 0.5f), 0, 0, GraphicsDevice.Viewport.AspectRatio, 0.05f, 100f);

            effect = new BasicEffect(GraphicsDevice);

            maze = new Maze(GraphicsDevice, this.Content);

            this.IsMouseVisible = true;

            base.Initialize();
        }

        private void loadConfig()
        {
            configHandler.ReadConfig();
            try
            {
                moveScale = (float)configHandler.ConfigBundle["movescale"];
                rotateScale = (float)configHandler.ConfigBundle["rotatescale"];
                countDuration = (float)configHandler.ConfigBundle["countduration"];
                cubespawnmin = (float)configHandler.ConfigBundle["cubespawnmindistance"];
                enemySpawnRate = (float)configHandler.ConfigBundle["enemyspawnrate"];
                enemySpeed = (float)configHandler.ConfigBundle["enemyspeed"];
                enemyspawnmin = (float)configHandler.ConfigBundle["enemyspawnmindistance"];
                enemyneardistance = (float)configHandler.ConfigBundle["enemyneardistance"];
                cubeCollisionRadius = (float)configHandler.ConfigBundle["cubecollisionradius"];

                screenHeight = (int)configHandler.ConfigBundle["screenheight"];
                screenWidth = (int)configHandler.ConfigBundle["screenwidth"];

                enemyCollisionRadius = (float)configHandler.ConfigBundle["enemycollisionradius"];

                backgroundMusicVolume = (float)configHandler.ConfigBundle["backgroundmusicvolume"];
                forwardKey = (Keys)Enum.Parse(typeof(Keys), (string)configHandler.ConfigBundle["forwardkey"]);
                leftKey = (Keys)Enum.Parse(typeof(Keys), (string)configHandler.ConfigBundle["leftkey"]);
                backwardKey = (Keys)Enum.Parse(typeof(Keys), (string)configHandler.ConfigBundle["backwardkey"]);
                rightKey = (Keys)Enum.Parse(typeof(Keys), (string)configHandler.ConfigBundle["rightkey"]);
                shoutKey = (Keys)Enum.Parse(typeof(Keys), (string)configHandler.ConfigBundle["shoutkey"]);
            }
            catch(KeyNotFoundException kfe)
            {
                Console.WriteLine("Config is missing values!");
            }
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            eeroShouts = new List<SoundEffectInstance>();
            eeroShouts.Add(Content.Load<SoundEffect>("Eero1").CreateInstance());
            eeroShouts.Add(Content.Load<SoundEffect>("Eero2").CreateInstance());
            eeroShouts.Add(Content.Load<SoundEffect>("Eero3").CreateInstance());
            eeroShouts.Add(Content.Load<SoundEffect>("Eero4").CreateInstance());
            eeroShouts.Add(Content.Load<SoundEffect>("Eero5").CreateInstance());
            eeroShouts.Add(Content.Load<SoundEffect>("Eero6").CreateInstance());

            for (int i = 0; i < eeroShouts.Count; i++)
            {
                eeroShouts[i].Pitch = -0.75f;
                eeroShouts[i].Volume = 0.2f;
            }
            bgMusic = Content.Load<SoundEffect>("spookybackgroundmusic").CreateInstance();
            bgMusic.Volume = backgroundMusicVolume;

            cube = new Cube(this.GraphicsDevice, camera.Position, cubespawnmin, cubeCollisionRadius, Content.Load<Texture2D>("eerominati"), Content.Load<SoundEffect>("ambienthum"));
            enemyList.Add(new Enemy(this.GraphicsDevice, camera.Position, enemyspawnmin, enemyCollisionRadius,Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum")));

            menu = new Menu(Content, spriteBatch, this, GraphicsDevice);

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

            centerMouse();
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
        #endregion
        #region Main Loop
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
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
            }
            base.Update(gameTime);
        }
        #endregion
        #region Video/Audio related Methods
        public void playTransition(GameTime gameTime)
        {
            if (Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                videoplayer.Stop();
                hasVideoPlayed = true;
            }
                
            if (hasVideoPlayed == true)
            {
                gameState = GameState.Playing;
                centerMouse();
            }

            else
            {
                videoplayer.Play(staticVideo);
                currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currentTime >= countDuration)
                {
                    counter++;
                    currentTime -= countDuration;

                }
                if (counter >= 4)
                {
                    counter = 0;//Reset the counter;
                    videoplayer.Stop();
                    hasVideoPlayed = true;
                }
            }
        }

        private bool isShoutPlaying()
        {
            for (int i = 0; i < eeroShouts.Count; i++)
            {
                if (eeroShouts[i].State == SoundState.Playing)
                {
                    return true;
                }

            }
            return false;
        }

        private void changeShoutPitch()
        {
            for (int i = 0; i < eeroShouts.Count; i++)
            {
                if (eeroShouts[i].State == SoundState.Playing && isEnemyNear)
                {
                    eeroShouts[i].Pitch = 1f;
                }

                else
                {
                    eeroShouts[i].Pitch = -0.75f;
                }
            }
        }

        private void stopAllEeroShouts()
        {
            for (int i = 0; i < eeroShouts.Count; i++)
                eeroShouts[i].Stop();
        }

        private void updateAudioCue()
        {
            camera.listener.Position = camera.Position;
            cube.soundEffectInstance.Apply3D(camera.listener, cube.emitter);
        }

        #endregion
        #region Update
        private void UpdateGamePlay(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();

            Vector2 moveAmount = Vector2.Zero;

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;

                leftrightRot -= rotateScale * xDifference * elapsed;
                updownRot -= -rotateScale * yDifference * elapsed;

                centerMouse();

                camera.RotationY = MathHelper.WrapAngle(leftrightRot);
                camera.RotationX = MathHelper.WrapAngle(updownRot);
            }

            if (keyState.IsKeyDown(leftKey))
            {
                moveAmount.X = moveScale * elapsed;
            }

            if (keyState.IsKeyDown(rightKey))
            {
                moveAmount.X = -moveScale * elapsed;
            }

             if (keyState.IsKeyDown(forwardKey))
            {
                 moveAmount.Y = moveScale * elapsed;
            }

             if (keyState.IsKeyDown(backwardKey))
            {
                moveAmount.Y = -moveScale * elapsed;
            }

             if (keyState.IsKeyDown(shoutKey) && (isShoutPlaying() != true))
             {
                 eeroShouts[rnd.Next(eeroShouts.Count)].Play();
             }
             if (keyState.IsKeyDown(Keys.Escape))
             {
                 resetGameLevel();
                 gameState = GameState.MainMenu;
             }

            //normalisoidaan nopeus ettei diagonaalisesti pysty liikkumaan liian nopeasti
            if (moveAmount.X != 0 && moveAmount.Y != 0)
            {
                moveAmount.X *= 0.7071f;
                moveAmount.Y *= 0.7071f;
            }

            if (moveAmount.X != 0 || moveAmount.Y != 0)
            {
                Vector3 newLocation = camera.PreviewMoveVector(moveAmount);
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
                    camera.MoveForwardVector(moveAmount);
            }

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

                //Siirret��n vihollista kameran suuntaan tietyll� nopeudella (20-40 arvot varmaan sopivia)
                enemyList[i].location = new Vector3(enemyList[i].location.X - (normal.X / (100.0f - enemySpeed)), enemyList[i].location.Y, enemyList[i].location.Z - (normal.Y / (100.0f - enemySpeed)));

                if (Vector3.Distance(camera.Position, enemyList[i].location) < enemyneardistance)
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

            changeShoutPitch();

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
                enemyList.Add(new Enemy(this.GraphicsDevice, camera.Position, enemyspawnmin, enemyCollisionRadius, Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum")));
                enemyTimer = 0;
            }
            cube.Update(gameTime);
        }
        #endregion
        #region Miscallenous Gameplay Methods
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
            enemyList.Add(new Enemy(this.GraphicsDevice, camera.Position, 15.0f, enemyCollisionRadius, Content.Load<Texture2D>("nmi"), Content.Load<SoundEffect>("ambienthum")));
            this.IsMouseVisible = true;
            stopAllEeroShouts();
        }

        private void randomizeEnemyPositions()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].PositionEnemy(camera.Position, 2.5f, 4);
            }
        }

        public void centerMouse()
        {
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        }
        #endregion
        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        ///
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
        #endregion
    }
}
