using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public class Asteroid : SpriteGameObject
    {
        public float AngularSpeed;
        public float Speed;
        private Vector2 _velocity;

        public Asteroid(string textureName) : base(textureName)
        {

        }

        public override void Initialize()
        {
            // Set Origin
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;

            Position = GenerateRandomPosition();
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
    }
}
