using GAlgoT2530.Engine;
using Lab06;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Acknowledgement / Honour Code:
// - Codes are written by going through available classes such as GameObjectCollection.cs
//   to see what functions are available to use.
// - Visual Studio's autocomplete feature sometimes provides insights on how to approach the
//   intended functionality.

//Missile 1 (Ricochet Missile): RichochetMissile.cs

// - Press Left Click to shoot this missile.
// - After the first hit, the missile will ricochet/move towards the next nearest asteroid in a
//   certain radius.
// - The missile has a maximum kill count of 3, so after hitting 3 asteroids, it will destroy itself.
// - This missile rewards the players for hitting an asteroid.

namespace Lab06
{
    public class RicochetMissile : SpriteGameObject, ICollidable
    {
        public float Speed = 200f;
        private Vector2 _velocity;
        private Rectangle _rectangle;

        private SoundEffect _explosionSoundEffect;
        private SoundEffect _asteroidDestroyedSoundEffect;

        private float _richochetRadius = 150f;
        private int _killCount = 0;
        private int _maxKillCount = 3;
        private bool _hasHit = false;
        
        public RicochetMissile() : base("spaceMissiles_015_right")
        {

        }

        public override void LoadContent()
        {
            // Reusing SpriteGameObject.LoadContent() will load the texture;
            base.LoadContent();

            _explosionSoundEffect = _game.Content.Load<SoundEffect>("enemyExplosion");
            _asteroidDestroyedSoundEffect = _game.Content.Load<SoundEffect>("asteroidDestroyed");
        }

        public override void Initialize()
        {
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;

            // Assumption : Orientation has been set before Initialize() is called
            Vector2 facingDirection = new Vector2
            {
                X = (float)MathF.Cos(Orientation),
                Y = (float)MathF.Sin(Orientation)
            };

            _velocity = facingDirection * Speed;

            _rectangle.Location = Position.ToPoint();
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;

            // Listen to non-intersection between Background and RicochetMissile objects
            _game.CollisionEngine.Listen(typeof(Background), typeof(RicochetMissile), CollisionEngine.NotAABB);

            // Listen to non-intersection between Asteroid and RicochetMissile objects
            _game.CollisionEngine.Listen(typeof(Asteroid), typeof(RicochetMissile), CollisionEngine.AABB);
        }

        public override void Update()
        {
            Position += _velocity * ScalableGameTime.DeltaTime;

            if(_hasHit)
            {
                StartRichochet();

                if (_killCount >= _maxKillCount)
                {
                    GameObjectCollection.DeInstantiate(this);
                }
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            _game.SpriteBatch.End();
        }

        private Asteroid FindNearestAsteroid()
        {
            GameObject[] asteroids = GameObjectCollection.FindObjectsByType(typeof(Asteroid));

            if (asteroids == null) 
                return null;

            Asteroid nearestAsteroid = null;

            foreach (GameObject obj in asteroids)
            {
                Asteroid asteroid = obj as Asteroid;

                if (asteroid == null) continue;

                float distance = Vector2.Distance(Position, asteroid.Position);

                if (distance <= _richochetRadius)
                    nearestAsteroid = asteroid;
            }

            return nearestAsteroid;
        }

        private void StartRichochet()
        {
            Asteroid targetAsteroid = FindNearestAsteroid();

            if (targetAsteroid != null)
            {
                Speed += 20f;

                Vector2 directionToTarget = Vector2.Normalize(targetAsteroid.Position - Position);
                _velocity = directionToTarget * Speed;
                Orientation = MathF.Atan2(directionToTarget.Y, directionToTarget.X);
            }
        }

        string ICollidable.GetGroupName()
        {
            return this.GetType().Name;
        }

        Rectangle ICollidable.GetBound()
        {
            _rectangle.Location = Position.ToPoint();
            return _rectangle;
        }

        void ICollidable.OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Other is Background)
            {
                GameObjectCollection.DeInstantiate(this);
                _explosionSoundEffect.Play();
            }

            else if (collisionInfo.Other is Asteroid asteroid)
            {
                GameObjectCollection.DeInstantiate(asteroid);
                _asteroidDestroyedSoundEffect.Play();

                _hasHit = true;
                _killCount++;
            }
        }
    }
}
