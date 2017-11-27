﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using DwarfWars.Library;
using Lidgren.Network;
using System;

namespace DwarfWars
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ClientGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Client client;
        Thread readingThread;
        KeyboardState current;
        int lastIteration;
        SpriteFont font;


        public ClientGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            client = new Client();
            readingThread = new Thread(client.ReadMessages);
            readingThread.Start();
            current = Keyboard.GetState();
            lastIteration = -1;
            IsMouseVisible = true;
            
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
            font = Content.Load<SpriteFont>("File");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            current = Keyboard.GetState();

            var seconds = gameTime.TotalGameTime.Seconds;
            if(seconds > lastIteration)
            {
                if (current.IsKeyDown(Keys.Up))
                {
                    client.Movement("U");
                    lastIteration = seconds;
                }
                else if (current.IsKeyDown(Keys.Down))
                {
                    client.Movement("D");
                    lastIteration = seconds;

                }
                else if (current.IsKeyDown(Keys.Left))
                {
                    client.Movement("L");
                    lastIteration = seconds;

                }
                else if (current.IsKeyDown(Keys.Right))
                {
                    client.Movement("R");
                    lastIteration = seconds;

                }
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Texture2D temp = new Texture2D(GraphicsDevice, 1, 1);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (ClientPlayer p in client.allPlayers)
            {
                Color color;
                if (p.IsClient)
                {
                    color = Color.Green;
                }
                else
                {
                    color = Color.Red;
                }
                spriteBatch.Draw(temp, new Rectangle(p.XPos, p.YPos, 100, 100), color);
            }
            spriteBatch.DrawString(font, client.allPlayers.Count.ToString(), new Vector2(0,0), Color.Black);
            base.Draw(gameTime);
            spriteBatch.End();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            readingThread.Abort();
            base.OnExiting(sender, args);
        }
    }
}