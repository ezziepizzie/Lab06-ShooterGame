using GAlgoT2530.Engine;
using Microsoft.VisualBasic;
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

namespace Lab06
{
    public class BlackHoleMissile : SpriteGameObject, ICollidable
    {
        public float Speed = 200f;
        private Vector2 _velocity;
        private Rectangle _rectangle;
        

        private float _gravityRadius = 150f;
        private float _gravityStrength = 500f;
        private float _shipGravityRadius = 250f;
        private float _shipGravityStrength = 250f;
        private Spaceship _spaceship;

        private SoundEffect _gravityPopSoundEffect;
        private SoundEffect _explosionSoundEffect;

        private float _lifetime = 5f;
        private float _activateTime = 0.5f;
        private float _timeSinceSpawn = 0f;

        public BlackHoleMissile() : base("blackHoleMissile")
        {

        }

        public override void LoadContent()
        {
            // Reusing SpriteGameObject.LoadContent() will load the texture;
            base.LoadContent();

            _gravityPopSoundEffect = _game.Content.Load<SoundEffect>("gravity pop");
            _explosionSoundEffect = _game.Content.Load<SoundEffect>("enemyExplosion");
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

            // Listen to non-intersection between Background and BlackHoleMissile objects
            _game.CollisionEngine.Listen(typeof(Background), typeof(BlackHoleMissile), CollisionEngine.NotAABB);

            // Listen to intersection between Asteroid and BlackHoleMissile objects
            _game.CollisionEngine.Listen(typeof(Asteroid), typeof(BlackHoleMissile), CollisionEngine.AABB);
        }

        public override void Update()
        {
            _timeSinceSpawn += ScalableGameTime.DeltaTime;

            if (_timeSinceSpawn >= _activateTime)
            {
                _velocity = Vector2.Zero;

                StartGravityPull();
                PullSpaceship();
            }
            else
            {
                Position += _velocity * ScalableGameTime.DeltaTime;
            }

            if (_timeSinceSpawn >= _lifetime)
            {
                _spaceship?.ResetGravityPull();
                GameObjectCollection.DeInstantiate(this);
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            _game.SpriteBatch.End();
        }

        private void StartGravityPull()
        {
            GameObject[] asteroids = GameObjectCollection.FindObjectsByType(typeof(Asteroid));

            if (asteroids == null) return;

            foreach (GameObject obj in asteroids)
            {
                Asteroid asteroid = obj as Asteroid;

                if (asteroid == null) continue;

                Vector2 direction = Position - asteroid.Position;
                float distance = direction.Length();

                if (distance < _gravityRadius)
                {
                    asteroid.PullTowardsBlackHole(Position, _gravityStrength);
                }
                else
                {
                    asteroid.ReturnToInitialVelocity();
                }
            }
        }

        private void PullSpaceship()
        {
            GameObject spaceshipObject = GameObjectCollection.FindObjectsByType(typeof(Spaceship))?.FirstOrDefault(); // gets the first element
            if (spaceshipObject == null) return;

            _spaceship = spaceshipObject as Spaceship;
            if (_spaceship == null) return;

            Vector2 direction = Position - _spaceship.Position;
            float distance = direction.Length();

            if (distance > _shipGravityRadius)
                return;

            direction.Normalize();

            Vector2 force = direction * _shipGravityStrength * ScalableGameTime.DeltaTime;

            _spaceship.ApplyGravityPull(force);
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
                _gravityPopSoundEffect.Play();
            }

            else if (collisionInfo.Other is Spaceship spaceship)
            {
                _gravityPopSoundEffect.Play();
            }
        }
    }
}
