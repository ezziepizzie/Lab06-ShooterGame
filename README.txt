Acknowledgement / Honour Code:
- Codes are written by going through available classes such as GameObjectCollection.cs
  to see what functions are available to use.
- Visual Studio's autocomplete feature sometimes provides insights on how to approach the
  intended functionality.

Missile 1 (Orbiter Missile): OrbiterMissile.cs

- Press Left Click to shoot this missile.
- The missile will orbit the spaceship and move accordingly.
- Any asteroids hit will be destroyed.
- The missile will be destroyed after 10 seconds.
- Only one orbiter missile can be shot at a time.

Missile 2 (Black Hole Missile): BlackHoleMissile.cs

- Press Right Click to shoot this missile
- This missile has a short range and starts activating 0.5s after the initial shot.
- The missile will pull in any near asteroids in a certain radius.
- The missile will also pull the player/spaceship, where the player can move in the 
  opposite direction to fight against it.
- Any asteroids or spaceship that gets pull in will be destroyed.
- Only one black hole missile can be shot at a time.