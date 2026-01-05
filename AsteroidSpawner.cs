using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public class AsteroidSpawner : GameObject
    {
        public int SpawnsPerBatch;
        public float SpawnTimer; // how many seconds to wait between batches

        private float _lastSpawnTime;

        public AsteroidSpawner()
        {
            // Intentionally left blank;
        }

        public override void Update()
        {
            if (_lastSpawnTime + SpawnTimer < ScalableGameTime.RealTime)
            {
                SpawnAsteroids();
                _lastSpawnTime = ScalableGameTime.RealTime;
            }
        }

        private void SpawnAsteroids()
        {
            for (int i = 0; i < SpawnsPerBatch; i++)
            {
                Asteroid asteroid = new Asteroid("spaceMeteors_002_small");
                asteroid.Speed = 100f;
                asteroid.AngularSpeed = MathHelper.ToRadians(60f);
                asteroid.LoadContent();
                asteroid.Initialize();
            }
        }
    }
}
