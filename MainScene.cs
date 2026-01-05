using GAlgoT2530.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public class MainScene : GameScene
    {
        public override void CreateScene()
        {
            Background background = new Background("purple");
            Spaceship spaceship = new Spaceship("spaceShips_009_right");
            spaceship.Speed = 100f;

            AsteroidSpawner asteroidSpawner = new AsteroidSpawner();
            asteroidSpawner.SpawnsPerBatch = 5;
            asteroidSpawner.SpawnTimer = 10f;
        }
    }
}
