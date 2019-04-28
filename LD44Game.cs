﻿using Barely.SceneManagement;
using Barely.Util;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;

namespace LD44
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class LD44Game : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BarelyScene currScene = null;

        public LD44Game()
        {
            Config.LoadFromDisc("Content/config.xml");
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = Config.Resolution.X;
            graphics.PreferredBackBufferHeight = Config.Resolution.Y;
            graphics.IsFullScreen = Config.Fullscreen;

            graphics.ApplyChanges();
            SpriteBatchEx.GraphicsDevice = GraphicsDevice;

            Assets.Load(Content);

            XmlDocument soundsXml = new XmlDocument();
            soundsXml.Load("Content/Sounds/sounds.xml");
            Sounds.Initialize(Content, soundsXml);

            XmlDocument lang = new XmlDocument();
            lang.Load("Content/Language/en.xml");
            Texts.SetTextFile(lang);

            Style.InitializeStyle("Content/uiStyle.xml", Content);
            Layout.InitializeLayouts("Content/uiLayout.xml");

            XmlDocument effects = new XmlDocument();
            effects.Load("Content/effects.xml");
            Effects.Initialize(effects, Assets.EffectsTexture);

            Window.Title = Texts.Get("windowTitle");
        }


        protected override void Initialize()
        {
            ShowNewGame();
            //ShowMainMenu();
            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }


        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            double dt = gameTime.ElapsedGameTime.TotalSeconds;

            Input.Update();
            Timer.UpdateAll((float)dt);
            Effects.Update(dt);
            Animator.Update(dt);
            currScene.Update(dt);

            base.Update(gameTime);
        }        

        public void ShowNewGame()
        {
            MediaPlayer.Stop();
            currScene = new GameScene(Content, GraphicsDevice, this);
            currScene.Initialize();
            Sounds.Reset();
            Effects.Reset();
        }

        public void ShowMainMenu()
        {
            MediaPlayer.Stop();
            currScene = new MainMenu(Content, GraphicsDevice, this);
            currScene.Initialize();
            Sounds.Reset();
            Effects.Reset();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(56, 50, 80));
            double dt = gameTime.ElapsedGameTime.TotalMilliseconds;
            currScene.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
