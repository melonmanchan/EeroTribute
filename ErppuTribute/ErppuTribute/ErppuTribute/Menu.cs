using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErppuTribute
{

    class Menu
    {
        public Game1 game;

        private Texture2D mainMenu;
        private Texture2D cursor;
        private List<Texture2D> buttons = new List<Texture2D>();
        private List<Texture2D> selectedbuttons = new List<Texture2D>();
        private List<Vector2> buttonPositions = new List<Vector2>();
        private SpriteBatch spriteBatch;

        private SoundEffect selectionChanged;
        private SoundEffect Boom;
        private int selectedButtonIndex;


        private float screenWidth;
        private float screenHeight;

        private Vector2 scaleVector;

        public Menu(ContentManager content, SpriteBatch spriteBatch, Game1 game, GraphicsDevice graphicsDevice)
        {
            this.mainMenu = content.Load<Texture2D>("Eerobg");
            this.cursor = content.Load<Texture2D>("pointer");
            this.spriteBatch = spriteBatch;
            this.game = game;

            selectedbuttons.Add(content.Load<Texture2D>("newButton"));
            selectedbuttons.Add(content.Load<Texture2D>("quitButton"));
            buttons.Add(content.Load<Texture2D>("newButtonJpn"));
            buttons.Add(content.Load<Texture2D>("quitButtonJpn"));

            this.selectionChanged = content.Load<SoundEffect>("selectionChange");
            this.Boom = content.Load<SoundEffect>("Boom");
            selectedButtonIndex = 0;

            //default koko on 1600x1000
            screenWidth = graphicsDevice.Viewport.Width;
            screenHeight = graphicsDevice.Viewport.Height;

            scaleVector = new Vector2(screenWidth / 1600, screenHeight / 1000);
            initializeButtonPositions();
        }

        public void initializeButtonPositions()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Vector2 Pos = new Vector2(screenWidth / 2.37f, screenHeight / 1.66f + i * screenHeight / 6.666f);
                buttonPositions.Add(Pos);
            }

        }

        public void Draw()
        {

            spriteBatch.Begin();
            spriteBatch.Draw(mainMenu, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);

                for (int i = 0; i < buttons.Count; i++)
                {
                    if (i == selectedButtonIndex)
                    {
                        spriteBatch.Draw(selectedbuttons[i], buttonPositions[i], null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);
                        spriteBatch.Draw(cursor, (buttonPositions[i] - new Vector2(80, -30)), null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(buttons[i], buttonPositions[i], null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);
                    }
                }

            spriteBatch.End();
        }

        public void Update()
        {

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Space) || ks.IsKeyDown(Keys.Enter))
            {
                if (selectedButtonIndex == 0)
                {
                    Boom.Play();
                    game.IsMouseVisible = false;
                    game.gameState = GameState.Playing;
                }
                else if (selectedButtonIndex == 1)
                    game.Exit();
            }
            else if (ks.IsKeyDown(Keys.Escape))
            {
                game.Exit();
            }

            else if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
            {
                if (selectedButtonIndex > 0)
                {
                    selectedButtonIndex--;
                    selectionChanged.Play();
                }
            }
            else if (ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
            {
                if (selectedButtonIndex < buttons.Count - 1)
                {
                    selectedButtonIndex++;
                    selectionChanged.Play();
                }
            }
        }
    }
}
