using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public class Asteroid : SpriteGameObject, ICollidable
    {
        public float AngularSpeed;
        public float Speed;
        private Vector2 _velocity;
        private Vector2 _initialVelocity;
        private Rectangle _rectangle;

        public Asteroid(string textureName) : base(textureName)
        {

        }

        public override void Initialize()
        {
            // Set Origin
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;

            Position = GenerateRandomPosition();

            _rectangle.Location = Position.ToPoint();
            _rectangle.Width = Texture.Width;
            _rectangle.Height = Texture.Height;
        }

        private Vector2 GenerateRandomPosition()
        {
            Vector2 windowCentre = new Vector2(_game.Window.ClientBounds.Width / 2f, _game.Window.ClientBounds.Height / 2f);

            Random random = new Random();

            // Generate random angle between -pi to pi
            float angle = random.NextSingle() * MathHelper.TwoPi - MathHelper.Pi;

            Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            Vector2 displacement = _game.Window.ClientBounds.Width * direction;

            // Calculate velocity
            _velocity = -direction * Speed;
            _initialVelocity = _velocity;

            return windowCentre + displacement;
        }

        public override void Update()
        {
            Position += _velocity * ScalableGameTime.DeltaTime;
            Orientation += AngularSpeed * ScalableGameTime.DeltaTime;
            Orientation = MathHelper.WrapAngle(Orientation);
        }

        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(Texture, Position, null, Color.White, Orientation, Origin, Scale, SpriteEffects.None, 0f);

            _game.SpriteBatch.End();
        }

        public void PullTowardsBlackHole(Vector2 targetPosition, float pullSpeed)
        {
            Vector2 direction = targetPosition - Position;
            float distance = direction.Length();
            if (distance > 0)
            {
                direction.Normalize();
                // Directly set velocity toward the black hole center
                _velocity = direction * pullSpeed;
            }
        }

        public void ReturnToInitialVelocity()
        {
            _velocity = Vector2.Lerp(_velocity, _initialVelocity, .5f);

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
            if (collisionInfo.Other is Missile)
            {
                GameObjectCollection.DeInstantiate(this);
                //_explosionSoundEffect.Play();
            }
            else if (collisionInfo.Other is Spaceship)
            {
                //GameObjectCollection.DeInstantiate(this);
            }
        }
    }
}
