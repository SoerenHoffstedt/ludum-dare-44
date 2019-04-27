using Glide;
using System;
using System.Collections.Generic;
using System.Linq;
using Barely.Util;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using LD44.World;
using LD44.UI;
using LD44.InputModes;

namespace LD44.Scenes
{
    public class GameScene : Barely.SceneManagement.BarelyScene
    {
        public enum State
        {
            Normal
        }

        public static Tweener Tweener = new Tweener();
        public static Tweener UITweener = new Tweener();

        private Galaxy map;            
        private State state = State.Normal;  
        Point mouseOverTile = new Point(-1, -1);

        Canvas canvas;
        InputMode inputMode;

        InputMode[] allInputModes;


        public GameScene(ContentManager Content, GraphicsDevice GraphicsDevice, Game game) 
                : base(Content, GraphicsDevice, game)
        {
            
            CreateUI();
        }

        public override void Initialize()
        {
            map = new Galaxy(this);

            allInputModes = new InputMode[(int)InputModeType.Count];
            foreach (InputModeType type in Enum.GetValues(typeof(InputModeType)))
            {
                switch (type)
                {
                    case InputModeType.Selection:
                        allInputModes[(int)type] = new SelectionMode(map);
                        break;
                }
            }
            inputMode = allInputModes[(int)InputModeType.Selection];

            SetCameraBounds();
            CameraInput.Initialize();
        }

        public override void Update(double deltaTime)
        {
            float dt = (float)deltaTime;            

            Tweener.Update(dt);
            UITweener.Update(dt);

            HandleInput(deltaTime);
            CameraInput.HandleCameraInput(camera, deltaTime, cameraTakesInput);
            inputMode.Update(deltaTime);            
                            
            canvas.Update(dt);            
        }

        #region Input

        bool cameraTakesInput = true;
        bool hideUI = false;

        private void HandleInput(double deltaTime)
        {
            bool handled = hideUI ? false : canvas.HandleInput();
            cameraTakesInput = true;
            if (handled)
            {
                cameraTakesInput = false;
                if (isDragging && !Input.GetRightMousePressed() && !Input.GetMiddleMousePressed())
                    isDragging = false;
                return;
            }            

            if (Input.GetKeyDown(Keys.F12))
            {
                hideUI = !hideUI;
            }

            if (Input.GetKeyDown(Keys.F12))
            {
                Canvas.DRAW_DEBUG = !Canvas.DRAW_DEBUG;
            }

        }
                  
        protected override void HandleCameraInput(double deltaTime)
        {
            
        }

        void SetCameraBounds()
        {            
            camera.SetMinMaxPosition(new Vector2(0, 0), Galaxy.Size.ToVector2() * Galaxy.TileSize.ToVector2());
        }

        #endregion



        #region Map/Actor API 

        public void LeaveToMenu()
        {
            ((LD44Game)game).ShowMainMenu();
        }

        float soundVolume = 0.4f;

        public void ToggleMusic()
        {
            if(MediaPlayer.Volume == 0.0f)
                MediaPlayer.Volume = soundVolume;
            else
                MediaPlayer.Volume = 0.0f;
        }

        public void ShowNotification(string text, bool playNotificationSound = true)
        {
            notifications.AddNotification(text);
            if(playNotificationSound)
                Sounds.Play("notification");
        }
            
        public Point GetMouseOverTile()
        {
            return mouseOverTile;
        }

        public Tile GetTile(Point p)
        {
            return map.IsInRange(p) ? map[p] : null;
        }       

        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.Transform);
            map.Render(spriteBatch);

            //...

            Effects.Render(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.uiTransform);
            canvas.Render(spriteBatch);
            spriteBatch.End();
        }

        #region UI

        Notifications notifications;

        private void CreateUI()
        {
            canvas = new Canvas(Content, Config.Resolution, GraphicsDevice);

            Style.PushStyle("standard");
            Layout.PushLayout("standard");

            //...           
            
            canvas.FinishCreation();
        }

        #endregion
    }
}
