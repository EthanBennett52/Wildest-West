using Godot;
using System;

public class World : Node2D {
	// Load all the scenes for the things that will be spawned 
	PackedScene banditScene = GD.Load<PackedScene>("res://Scenes/Bandit.tscn");
	PackedScene shotgunBanditScene = GD.Load<PackedScene>("res://Scenes/ShotgunBandit.tscn");
	PackedScene rifleBanditScene = GD.Load<PackedScene>("res://Scenes/RifleBandit.tscn");
	PackedScene knifeBanditScene = GD.Load<PackedScene>("res://Scenes/KnifeBandit.tscn");
	PackedScene machineGunBanditScene = GD.Load<PackedScene>("res://Scenes/MachineGunBandit.tscn");
	PackedScene ammoScene = GD.Load<PackedScene>("res://Scenes/AmmoPickup.tscn");
	PackedScene goldScene = GD.Load<PackedScene>("res://Scenes/GoldPickup.tscn");
	PackedScene chestScene = GD.Load<PackedScene>("res://Scenes/Chest.tscn");
	PackedScene healthScene = GD.Load<PackedScene>("res://Scenes/HealthPickup.tscn");

	// Two-dimensional array representing the chances of each enemy spawning per level
	// inner arrays represent { Bandit, ShotgunBandit, RifleBandit, KnifeBandit, MachineGunBandit}
	private double[][] enemyChances = new double[][] {new double[]{ 1.0, 0.0, 0.0, 0.0, 0.0 }, new double[]{ 0.8, 0.2, 0.0, 0.0, 0.0 }, new double[]{ 0.6, 0.4, 0.0, 0.0, 0.0 },
																  new double[]{ 0.6, 0.25, 0.15, 0.0, 0.0 }, new double[]{ 0.5, 0.3, 0.2, 0.0, 0.0 }, new double[]{ 0.35, 0.3, 0.2, 0.15, 0.0 },
																  new double[]{ 0.2, 0.25, 0.3, 0.25, 0.0 }, new double[]{ 0.225, 0.225, 0.225, 0.225, 0.1 }, new double[]{ 0.2, 0.2, 0.2, 0.2, 0.2 },
																  new double[]{ 0.1, 0.225, 0.225, 0.225, 0.225 }, new double[]{ 0.1, 0.2, 0.2, 0.3, 0.2 }, new double[]{ 0.0, 0.15, 0.2, 0.35, 0.3 }};

	// Array representing how many enemies should spawn on each level 
	private int[] enemyNumbers = new int[] { 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 14, 15, 16, 16};

	// Array representing the chances of items spawning
	// { Chest, Gold, Ammo, Health }
	private double[] itemChances = new double[] { 0.1, 0.5, 0.2, 0.2 };

	// To approximately convert to/from map tile location VS absolute location (i.e. a node's position field) divide or multiply by this constant
	private const int PixelMultiplier = 32;
	// Which iteration the map is on, i.e. which level the player is on 
	private int iteration;
	// Array specifying the exit area ( x1, x2, y1, y2 )
	private int[] transitionBound;
	// Array specifying the entrance area ( x1, x2, y1, y2 )
	private int[] entranceBound;

	// Noise generator and random number generator to seed the niose
	private OpenSimplexNoise noise;
	private RandomNumberGenerator rand;

	public Godot.TileMap bottomTerrainMap, topTerrainMap, propMap;

	[Export]
	public Vector2 mapSize;

	[Export(PropertyHint.Range, "0,1,.05")]
	public double topTerrainThreshold;

	[Export]
	public Vector2 roadThreshholds;

	private double enemyThreshold;
	private double enemyUpperThreshold;

	private double itemThreshold;
	private double itemUpperThreshold;

	int width, height;
	
	//ColorRect fadeIn;
	//AnimationPlayer ani;

	AStar astar;

	Node mainScene;
	
	[Signal]
	delegate void startLoading();
	
	[Signal]
	delegate void endLoading();
	
	[Signal]
	delegate void endInitialLoading();
	
	public override void _Ready() {

		//fadeIn  = GetChild(3) as Godot.ColorRect;
		//fadeIn.Connect("fade_finished", this, "_on_FadeIn_fade_finished");
		//ani = FindNode("AnimationPlayer") as Godot.AnimationPlayer;
		
		EmitSignal("startLoading");
		
		mainScene = GetTree().CurrentScene;
		
		// Set default map values
		mapSize = new Vector2(120, 70);
		// Have to cast these floats or this doesn't work
		roadThreshholds = new Vector2((float)0.05, (float)0.185); 
		topTerrainThreshold = 0.4;
		
		enemyThreshold = -0.99;
		enemyUpperThreshold = -.91;

		itemThreshold = -0.75;
		itemUpperThreshold = -0.73;

		iteration = -2;
		transitionBound = new int[4];

		rand = new RandomNumberGenerator();
		// randomize the seed, comment out for certain testing
		rand.Randomize();

		bottomTerrainMap = GetChild(2) as Godot.TileMap;
		topTerrainMap = GetChild(1) as Godot.TileMap;
		propMap = GetChild(0) as Godot.TileMap;

		astar = GetNode<AStar>("AStar");
		

		width = (int)mapSize.x;
		height = (int)mapSize.y;

		noise = new OpenSimplexNoise();
		noise.Seed = (int)rand.Randi();
		noise.Octaves = 1;
		noise.Period = 12;
		//noise.persistence = 0.7;
		
		
		OS.DelayMsec(100);
		EmitSignal("endLoading");
		ResetMap(noise);
	}

	// Resets the map, replacing all the tiles with newly generated ones.
	// TODO: Add loading screen of some sort
	private void ResetMap(OpenSimplexNoise noise) {
		//Deletes all bullets
		//GetTree().CallGroup("Bullets", "Destroy");
		//Deletes all enemies
		//GetTree().CallGroup("Enemy", "Destroy");
		EmitSignal("startLoading");
		GetTree().CallGroup("destroy_on_level_change", "Destroy");
		
		//fadeIn.Show();
		//ani.Play("loadingScreen");
		
		bottomTerrainMap.Clear();
		topTerrainMap.Clear();

		noise.Seed = (int)rand.Randi();
		PlaceBottomTerrain();
		PlaceProps();
		PlaceTopTerrain();
		PlaceRoads();
		PlacePlayer();
		PlaceExit();

		iteration++;

		astar.CreateNavigationMap(bottomTerrainMap);
		astar.DisableCollisionTiles(topTerrainMap);
		placeMainRoad();

		// this is awful
		if (iteration >= 0) {
			if (iteration > 19) {
				PlaceEnemies(enemyNumbers[19]);
			}
			else {
				PlaceEnemies(enemyNumbers[iteration]);
			}
		}
		else {
			// no, really
			GetNode<Node2D>("/root/Main/Player").Position = new Vector2(transitionBound[0], transitionBound[2]);
		}
		PlaceItems();

		EmitSignal("endLoading");
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


	// places cactus randomly
	private void PlaceProps() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (rand.Randf() > 0.9965) {
					propMap.SetCell(-x + width / 2, -y + height / 2, 5);
				}
			}
		}
	}
	private void placeMainRoad(){
		Vector2 entranceVector = new Vector2( entranceBound[1] + 1, entranceBound[3] - 1);
		Vector2 exitVector = new Vector2(transitionBound[0] / PixelMultiplier, (transitionBound[2] / PixelMultiplier) + 2 );

		Vector2[] path = astar.GetPointPath(entranceVector, exitVector);

		foreach (Vector2 tile in path){
			for (int x = 0; x < 3; x++){
				for (int y = 0; y < 3; y++){
					Vector2 target = tile + new Vector2(x -1, y -1);

					if (bottomTerrainMap.GetCellv(target) == -1){
						continue;
					} else {
						topTerrainMap.SetCellv(target, 3);
					}

				}
			}
			
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

	// Places enemies
	// num: amount of enemies to place
	private void PlaceEnemies(int num) {
		Vector2[] places = new Vector2[num];
		int[] counts = { 0, 0, 0, 0, 0 };
		int placed = 0;
		double mod = 0.0;
		int altIteration = iteration;

		if (iteration > 11) {
			altIteration = 11;
		}

		while (placed < num) { 
			for (int x = 0; x < (width - 40); x++) {
				for (int y = 10; y < (height - 10); y++) {
					if ( placed >= num) {
						break;
					}
					double val = noise.GetNoise2d(x, y);
					if (val > enemyThreshold && val < (enemyUpperThreshold + mod)) {
						Vector2 pos = new Vector2( (-x + width / 2) * 32 , (-y + height / 2) * 32 );
						double n = rand.Randf();
						bool isFar = CheckPlaces(pos, places, 5);

						if (n <= enemyChances[altIteration][0] && isFar) {
							Node2D enemy = banditScene.Instance<Node2D>();
							enemy.Position = pos;
							counts[0]++;
							enemy.Name = "Bandit" + counts[0];
							placed++;
							mainScene.AddChild(enemy);
						}
						else if (n <= (enemyChances[altIteration][0] + enemyChances[altIteration][1])  && isFar) {
							Node2D enemy = shotgunBanditScene.Instance<Node2D>();
							enemy.Position = pos;
							counts[1]++;
							enemy.Name = "ShotgunBandit" + counts[1];
							placed++;
							mainScene.AddChild(enemy);
						}
						else if (n <= (enemyChances[altIteration][0] + enemyChances[altIteration][1] + enemyChances[altIteration][2]) && isFar) {
							Node2D enemy = rifleBanditScene.Instance<Node2D>();
							enemy.Position = pos;
							counts[2]++;
							enemy.Name = "RifleBandit" + counts[2];
							placed++;
							mainScene.AddChild(enemy);
						}
						else if (n <= (enemyChances[altIteration][0] + enemyChances[altIteration][1] + enemyChances[altIteration][2] + enemyChances[altIteration][3]) && isFar) { 
							Node2D enemy = knifeBanditScene.Instance<Node2D>();
							enemy.Position = pos;
							counts[3]++;
							enemy.Name = "KnifeBandit" + counts[3];
							placed++;
							mainScene.AddChild(enemy);
						}
						else if (n <= (enemyChances[altIteration][0] + enemyChances[altIteration][1] + enemyChances[altIteration][2] + enemyChances[altIteration][3] + enemyChances[altIteration][4]) && isFar) { 
							Node2D enemy = machineGunBanditScene.Instance<Node2D>();
							enemy.Position = pos;
							counts[4]++;
							enemy.Name = "MachineGunBandit" + counts[4];
							placed++;
							mainScene.AddChild(enemy);
						}
						else {
							continue;
						}
						places[placed-1] = pos;
						//foreach (Vector2 i in places) {
						//	GD.Print(i);
						//}
					}
				}
			}
			mod += .005;
		}
	}

	private void PlaceItems() {
		Vector2[] places = new Vector2[10];
		int placed = 0;
		double mod = 0.0;
		while (placed < 8) {
			for (int x = 0; x < (width - 20); x++) {
				for (int y = 10; y < (height - 10); y++) {
					double val = noise.GetNoise2d(x, y);
					if (val > itemThreshold && val < (itemUpperThreshold + mod)) {
						Vector2 pos = new Vector2((-x + width / 2) * 32, (-y + height / 2) * 32);
						double n = rand.Randf();
						bool isFar = CheckPlaces(pos, places, 10);

						if (n <= itemChances[0] && isFar) {
							Node2D item = chestScene.Instance<Node2D>();
							item.Position = pos;
							placed++;
							mainScene.AddChild(item);
						}
						else if (n <= (itemChances[0] + itemChances[1]) && isFar) {
							Node2D item = goldScene.Instance<Node2D>();
							item.Position = pos;
							placed++;
							mainScene.AddChild(item);
						}
						else if (n <= (itemChances[0] + itemChances[1] + itemChances[2]) && isFar) {
							Node2D item = ammoScene.Instance<Node2D>();
							item.Position = pos;
							placed++;
							mainScene.AddChild(item);
						}
						else if (n <= (itemChances[0] + itemChances[1] + itemChances[2] + itemChances[3]) && isFar) {
							Node2D item = healthScene.Instance<Node2D>();
							item.Position = pos;
							placed++;
							mainScene.AddChild(item);
						}
						else {
							continue;
						}
						places[placed - 1] = pos;
					}
				}
			}
			mod += .005;
		}
	}
	// checks a given coordinate to see if another enemy has already been placed there
	private bool CheckPlaces(Vector2 pos, Vector2[] places, int dist) {
		foreach (Vector2 i in places) {
			if (tooClose(i, pos, dist)) {
				return false;
			}
		}
		return true;
	}

	// checks if a coordinate is too close to another coordinate 
	// within ~5 tiles (160 units) on each side is too close for enemies
	private bool tooClose(Vector2 pos1, Vector2 pos2, int dist) {
		if (pos2.x > (pos1.x - (dist * PixelMultiplier)) && pos2.x < (pos1.x + (dist * PixelMultiplier))) {
			if (pos2.y > (pos1.y - (dist * PixelMultiplier)) && pos2.y < (pos1.y + (dist * PixelMultiplier))) {
				return true; 
			}
		}
		return false; 
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



}
