using Godot;
using System;

public class World : Node2D {
	// To approximately convert to/from map tile location VS absolute location (i.e. a node's position field) divide or multiply by this constant
	private const int PixelMultiplier = 32;
	// Which iteration the map is on, i.e. which level the player is on 
	private int iteration;
	// Array specifying the exit area ( x1, x2, y1, y2 )
	private int[] transitionBound;
	// Array specifying the entrance area ( x1, x2, y1, y2 )
	private int[] entranceBound;

	private OpenSimplexNoise noise;
	private RandomNumberGenerator rand;

	public Godot.TileMap bottomTerrainMap, topTerrainMap, propMap;

	[Export]
	public Vector2 mapSize;

	[Export(PropertyHint.Range, "0,1,.05")]
	public double topTerrainThreshold;

	[Export]
	public Vector2 roadThreshholds;

	int width, height;
	
	ColorRect fadeIn;
	AnimationPlayer ani;

	AStar astar;


	public override void _Ready() {
		fadeIn  = GetChild(3) as Godot.ColorRect;
		fadeIn.Connect("fade_finished", this, "_on_FadeIn_fade_finished");
		ani = FindNode("AnimationPlayer") as Godot.AnimationPlayer;
		
		
		// Set default map values
		mapSize = new Vector2(120, 70);
		// Have to cast these floats or this doesn't work
		roadThreshholds = new Vector2((float)0.05, (float)0.185); 
		topTerrainThreshold = 0.4;

		iteration = -1;
		transitionBound = new int[4];

		rand = new RandomNumberGenerator();
		// randomize the seed, comment out for certain testing
		rand.Randomize();

		bottomTerrainMap = GetChild(2) as Godot.TileMap;
		topTerrainMap = GetChild(1) as Godot.TileMap;
		//propMap = GetChild(0) as Godot.TileMap;

		astar = GetNode<AStar>("AStar");
		

		width = (int)mapSize.x;
		height = (int)mapSize.y;

		noise = new OpenSimplexNoise();
		noise.Seed = (int)rand.Randi();
		noise.Octaves = 1;
		noise.Period = 12;
		//noise.persistence = 0.7;

		ResetMap(noise);
	}

	// Resets the map, replacing all the tiles with newly generated ones.
	// TODO: Add loading screen of some sort
	private void ResetMap(OpenSimplexNoise noise) {
		//Deletes all bullets
		GetTree().CallGroup("Bullets", "Destroy");
		//Deletes all enemies
		GetTree().CallGroup("Enemy", "Destroy");
		fadeIn.Show();
		ani.Play("loadingScreen");
		
		bottomTerrainMap.Clear();
		topTerrainMap.Clear();

		noise.Seed = (int)rand.Randi();
		PlaceBottomTerrain();
		PlaceTopTerrain();
		//PlaceRoads();
		PlacePlayer();
		PlaceExit();
		iteration++;

		astar.CreateNavigationMap(bottomTerrainMap);
		astar.DisableCollisionTiles(topTerrainMap);
		placeMainRoad();
	}

	private void PlaceBottomTerrain() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				//set entire map to "default tile" (dirt or rocky dirt) so there are no gaps
				if (rand.Randf() > 0.965) {
					bottomTerrainMap.SetCell(-x + width / 2, -y + height / 2, 4);
				}
				else {
					bottomTerrainMap.SetCell(-x + width / 2, -y + height / 2, 0);
				}
			}
		}
		//not necessary if not using autotiling tileset, remove if causing issues
		bottomTerrainMap.UpdateBitmaskRegion(new Vector2(0, 0), mapSize);
	}

	private void PlaceTopTerrain() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				//place secondary tile, usually water or some other impassable liquid
				if (noise.GetNoise2d(x, y) > topTerrainThreshold) {
					if (noise.GetNoise2d(x, y) > (topTerrainThreshold + .15)) {
						topTerrainMap.SetCell(-x + width / 2, -y + height / 2, 2);
					}
					else {
						topTerrainMap.SetCell(-x + width / 2, -y + height / 2, 1);
					}
				}
			}
		}
		//not necessary if not using autotiling tileset, remove if causing issues
		topTerrainMap.UpdateBitmaskRegion(new Vector2(0, 0), mapSize);
	}

	private void PlaceRoads() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				double n = noise.GetNoise2d(x, y);
				if (n > roadThreshholds.x && n < roadThreshholds.y) {
					topTerrainMap.SetCell(-x + width / 2, -y + height / 2, 3);
				}
			}
		}
		//not necessary if not using autotiling tileset, remove if causing issues
		topTerrainMap.UpdateBitmaskRegion(new Vector2(0, 0), mapSize);
	}

	private void placeMainRoad(){
		Vector2 entranceVector = new Vector2( entranceBound[1] + 1, entranceBound[3] - 1);
		Vector2 exitVector = new Vector2(transitionBound[0] / PixelMultiplier, (transitionBound[2] / PixelMultiplier) + 2 );

		Vector2[] path = astar.GetPointPath(entranceVector, exitVector);

		foreach (Vector2 tile in path){
			topTerrainMap.SetCellv(tile, 3);
		}

	}

	// Places the 1x5-sized exit tiles, making sure there is no water 
	// Removes any impassable props in this area
	// Return: a vector containing the coordinates of the top-left corner of the level-change trigger area
	private void PlaceExit() {

		Vector2 xRange = new Vector2(59,60);
		Vector2 yRange = new Vector2(-2, 2);

		int[] bounds = TryArea(xRange, yRange, 5, 0, -1);
		for (int x = bounds[0] - 1; x <= bounds[1]; x++) {
			for (int y = bounds[2]; y <= bounds[3]; y++) {
				topTerrainMap.SetCell(x, y, 7);
			}
		}
		for (int i = 0; i <= 3; i++) {
			transitionBound[i] = bounds[i] * PixelMultiplier;
		}
		transitionBound[0] -= 48;
	}

	private void PlacePlayer() {
		// Player should spawn at appx -57, 0 (-1824, 0)
		// If blocked, retry at different y-value but same x-value 
		// range of acceptable y-values is 1056 through -1024
		// range of x-values starts at -59 through -57, overall is about -1856 through 1824

		Vector2 xRange = new Vector2(-59, -57);
		Vector2 yRange = new Vector2(-1, 1);

		int[] bounds = TryArea(xRange, yRange, 3, 0, 1);

		//place entrance platform
		for (int x = bounds[0]; x <= bounds[1]; x++) {
			for (int y = bounds[2]-1; y <= bounds[3]+1; y++) {
				topTerrainMap.SetCell(x, y, 7);
			}
		}

		entranceBound = bounds;
		GetNode<Node2D>("/root/Main/Player").Position = new Vector2( (bounds[0] + bounds[1]) / 2 * PixelMultiplier + 8, (bounds[2] + bounds[3]) / 2 * PixelMultiplier +16);
		//GD.Print("placed player at " + ((bounds[0] + bounds[1]) / 2) + ", " + ((bounds[2] + bounds[3]) / 2));
	}

	// Checks a given area to make sure it is clear, finds a clear area if not
	// can be used with PlacePlayer() or PlaceExit()
	// Returns an array containing the x and y-bounds of the found clear area
	// direction: 1 or -1 only. defines the x-direction in which the algorithm will move if it fails to find a clear space 
	private int[] TryArea(Vector2 xRange, Vector2 yRange, int height, int areaIteration, int direction) {
		Vector2 xRangeLocal = new Vector2(xRange[0], xRange[1]);

		// integer division to round down
		int heightModifier = height / 2;

		if (areaIteration > 25) {
			xRangeLocal[0] += direction;
			xRangeLocal[1] += direction;
			areaIteration = 0;
		}

		for (int x = (int)xRangeLocal[0]; x <= (int)xRangeLocal[1]; x++) {
			for (int y = (int)yRange[0]; y <= (int)yRange[1]; y++) {
				//GD.Print("tile " + x + "," + y + " is " + topTerrainMap.GetCell(x, y) + " current iteration: " + areaIteration);
				if (topTerrainMap.GetCell(x, y) == 1) {
					int newY = rand.RandiRange(-32, 33);
					return TryArea(xRangeLocal, new Vector2((newY - heightModifier), (newY + heightModifier)), height, ++areaIteration, direction);
				}
			}
		}
		return new int[] { (int)xRangeLocal[0], (int)xRangeLocal[1], (int)yRange[0], (int)yRange[1] };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		//check if player is in transition area
		Node2D player = GetNode<Node2D>("/root/Main/Player");
		if ( (player.Position[0] >= transitionBound[0] && player.Position[1] >= transitionBound[2]) && (player.Position[0] <= transitionBound[1] && player.Position[1] <= transitionBound[3]) ) {
			ResetMap(noise);
		}
	}


private void _on_FadeIn_fade_finished()
{
	fadeIn.Hide();
}

}
