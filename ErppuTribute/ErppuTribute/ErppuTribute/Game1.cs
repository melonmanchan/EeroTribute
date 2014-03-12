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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        Maze maze;
        private Cube cube;
        BasicEffect effect;
        Texture2D groundTexture;
        float moveScale = 1.5f;
        float rotateScale = MathHelper.PiOver2;

        //menujutskast
        Texture2D mainMenu;
        enum GameState { MainMenu, Playing, GameOver }
        GameState gameState = GameState.MainMenu;
  
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080;
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
            cube = new Cube(this.GraphicsDevice, camera.Position, 10f, Content.Load<Texture2D>("Glass"));
            mainMenu = Content.Load<Texture2D>("erp");
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
                    KeyboardState ks = Keyboard.GetState();
                    if (ks.IsKeyDown(Keys.Space))
                    {
                        GraphicsDevice.BlendState = BlendState.Opaque;
                        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                        gameState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    UpdateGamePlay(gameTime);
                    break;
                case GameState.GameOver:
                    //Peli ohi jutskat
                    break;
            }
            base.Update(gameTime);
        }

        private void UpdateGamePlay(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();
            float moveAmountForward = 0;
            float moveAmountSideways = 0;

            if (keyState.IsKeyDown(Keys.Right))
            {
                camera.Rotation = MathHelper.WrapAngle(camera.Rotation - (rotateScale * elapsed));
                //  moveAmountSideways = -moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                camera.Rotation = MathHelper.WrapAngle(camera.Rotation + (rotateScale * elapsed));

                //   moveAmountSideways = moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.Up))
            {
                //camera.MoveForward(moveScale * elapsed);
                moveAmountForward = moveScale * elapsed;
            }

            if (keyState.IsKeyDown(Keys.Down))
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
            cube.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameState == GameState.MainMenu)
            {
                drawMenu();
            }
            else if (gameState == GameState.Playing)
            {

                // TODO: Add your drawing code here
                effect.Texture = groundTexture;
                maze.Draw(camera, effect);
                cube.Draw(camera, effect);
            }
            
            base.Draw(gameTime);
        }

        private void drawMenu()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mainMenu, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
        

    }
}
