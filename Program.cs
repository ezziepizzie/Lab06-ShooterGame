// using var game = new Lab06.Game1();
using var game = new GAlgoT2530.Engine.GameEngine("Simple Shooter", 1024, 600, false);
game.AddScene("MainScene", new Lab06.MainScene());
game.Run();
