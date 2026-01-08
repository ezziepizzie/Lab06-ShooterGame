Acknowledgement / Honour Code:
- Codes are written by going through available classes such as GameObjectCollection.cs
  to see what functions are available to use.
- Visual Studio's autocomplete feature sometimes provides insights on how to approach the
  intended functionality.

Missile 1 (Ricochet Missile): RichochetMissile.cs

- Press Left Click to shoot this missile.
- After the first hit, the missile will ricochet/move towards the next nearest asteroid in a
  certain radius.
- The missile has a maximum kill count of 3, so after hitting 3 asteroids, it will destroy itself.
- This missile rewards the players for hitting an asteroid.

Missile 2 (Black Hole Missile): BlackHoleMissile.cs

- This missile has a short range and starts activating 0.5s after the initial shot.
- The missile will pull in any near asteroids in a certain radius.
- The missile will also pull the player/spaceship, where the player can move in the 
  opposite direction to fight against it.
- Any asteroids or spaceship that gets pull in will be destroyed.
- Only one black hole missile can be shot at a time.