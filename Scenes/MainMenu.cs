using Barely.SceneManagement;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.Scenes
{
    public class MainMenu : BarelyScene
    {
        Canvas canvas;

        public MainMenu(ContentManager Content, GraphicsDevice GraphicsDevice, Game game) 
            : base(Content, GraphicsDevice, game)
        {
            canvas = new Canvas(Content, Config.Resolution, GraphicsDevice);
            CreateUI();
        }

        private void CreateUI()
        {
            LD44Game g = (LD44Game)game;

            Layout.PushLayout("mainMenu");
            Style.PushStyle("mainMenu");          

            VerticalLayout menu = new VerticalLayout();

            Style.PushStyle("gameName");
            Text name = new Text("gameName");
            Style.PopStyle("gameName");

            Text subtitle = new Text("subtitle");

            Button newGameNormal = new Button("newGame", new Point(200, 40));            
            newGameNormal.OnMouseClick = () => g.ShowNewGame();
            
            Button exit = new Button("exit", new Point(200, 40));            
            exit.OnMouseClick = () => g.Exit();

            Style.PushStyle("tutText");
            Text ld = new Text("ld");
            Text by = new Text("by");
            Text thanks = new Text("thanks");
            Style.PopStyle("tutText");

            Layout.PushLayout("resButtons");
            HorizontalLayout resButtons = new HorizontalLayout();

            Button res1080      = new Button("1920x1080");
            res1080.OnMouseClick = () => ((LD44Game)game).ChangeResolution(1920, 1080);
            Button res900       = new Button("1600x900");
            res900.OnMouseClick = () => ((LD44Game)game).ChangeResolution(1600, 900);
            Button res720       = new Button("1280x720");
            res720.OnMouseClick = () => ((LD44Game)game).ChangeResolution(1280, 720);
            Button fullScreen   = new Button("Fullscreen");
            fullScreen.OnMouseClick = ((LD44Game)game).ToggleFullscreen;

            resButtons.AddChild(res1080, res900, res720, fullScreen);
            Layout.PopLayout("resButtons");

            menu.AddChild(new UIElement[] { name, subtitle, new Space(5), newGameNormal, exit, new Space(5), resButtons, new Space(5), ld, by, thanks});

            Layout.PopLayout("mainMenu");
            Style.PopStyle("mainMenu");

            canvas.AddChild(new UIElement[] { menu });            
            canvas.FinishCreation();
        }

        public override void Initialize()
        {
            
        }

        public override void Update(double deltaTime)
        {
            canvas.HandleInput();
            canvas.Update((float)deltaTime);
        }

        protected override void HandleCameraInput(double deltaTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.uiTransform);
            canvas.Render(spriteBatch);
            spriteBatch.End();
        }
    }
}
