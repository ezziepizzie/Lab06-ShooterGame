using GAlgoT2530.Engine;
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
    public class Spaceship : SpriteGameObject, ICollidable
    {
        public float Speed;
        public float FiringRate = 1f; // Missiles per second
        public float CoolDownTime;
        public float LastFiredTime;

        /*private Vector2 _leftGunPoint;
        private Vector2 _rightGunPoint;
        private Vector2 _leftGunDirectionFromCentre;
        private Vector2 _rightGunDirectionFromCentre;
        private Vector2 _leftGunVector;
        private Vector2 _rightGunVector;*/

        private Vector2 _gravityPull;

        private Rectangle _rectangle;
        public ControllableMissile _activeMissile;

        private SoundEffect _ricochetMissileSoundEffect;
        private SoundEffect _blackHoleMissileSoundEffect;
        private SoundEffect _spaceshipDeathSoundEffect;

        public Spaceship(string textureName) : base(textureName)
        {

        }

        public override void LoadContent()
        {
            // Reusing SpriteGameObject.LoadContent() will load the texture;
            base.LoadContent();

            _ricochetMissileSoundEffect = _game.Content.Load<SoundEffect>("ricochetShot");
            _blackHoleMissileSoundEffect = _game.Content.Load<SoundEffect>("blackHoleShot");
            _spaceshipDeathSoundEffect = _game.Content.Load<SoundEffect>("spaceshipDeath");
        }

        public override void Initialize()
        {
            Origin.X = Texture.Width / 2;
            Origin.Y = Texture.Height / 2;

            Position.X = _game.Graphics.PreferredBackBufferWidth / 2;
            Position.Y = _game.Graphics.PreferredBackBufferHeight / 2;

            CoolDownTime = 1f / FiringRate;
            LastFiredTime = 0f;

            /*_leftGunPoint.X = 82;
            _leftGunPoint.Y = 20;
            _rightGunPoint.X = 82;
            _rightGunPoint.Y = 94;

            _leftGunDirectionFromCentre = _leftGunPoint - Origin;
            _rightGunDirectionFromCentre = _rightGunPoint - Origin;*/

            _rectangle.Location = Position.ToPoint();
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;

            // Listen to non-intersection between Asteroid and Spaceship objects
            _game.CollisionEngine.Listen(typeof(Asteroid), typeof(Spaceship), CollisionEngine.AABB);
            _game.CollisionEngine.Listen(typeof(Background), typeof(Spaceship), CollisionEngine.NotAABB);
        }

        public override void Update()
        {
            // Determine movement direction based on keyboard input
            Vector2 movementDirection = Vector2.Zero;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                movementDirection.Y = -1f;
            }

            else if (keyboardState.IsKeyDown(Keys.S))
            {
                movementDirection.Y = 1f;
            }

            if(keyboardState.IsKeyDown(Keys.A))
            {
                movementDirection.X = -1f;
            }

            else if (keyboardState.IsKeyDown(Keys.D))
            {
                movementDirection.X = 1f;
            }

            Vector2 velocity = Vector2.Zero + _gravityPull;

            if (movementDirection != Vector2.Zero)
            {
                velocity = Vector2.Normalize(movementDirection) * Speed;
            }
            
            Position += velocity * ScalableGameTime.DeltaTime;

            // Detect mouse position
            MouseState mouseState = Mouse.GetState();
            Vector2 distance = mouseState.Position.ToVector2() - Position;  
            Orientation = (float)Math.Atan2(distance.Y, distance.X);

            // Handle shooting missiles
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                FireRicochetMissile();
            }

            else if(mouseState.RightButton == ButtonState.Pressed)
            {
                FireBlackHoleMissile();
            }
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            _game.SpriteBatch.End();
        }

        /*private void FireMissile()
        {
            if(LastFiredTime + CoolDownTime <= ScalableGameTime.RealTime)
            {
                Vector2 direction = new Vector2(MathF.Cos(Orientation), MathF.Sin(Orientation));
                Vector2 displacement = Texture.Width / 2f * direction;

                // Calculate gun point vectors
                _leftGunVector = Vector2.Rotate(_leftGunDirectionFromCentre, Orientation);
                _rightGunVector = Vector2.Rotate(_rightGunDirectionFromCentre, Orientation);

                Missile leftMissile = new Missile();
                leftMissile.Position = this.Position + _leftGunVector; //+ displacement;
                leftMissile.Orientation = this.Orientation;
                leftMissile.LoadContent();
                leftMissile.Initialize();

                Missile rightMissile = new Missile();
                rightMissile.Position = this.Position + _rightGunVector; //+ displacement;
                rightMissile.Orientation = this.Orientation;
                rightMissile.LoadContent();
                rightMissile.Initialize();

                LastFiredTime = ScalableGameTime.RealTime;

                _shootingSoundEffect.Play();    
            }
        }*/

        private void FireBlackHoleMissile()
        {
            if (GameObjectCollection.FindObjectsByType(typeof(BlackHoleMissile))?.Length > 0)
                return;

            Vector2 direction = new Vector2(MathF.Cos(Orientation), MathF.Sin(Orientation));
            Vector2 displacement = Texture.Width / 2f * direction;

            BlackHoleMissile missile = new BlackHoleMissile();
            missile.Position = this.Position; //+ displacement;
            missile.Orientation = this.Orientation;
            missile.LoadContent();
            missile.Initialize();


            _blackHoleMissileSoundEffect.Play();

        }

        private void FireRicochetMissile()
        {
            if (LastFiredTime + CoolDownTime <= ScalableGameTime.RealTime)
            {
                Vector2 direction = new Vector2(MathF.Cos(Orientation), MathF.Sin(Orientation));
                Vector2 displacement = Texture.Width / 2f * direction;

                RicochetMissile missile = new RicochetMissile();
                missile.Position = this.Position; //+ displacement;
                missile.Orientation = this.Orientation;
                missile.LoadContent();
                missile.Initialize();

                LastFiredTime = ScalableGameTime.RealTime;

                _ricochetMissileSoundEffect.Play();
            }
        }

        private void FireControllableMissile()
        {
            if (_activeMissile != null)
                return;

            ControllableMissile missile = new ControllableMissile();
            missile.Position = this.Position;
            missile.Orientation = this.Orientation;
            missile.LoadContent();
            missile.Initialize();

            _activeMissile = missile;

            _ricochetMissileSoundEffect.Play();
        }

        public void ApplyGravityPull(Vector2 force)
        {
            _gravityPull += force;
        }

        public void ResetGravityPull()
        {
            _gravityPull = Vector2.Zero;
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
            if (collisionInfo.Other is Asteroid || collisionInfo.Other is Background)
            {
                GameObjectCollection.DeInstantiate(this);
                _spaceshipDeathSoundEffect.Play();
            }
        }

        
    }
}
