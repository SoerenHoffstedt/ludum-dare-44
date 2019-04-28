using Barely.Util;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LD44.Actors
{
    public class Ship
    {
        private const int UNITS_PER_FUEL = 10;
        private const float BASE_SPEED = 40f;
        private const float SPEED_ADDED_PER_STAT = 10f;

        public int X { get { return WorldPosition.X; } set { WorldPosition.X = value; } }
        public int Y { get { return WorldPosition.Y; } set { WorldPosition.Y = value; } }

        public Point TilePosition { get { return new Point(WorldPosition.X / Galaxy.Size.X, WorldPosition.Y / Galaxy.Size.Y); } }
        public Point WorldPosition;
        public Point Size = new Point(16, 16);
        public string Name;
        public ShipState State { get; private set; }
        public Sprite Sprite;
        public ShipFaction Faction { get; private set; }
        private int[] stats;

        private Sprite thrustSprite;
        private Sprite highlightSprite;
        private float rotation = 0f;
        private SoundEffectInstance moveSoundEffect;
        private GameStats gameStats;

        public Ship(ShipBlueprint blueprint, Point startCoord, GameStats gameStats)
        {
            Name = blueprint.Name;
            Faction = blueprint.Faction;

            stats = new int[(int)Stats.Count];
            foreach(Stats stat in Enum.GetValues(typeof(Stats)))
            {
                if (stat != Stats.Count)
                    stats[(int)stat] = blueprint.stats[(int)stat];
            }
            
            WorldPosition = startCoord * Galaxy.TileSize;
            State = ShipState.Idle;
            Sprite = Assets.ShipSprites[blueprint.SpriteId];
            highlightSprite = Assets.OtherSprites["shipHighlight"];
            thrustSprite = Assets.OtherSprites["shipThrust"];

            moveSoundEffect = Sounds.Get("shipMove").CreateInstance();
            moveSoundEffect.IsLooped = true;

            if (Faction == ShipFaction.Player)
                this.gameStats = gameStats;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if(Faction == ShipFaction.Player)
                highlightSprite.Render(spriteBatch, WorldPosition + new Point(-2, 2));

            if(State == ShipState.Flying)                            
                spriteBatch.Draw(thrustSprite.atlas, new Rectangle(WorldPosition + thrustSprite.drawOffset + new Point(8,8), thrustSprite.spriteRect.Size), thrustSprite.spriteRect, Sprite.color, rotation, new Vector2(16, 8), SpriteEffects.None, 0f);            

            spriteBatch.Draw(Sprite.atlas, new Rectangle(WorldPosition + Sprite.drawOffset + new Point(8,8), Sprite.spriteRect.Size), Sprite.spriteRect, Sprite.color, rotation, new Vector2(8,8), SpriteEffects.None, 0f);
        }

        public int GetStat(Stats s)
        {
            return stats[(int)s];
        }

        public void ChangeStat(Stats s, int change)
        {
            stats[(int)s] += change;
            if (stats[(int)s] < 0)
                stats[(int)s] = 0;
        }

        const int RANGE_PER_STAT = 4;

        public int GetScanningRange()
        {
            return RANGE_PER_STAT * GetStat(Stats.Scanning);
        }

        public int GetFuelCostTo(Point targetCoord)
        {
            float dist = DistanceTo(targetCoord);
            return GetFuelCost(dist);
        }

        public float DistanceTo(Point targetCoord)
        {
            targetCoord *= Galaxy.TileSize;
            float dist = (WorldPosition - targetCoord).ToVector2().Length();
            return dist;
        }

        public int GetFuelCost(float dist)
        {            
            return (int)dist / UNITS_PER_FUEL;
        }

        public bool CanFlyTo(Point targetCoord)
        {
            return GetStat(Stats.Fuel) >= GetFuelCostTo(targetCoord);
        }
        
        public void StopFly()
        {
            GameScene.Tweener.Tween(this, new { X = WorldPosition.X, Y = WorldPosition.Y }, 0f);
        }

        public void FlyTo(Point targetCoord, Action<float> OnReached)
        {
            if (targetCoord * Galaxy.TileSize == WorldPosition)
            {
                OnReached(0f);
                return;
            }

            Vector2 dir = (targetCoord * Galaxy.TileSize - WorldPosition).ToVector2();
            dir.Normalize();            
            float newRotation = (float)Math.Atan2(dir.Y, dir.X);           

            float dist = DistanceTo(targetCoord);
            int fuelCost = GetFuelCost(dist);
            ChangeStat(Stats.Fuel, -fuelCost);

            if (gameStats != null) {
                gameStats.FuelBurned += fuelCost;
                gameStats.DistanceTraveled += dist;
            }
            

            State = ShipState.Flying;
            targetCoord *= Galaxy.TileSize;
            float speed = GetSpeed();
            GameScene.Tweener.Tween(this, new { X = targetCoord.X, Y = targetCoord.Y }, dist / speed).OnComplete(FlyFinished).OnComplete(() => OnReached(dist/speed));
            GameScene.Tweener.Tween(this, new { rotation = newRotation }, dist / speed / 4);

            moveSoundEffect.IsLooped = true;
            moveSoundEffect.Play();            
        }        

        public float GetSpeed()
        {
            return BASE_SPEED + SPEED_ADDED_PER_STAT * GetStat(Stats.Speed);
        }

        private void FlyFinished()
        {
            State = ShipState.Idle;
            moveSoundEffect.IsLooped = false;
        }        

    }

    public enum ShipFaction
    {
        Player,
        Enemy
    }

    public enum ShipState
    {
        Flying,
        Idle
    }

    public class ShipBlueprint
    {
        public string Id;
        public string Name;
        public ShipFaction Faction;
        public int SpriteId;
        public int[] stats;

        public ShipBlueprint(XmlNode node)
        {
            Id = node.Attributes["id"].Value;
            Name = node.Attributes["name"].Value;
            Faction = (ShipFaction)Enum.Parse(typeof(ShipFaction), node.Attributes["faction"].Value);
            SpriteId = int.Parse(node.Attributes["sprite"].Value);

            XmlNode statNode = node.SelectSingleNode("stats");
            stats = new int[(int)Stats.Count];
            stats[(int)Stats.Damage]    = int.Parse(statNode.Attributes["Damage"].Value);
            stats[(int)Stats.Defense]   = int.Parse(statNode.Attributes["Defense"].Value);
            stats[(int)Stats.Fuel]      = int.Parse(statNode.Attributes["Fuel"].Value);
            stats[(int)Stats.Health]    = int.Parse(statNode.Attributes["Health"].Value);            
            stats[(int)Stats.Speed]     = int.Parse(statNode.Attributes["Speed"].Value);
            stats[(int)Stats.Scanning]  = int.Parse(statNode.Attributes["Scanning"].Value);

        }
    }
}
