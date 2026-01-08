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

// Acknowledgement / Honour Code:
// - Codes are written by going through available classes such as GameObjectCollection.cs
//   to see what functions are available to use.
// - Visual Studio's autocomplete feature sometimes provides insights on how to approach the
//   intended functionality.

// Missile 1 (Orbiter Missile): OrbiterMissile.cs

// - Press Left Click to shoot this missile.
// - The missile will orbit the spaceship and move accordingly.
// - Any asteroids hit will be destroyed.
// - The missile will be destroyed after 10 seconds.
// - Only one orbiter missile can be shot at a time.

namespace Lab06
{
    public class OrbiterMissile : SpriteGameObject, ICollidable
    {
        public float Speed = 200f;
        private Vector2 _velocity;
        private Rectangle _rectangle;

        private Spaceship _spaceship;

        private SoundEffect _explosionSoundEffect;
        private SoundEffect _asteroidDestroyedSoundEffect;

        private float _orbitRadius = 150f;
        private float _angle;
        private float _angularSpeed = 7f;
        private float _lifetime = 10f;
        private float _timeSinceSpawn = 0f;

        public OrbiterMissile() : base("spaceMissiles_015_right")
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
            _angle = 0f;
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

            _spaceship = GameObjectCollection.FindObjectsByType(typeof(Spaceship))?.FirstOrDefault() as Spaceship;

            // Listen to non-intersection between Background and OrbiterMissile objects
            _game.CollisionEngine.Listen(typeof(Background), typeof(OrbiterMissile), CollisionEngine.NotAABB);

            // Listen to intersection between Asteroid and OrbiterMissile objects
            _game.CollisionEngine.Listen(typeof(Asteroid), typeof(OrbiterMissile), CollisionEngine.AABB);
        }

        public override void Update()
        {
            _timeSinceSpawn += ScalableGameTime.DeltaTime;
            
            if (_timeSinceSpawn >= _lifetime)
            {
                GameObjectCollection.DeInstantiate(this);
                return;
            }

            _angle += _angularSpeed * ScalableGameTime.DeltaTime;

            Vector2 offset = new Vector2(MathF.Cos(_angle), MathF.Sin(_angle)) * _orbitRadius;

            Position = _spaceship.Position + offset;

            // face movement direction
            Orientation = _angle + MathF.PI / 2f;
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            _game.SpriteBatch.End();
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
            }
        }
    }
}
