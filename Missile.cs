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
    public class Missile : SpriteGameObject, ICollidable
    {
        public float Speed = 200f;
        private Vector2 _velocity;
        private Rectangle _rectangle;
        private SoundEffect _explosionSoundEffect;

        public Missile() : base("spaceMissiles_015_right")
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

            // Listen to non-intersection between Background and Missile objects
            _game.CollisionEngine.Listen(typeof(Background), typeof(Missile), CollisionEngine.NotAABB);
        }

        public override void Update()
        {
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
                _explosionSoundEffect.Play();
            }
        }
    }
}
