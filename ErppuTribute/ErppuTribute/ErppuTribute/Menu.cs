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
        private List<Texture2D> buttons;
        private List<Texture2D> selectedbuttons;
        private SpriteBatch spriteBatch;

        private SoundEffect selectionChanged;
        private SoundEffect Boom;
        private int selectedButtonIndex;


        public Menu(Texture2D mainMenu, Texture2D cursor, List<Texture2D> buttons, List<Texture2D> selectedbuttons, SpriteBatch spriteBatch, SoundEffect selectionChanged, SoundEffect Boom, Game1 game)
        {
            this.mainMenu = mainMenu;
            this.cursor = cursor;
            this.spriteBatch = spriteBatch;
            this.game = game;
            this.buttons = buttons;
            this.selectedbuttons = selectedbuttons;
            this.selectionChanged = selectionChanged;
            this.Boom = Boom;
            selectedButtonIndex = 0;
        }

        public void Draw()
        {
            spriteBatch.Begin();

                spriteBatch.Draw(mainMenu, Vector2.Zero, Color.White);

                for (int i = 0; i < buttons.Count; i++)
                {
                    Vector2 Pos = new Vector2(675, 600 + i * 150);
                    if (i == selectedButtonIndex)
                    {
                        spriteBatch.Draw(selectedbuttons[i], Pos, Color.White);
                        spriteBatch.Draw(cursor, (Pos - new Vector2(80, -30)), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(buttons[i], Pos, Color.White);
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
