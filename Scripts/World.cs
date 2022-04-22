using Godot;
using System;

public class World : Node2D {

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

		var rand = new RandomNumberGenerator();
		//randomize the seed, commented out for testing
		//rand.Randomize();

		bottomTerrainMap = GetChild(2) as Godot.TileMap;
		topTerrainMap = GetChild(1) as Godot.TileMap;
		//propMap = GetChild(0) as Godot.TileMap;

		width = (int)mapSize.x;
		height = (int)mapSize.y;

		var noise = new OpenSimplexNoise();
		noise.Seed = (int)rand.Randi();
		noise.Octaves = 1;
		noise.Period = 12;
		//noise.persistence = 0.7;

		PlaceBottomTerrain(noise);
		PlaceTopTerrain(noise);
		PlaceRoads(noise);
	}

	private void PlaceBottomTerrain(OpenSimplexNoise noise) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				//set entire map to "default tile" so there are no gaps
				bottomTerrainMap.SetCell(-x + width / 2, -y + height / 2, 0);
			}
		}
		//not necessary if not using autotiling tileset, remove if causing issues
		bottomTerrainMap.UpdateBitmaskRegion(new Vector2(0, 0), mapSize);
	}

	private void PlaceTopTerrain(OpenSimplexNoise noise) {
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

	private void PlaceRoads(OpenSimplexNoise noise) {
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

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
