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
using LD44.Actors;
using static LD44.Actors.Ship;

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
        Ship playerShip;
        List<Ship> enemyShip;

        private State state = State.Normal;  
        Point mouseOverTile = new Point(-1, -1);

        Canvas canvas;
        InputMode inputMode;

        InputMode[] allInputModes;


        public GameScene(ContentManager Content, GraphicsDevice GraphicsDevice, Game game) 
                : base(Content, GraphicsDevice, game)
        {
                        
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
                        allInputModes[(int)type] = new SelectionMode(this);
                        break;
                }
            }
            inputMode = allInputModes[(int)InputModeType.Selection];

            playerShip = new Ship(Assets.ShipBlueprints["player"], new Point(0, Galaxy.Size.Y / 2));

            SetCameraBounds();
            CameraInput.Initialize();

            CreateUI();
        }

        public override void Update(double deltaTime)
        {
            float dt = (float)deltaTime;            

            Tweener.Update(dt);
            UITweener.Update(dt);

            HandleInput(deltaTime);
            CameraInput.HandleCameraInput(camera, deltaTime, uiHandledInput);
            if(!uiHandledInput)
                inputMode.Update(deltaTime);            
                            
            canvas.Update(dt);            
        }

        #region Input

        bool uiHandledInput = false;
        bool hideUI = false;

        private void HandleInput(double deltaTime)
        {
            mouseOverTile = camera.ToWorld(Input.GetMousePosition()) / Galaxy.TileSize;

            uiHandledInput = hideUI ? false : canvas.HandleInput();            
            if (uiHandledInput)
            {                
                if (isDragging && !Input.GetRightMousePressed() && !Input.GetMiddleMousePressed())
                    isDragging = false;
                return;
            }

            if (Input.GetKeyDown(Keys.F10))
            {
                playerShip.ChangeStat(Stats.Fuel, 20);
            }

            if (Input.GetKeyDown(Keys.F11))
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
            camera.SetPosition(new Point(0, Galaxy.Size.Y / 2 * Galaxy.TileSize.Y));
        }

        #endregion



        #region Map/Actor API 

        public void MoveShipTo(Tile target)
        {
            if (playerShip.State == ShipState.Flying)
                return;
            if (target == null)
            {
                Sounds.Play("error");
                return;
            }
            if (playerShip.CanFlyTo(target.Coord))
            {
                playerShip.FlyTo(target.Coord, () => PlayerReached(target));
            } else
            {
                Sounds.Play("missingFuel");
                notifications.AddNotification("missingFuel");
                return;
            }

        }

        private void PlayerReached(Tile target)
        {
            if(target.Type != PlanetType.Empty)
            {
                switch (target.Type)
                {                    
                    case PlanetType.RandomEvent:
                        randomEventScreen.OpenFor(target);
                        break;
                    case PlanetType.Shop:
                        shopScreen.OpenFor(target);
                        break;
                    case PlanetType.EnemyBase:
                        break;
                    case PlanetType.Home:
                        break;
                }
            }
        }

        public int GetFuelCost(Point target)
        {
            return playerShip.GetFuelCostTo(target);
        }

        public void LeaveToMenu()
        {
            ((LD44Game)game).ShowMainMenu();
        }

        bool showIcons = true;

        public void ToggleIcons()
        {
            showIcons = !showIcons;
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
            
        public Point GetMouseOverCoordinate()
        {
            return mouseOverTile;
        }

        public Point GetMousePosition()
        {
            return camera.ToWorld(Input.GetMousePosition());
        }

        public Tile GetMouseOverTile()
        {
            return map.IsInRange(mouseOverTile) ? map[mouseOverTile] : null;
        }

        public Ship GetPlayerShip ()
        {
            return playerShip;
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
            playerShip.Render(spriteBatch);
            
            Effects.Render(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.uiTransform);
            canvas.Render(spriteBatch);
            spriteBatch.End();
        }

        #region UI

        Notifications notifications;
        MouseOverScreen mouseOverScreen;
        ResourceBar resourceBar;

        ShopScreen shopScreen;
        RandomEventScreen randomEventScreen;

        private void CreateUI()
        {
            canvas = new Canvas(Content, Config.Resolution, GraphicsDevice);

            Style.PushStyle("standard");
            Layout.PushLayout("standard");


            Style.PushStyle("resourceBar");
            Layout.PushLayout("resourceBar");
            resourceBar = new ResourceBar(this);
            Style.PopStyle("resourceBar");
            Layout.PopLayout("resourceBar");

            Style.PushStyle("mouseOver");
            Layout.PushLayout("mouseOver");
            mouseOverScreen = new MouseOverScreen(this);
            Style.PopStyle("mouseOver");
            Layout.PopLayout("mouseOver");

            Style.PushStyle("notifications");
            Layout.PushLayout("notifications");
            notifications = new Notifications();
            Style.PopStyle("notifications");
            Layout.PopLayout("notifications");

            Style.PushStyle("planetScreens");
            Layout.PushLayout("planetScreens");

            shopScreen = new ShopScreen(this, canvas);

            randomEventScreen = new RandomEventScreen(this, canvas);

            Style.PopStyle("planetScreens");
            Layout.PopLayout("planetScreens");

            canvas.AddChild(resourceBar, mouseOverScreen, notifications, shopScreen, randomEventScreen);

            canvas.FinishCreation();
        }

        #endregion
    }
}
