/****************************************************
 * Class: Menu
 * Description: Main menu of the game
 * Author(s): Matti Jokitulppo, Jonah Ahvonen
 * Date: April 1, 2014
****************************************************/

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
        private List<Texture2D> selectedButtons = new List<Texture2D>();
        private List<Vector2> buttonPositions = new List<Vector2>();
        private SpriteBatch spriteBatch;

        private SoundEffect selectionChanged;
        private SoundEffect Boom;
        private int selectedButtonIndex;
        public MouseState ms;

        private float screenWidth;
        private float screenHeight;

        private Vector2 scaleVector;

        public Menu(ContentManager content, SpriteBatch spriteBatch, Game1 game, GraphicsDevice graphicsDevice)
        {
            this.mainMenu = content.Load<Texture2D>("Eerobg");
            this.cursor = content.Load<Texture2D>("pointer");
            this.spriteBatch = spriteBatch;
            this.game = game;

            selectedButtons.Add(content.Load<Texture2D>("newButton"));
            selectedButtons.Add(content.Load<Texture2D>("quitButton"));
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

        private void initializeButtonPositions()
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
                        spriteBatch.Draw(selectedButtons[i], buttonPositions[i], null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);
                        spriteBatch.Draw(cursor, (buttonPositions[i] - new Vector2(80, -30)), null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(buttons[i], buttonPositions[i], null, Color.White, 0, Vector2.Zero, scaleVector, SpriteEffects.None, 0);
                    }
                }

            spriteBatch.End();
        }


        private void handleMouseInput()
        {
            ms = Mouse.GetState();

                Rectangle mouseClickRect = new Rectangle(ms.X, ms.Y, 10, 10);

                for (int i = 0; i < buttonPositions.Count; i++)
                {

                    Rectangle tempRect = new Rectangle((int)buttonPositions[i].X, (int)buttonPositions[i].Y, Convert.ToInt32(screenWidth / 6.4f), Convert.ToInt32(screenHeight / 10.2f));

                    if (tempRect.Contains(mouseClickRect))
                    {
                        if (ms.LeftButton == ButtonState.Pressed)
                        {
                            handleSelection(i);
                        }
                        else
                        {
                            if (selectedButtonIndex != i)
                                selectionChanged.Play();
                            selectedButtonIndex = i;
                        }
                    }
                }
        }

        private void handleSelection(int selected)
        {
            if (selected == 0)
            {
                Boom.Play();
                game.centerMouse();
                game.IsMouseVisible = false;
                game.gameState = GameState.PlayingVideo;
            }
            else if (selected == 1)
                game.Exit();
        }

        private void handleKeyBoardInput()
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Space) || ks.IsKeyDown(Keys.Enter))
            {

                handleSelection(selectedButtonIndex);

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

        public void Update()
        {
            handleMouseInput();
            handleKeyBoardInput();
        }
    }
}
