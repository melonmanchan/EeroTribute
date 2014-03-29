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
        private SpriteBatch spriteBatch;

        private SoundEffect selectionChanged;
        private SoundEffect Boom;
        private int selectedButtonIndex;


        public Menu(ContentManager content, SpriteBatch spriteBatch, Game1 game)
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
