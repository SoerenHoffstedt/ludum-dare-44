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
            Normal,
            EnemyFly
        }

        public static Tweener Tweener = new Tweener();
        public static Tweener UITweener = new Tweener();

        private Galaxy map;
        Ship playerShip;
        List<Ship> enemyShips;
        const int ENEMY_COUNT = 3;
        const int ENEMY_MOVE = 10;
        const int ENEMY_ATTACK_RANGE = 2 * 16;
        const int ENEMY_ATTACK_RANGE_SQ = ENEMY_ATTACK_RANGE * ENEMY_ATTACK_RANGE;

        public State state = State.Normal;  
        Point mouseOverTile = new Point(-1, -1);

        Canvas canvas;
        InputMode inputMode;

        InputMode[] allInputModes;

        Song song;

        public GameStats gameStats = new GameStats();


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

            playerShip = new Ship(Assets.ShipBlueprints["player0"], new Point(0, Galaxy.Size.Y / 2), gameStats);

            enemyShips = new List<Ship>(ENEMY_COUNT);
            Point p = new Point(-15, 5);
            for(int i = 0; i < ENEMY_COUNT; ++i)
            {                
                Ship e = new Ship(Assets.ShipBlueprints[$"enemy{i}"], p, null);
                enemyShips.Add(e);
                p.Y += 5;
            }


            SetCameraBounds();
            CameraInput.Initialize();

            CreateUI();

            mouseOverPlanetSprite = Assets.OtherSprites["mouseOverPlanet"];
            mouseOverSprite = Assets.OtherSprites["mouseOver"];
            song = Content.Load<Song>("Sounds/song");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = soundVolume;
            MediaPlayer.Play(song);
        }

        public override void Update(double deltaTime)
        {
            float dt = (float)deltaTime;            

            Tweener.Update(dt);
            UITweener.Update(dt);

            UpdateFlyingEnemies();                       

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

            if (Input.GetKeyDown(Keys.Tab))
            {
                float time = (camera.position - playerShip.WorldPosition.ToVector2()).Length() / 768f;
                Tweener.Tween(camera, new { posX = playerShip.WorldPosition.X, posY = playerShip.WorldPosition.Y }, time);                
            }

            if (Input.GetKeyDown(Keys.LeftShift))
            {
                Vector2 targetPos = (map.homePlanet.Coord * Galaxy.TileSize).ToVector2();
                float time = (camera.position - targetPos).Length() / 768f;
                Tweener.Tween(camera, new { posX = targetPos.X, posY = targetPos.Y }, time);
            }

            if (Input.GetKeyDown(Keys.H))
            {
                helpScreen.Open();
            }

            if (Input.GetKeyDown(Keys.F))
            {
                ToggleIcons();
            }

            if (Input.GetKeyDown(Keys.M))
            {
                ToggleMusic();
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

        public void ShowHelp()
        {
            helpScreen.Open();
        }

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
                playerShip.FlyTo(target.Coord, (time) => PlayerReached(target, time));
            } else
            {
                Sounds.Play("missingFuel");
                notifications.AddNotification("missingFuel");
                return;
            }

        }

        private void PlayerReached(Tile target, float time)
        {
            if(target.Type != PlanetType.Empty)
            {
                gameStats.PlanetsVisited += 1;
                switch (target.Type)
                {                                        
                    case PlanetType.RandomEvent:
                        gameStats.RandomEvents += 1;
                        randomEventScreen.OpenFor(target);
                        break;
                    case PlanetType.Shop:
                        gameStats.ShopsVisited += 1;
                        shopScreen.OpenFor(target);
                        break;
                    case PlanetType.EnemyBase:
                        gameStats.BattlesFought += 1;
                        CloseAnyOtherWindow();
                        battleScreen.OpenFor(target);
                        break;
                    case PlanetType.Home:
                        Won();
                        break;
                }
            } else if(playerShip.GetStat(Stats.Fuel) == 0)
            {
                GameOver(false);
            }

            if(time > 0f)
            {
                MoveEnemyShips(time);
            }

        }

        void MoveEnemyShips(float time)
        {
            state = State.EnemyFly;

            //camera.SetPosition(enemyShips[enemyShips.Count / 2 + enemyShips.Count % 2].WorldPosition);

            Point targetTween = Point.Zero;

            foreach(Ship e in enemyShips)
            {
                float speed = e.GetSpeed();
                float dist = time * speed;

                targetTween = e.WorldPosition + new Point((int)dist, 0);
                Point coord = targetTween / Galaxy.TileSize;
                e.FlyTo(coord, (t) => EnemyReached(null, t));
            }

            //Tweener.Tween(camera, new { posX = targetTween.X }, time / 2f);

        }

        void EnemyReached(Tile target, float time)
        {
            state = State.Normal;
            foreach (Ship e in enemyShips)
            {
                if (e.State == ShipState.Flying)
                    state = State.EnemyFly;
            }
        }

        void UpdateFlyingEnemies()
        {
            if (state == State.EnemyFly)
            {
                foreach (Ship enemy in enemyShips)
                {
                    float distSq = (enemy.WorldPosition - playerShip.WorldPosition).ToVector2().LengthSquared();
                    if (distSq < ENEMY_ATTACK_RANGE_SQ)
                    {
                        state = State.Normal;
                        CloseAnyOtherWindow();
                        battleScreen.OpenFor(enemy, (ship) => enemyShips.Remove(ship));
                        foreach (Ship e in enemyShips)
                        {
                            e.StopFly();
                        }
                    }
                }
            }
        }

        public void StartBattle(string enemyShipBlueprintId)
        {
            var ship = new Ship(Assets.ShipBlueprints[enemyShipBlueprintId], Point.Zero, null);
            CloseAnyOtherWindow();
            battleScreen.OpenFor(ship, null);
        }

        public int GetFuelCost(Point target)
        {
            return playerShip.GetFuelCostTo(target);
        }

        public Point GetPlayerShipPosition()
        {
            return playerShip.WorldPosition;
        }

        public bool IsInPlayerScanningRange(Point worldPos)
        {
            int range = GetPlayerShipScanningRange() * Galaxy.TileSize.X;
            range *= range;
            float dist = (GetPlayerShipPosition() - worldPos).ToVector2().LengthSquared();
            return dist <= range;
        }

        public int GetPlayerShipScanningRange()
        {
            return playerShip.GetScanningRange();
        }

        public void Won()
        {
            endScreen.Open(new GameEndInfo() {
                success = true,
                gameStats = gameStats
            });
        }

        public void GameOver(bool lostBattle)
        {
            endScreen.Open(new GameEndInfo()
            {
                success = false,
                lostBattle = lostBattle,
                noFuel = !lostBattle,
                gameStats = gameStats
            });
        }

        public void LeaveToMenu()
        {
            ((LD44Game)game).ShowMainMenu();
        }

        bool showIcons = true;

        public void ToggleIcons()
        {
            showIcons = !showIcons;
            if (showIcons)
                resourceBar.iconButton.sprite = Assets.OtherSprites["iconToggleOn"];
            else
                resourceBar.iconButton.sprite = Assets.OtherSprites["iconToggleOff"];
        }

        float soundVolume = 0.4f;

        public void ToggleMusic()
        {
            if(MediaPlayer.Volume == 0.0f)
                MediaPlayer.Volume = soundVolume;
            else
                MediaPlayer.Volume = 0.0f;


            if (MediaPlayer.Volume == 0.0f)
                resourceBar.soundButton.sprite = Assets.OtherSprites["soundOff"];
            else
                resourceBar.soundButton.sprite = Assets.OtherSprites["soundOn"];
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

        Sprite mouseOverPlanetSprite;
        Sprite mouseOverSprite;


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.Transform);
            map.Render(spriteBatch, showIcons);
            playerShip.Render(spriteBatch);

            foreach(Ship s in enemyShips)
            {
                s.Render(spriteBatch);
            }

            Tile t = GetMouseOverTile();
            if(t != null)
            {
                if(t.Type != PlanetType.Empty)                
                    mouseOverPlanetSprite.Render(spriteBatch, t.Coord * Galaxy.TileSize);
                else                
                    mouseOverSprite.Render(spriteBatch, t.Coord * Galaxy.TileSize);                
            }

            Effects.Render(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.uiTransform);
            canvas.Render(spriteBatch);
            spriteBatch.End();
        }

        #region UI

        void CloseAnyOtherWindow()
        {
            shopScreen.Close();
            randomEventScreen.Close();
            battleScreen.Close();
            helpScreen.Close();            
        }

        NotificationBar notifications;
        MouseOverBar mouseOverScreen;
        ResourceBar resourceBar;
        TutorialBar tutorialBar;

        ShopScreen shopScreen;
        RandomEventScreen randomEventScreen;
        BattleScreen battleScreen;
        HelpScreen helpScreen;
        EndScreen endScreen;
        IntroductionScreen introScreen;

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
            mouseOverScreen = new MouseOverBar(this);
            Style.PopStyle("mouseOver");
            Layout.PopLayout("mouseOver");

            Style.PushStyle("tutorial");
            Layout.PushLayout("tutorial");
            tutorialBar = new TutorialBar(this);
            Style.PopStyle("tutorial");
            Layout.PopLayout("tutorial");

            Style.PushStyle("notifications");
            Layout.PushLayout("notifications");
            notifications = new NotificationBar();
            Style.PopStyle("notifications");
            Layout.PopLayout("notifications");

            Style.PushStyle("planetScreens");
            Layout.PushLayout("planetScreens");

            shopScreen = new ShopScreen(this, canvas);

            randomEventScreen = new RandomEventScreen(this, canvas);

            battleScreen = new BattleScreen(this, canvas);

            helpScreen = new HelpScreen(canvas);

            endScreen = new EndScreen(this, canvas);

            introScreen = new IntroductionScreen(canvas);

            Style.PopStyle("planetScreens");
            Layout.PopLayout("planetScreens");

            canvas.AddChild(resourceBar, mouseOverScreen, notifications, tutorialBar, shopScreen, randomEventScreen, battleScreen, helpScreen, endScreen, introScreen);

            canvas.FinishCreation();
        }

        #endregion
    }

}
