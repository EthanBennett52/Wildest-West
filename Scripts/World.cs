using Godot;
using System;

public class World : Node2D {
	//To approximately convert to/from map tile location VS absolute location (i.e. a node's position field) divide or multiply by this constant
	private const int PixelMultiplier = 32;
	//which iteration the map is on, i.e. which level the player is on 
	private int iteration;

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

	public override void _Ready() {
		// Set default map values
		mapSize = new Vector2(120, 70);
		// I don't know why this cast should be necessary, but it is 
		roadThreshholds = new Vector2((float)0.05, (float)0.185); 
		topTerrainThreshold = 0.4;

		rand = new RandomNumberGenerator();
		//randomize the seed, comment out for certain testing
		rand.Randomize();

		bottomTerrainMap = GetChild(2) as Godot.TileMap;
		topTerrainMap = GetChild(1) as Godot.TileMap;
		//propMap = GetChild(0) as Godot.TileMap;

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
		bottomTerrainMap.Clear();
		topTerrainMap.Clear();

		noise.Seed = (int)rand.Randi();
		PlaceBottomTerrain();
		PlaceTopTerrain();
		PlaceRoads();
		PlacePlayer();
	}

	private void PlaceBottomTerrain() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				//set entire map to "default tile" so there are no gaps
				bottomTerrainMap.SetCell(-x + width / 2, -y + height / 2, 0);
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
					topTerrainMap.SetCell(-x + width / 2, -y + height / 2, 1);
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
					topTerrainMap.SetCell(-x + width / 2, -y + height / 2, 2);
				}
			}
		}
		//not necessary if not using autotiling tileset, remove if causing issues
		topTerrainMap.UpdateBitmaskRegion(new Vector2(0, 0), mapSize);
	}

	// Places the 1x5-sized exit tiles, making sure there is no water 
	// Removes any impassable props in this area
	// Return: a vector containing the coordinates of the top-left corner of the level-change trigger area
	private Vector2 PlaceExit() {
		// TODO: Implement
		return new Vector2(0,0);
	}

	private void PlacePlayer() {
		// Player should spawn at appx -58, 0 (-1835, 0)

		// TODO: Make sure there isn't a water tile or an impassable prop at this location
		// if so, place the player elsewhere
		GetNode<Node2D>("/root/Main/Player").Position = new Vector2(-1835, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		// Next stage triggers in the area of 60,-3 to 61,2 (1895,-110 to 1952, 128)

		// TODO: Change implementation to use values from PlaceExit() to determine correct area
		Node2D player = GetNode<Node2D>("/root/Main/Player");
		if ( (player.Position[0] >= 1895 && player.Position[1] >= -110) && (player.Position[0] <= 1952 && player.Position[1] <= 128) ) {
			ResetMap(noise);
		}
	}
}
