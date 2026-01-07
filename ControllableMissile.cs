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
    public class ControllableMissile : SpriteGameObject, ICollidable
    {
        public float Speed = 200f;
        private Vector2 _velocity;
        private Rectangle _rectangle;
        private SoundEffect _explosionSoundEffect;

        private float _turnSpeed = 4f;


        public ControllableMissile() : base("spaceMissiles_015_right")
        {

        }

        public override void LoadContent()
        {
            // Reusing SpriteGameObject.LoadContent() will load the texture;
            base.LoadContent();

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

            // Listen to non-intersection between Background and ControllableMissile objects
            _game.CollisionEngine.Listen(typeof(Background), typeof(ControllableMissile), CollisionEngine.NotAABB);

            // Listen to non-intersection between Asteroid and ControllableMissile objects
            _game.CollisionEngine.Listen(typeof(Asteroid), typeof(ControllableMissile), CollisionEngine.AABB);
        }

        public override void Update()
        {
            Vector2 mouseWorldPosition = Mouse.GetState().Position.ToVector2();

            Vector2 currentDirection = Vector2.Normalize(_velocity);

            Vector2 targetDirection = mouseWorldPosition - Position;

            if (targetDirection != Vector2.Zero)
                targetDirection.Normalize();

            float currentAngle = MathF.Atan2(currentDirection.Y, currentDirection.X);
            float targetAngle = MathF.Atan2(targetDirection.Y, targetDirection.X);

            float newAngle = MathHelper.WrapAngle(
                MathHelper.Lerp(currentAngle, targetAngle, _turnSpeed * ScalableGameTime.DeltaTime)
            );

            // Apply new velocity
            Vector2 newDirection = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));
            _velocity = newDirection * Speed;

            Orientation = newAngle;
            Position += _velocity * ScalableGameTime.DeltaTime;
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
            }

            else if (collisionInfo.Other is Asteroid asteroid)
            {
                GameObjectCollection.DeInstantiate(asteroid);

                //_explosionSoundEffect.Play();
            }
        }
    }
}
