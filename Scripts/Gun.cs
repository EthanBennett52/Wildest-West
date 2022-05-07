using Godot;
using System;

public class Gun : Node2D
{
	PackedScene BULLET = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
	
	public Sprite sprite;
	//for adjusting the bullet spawn location
	double rotationOffset = -.2;
	//How often the gun shoots
	protected double fireRate = .25;
	//How long it takes to reload (in seconds).
	protected double reloadTime = 1.5;
	//Cooldown timer between shots
	private double shotTimer = 0;
	//False if weapon is cooling down between shots
	private Boolean canShoot = true;
	//Max ammo capacity
	public int maxAmmo = 160;
	//amount of ammo available
	public int ammo = 80;
	//number of bullets that can be loaded
	public int maxLoadedCapacity = 8;
	//Ammo loaded in the gun
	public int loaded;
	//Damage per bullet
	public int damage = 25;
	
	//name of the gun
	public string name = "Revolver";

	protected Node2D parent;
	

	//Fires the gun
	public void fire(){
		if (canShoot && loaded > 0){
			CreateBullet(new Vector2((float)Math.Cos(Rotation),(float)Math.Sin(Rotation)));
			canShoot = false;
			shotTimer = fireRate;
			loaded--;
		} /*else if (loaded <= 0) {
			reload();
		}*/
		
	}

	private void CreateBullet(Vector2 bulletTarget){
		Bullet bullet = (Bullet)BULLET.Instance(); 
		bullet.Position = new Vector2((float) (GlobalPosition.x + 50 * Math.Cos(Rotation + rotationOffset)), (float) (GlobalPosition.y + 50 * Math.Sin(Rotation + rotationOffset))); 
		bullet.RotationDegrees = RotationDegrees;
		bullet.setDamage(damage);
		bullet.SetTargetVector(bulletTarget);
		GetNode("/root/Main").AddChild(bullet);
	}

	//Reloads the gun
	public void reload(){
		shotTimer = reloadTime;
		canShoot = false;
		int diff = maxLoadedCapacity - loaded;
		if (ammo >= diff){
			loaded += diff;
			ammo -= diff;
		} else {
			loaded += ammo;
			ammo = 0;
		}
	}

	//Called when the player picks up an ammo pack
	public void pickupAmmo(int amount){
		if ((ammo + amount) > maxAmmo) {
			ammo = maxAmmo;
		} else {
			ammo += amount;
		}		
	}

	public void pickedUp(){
		Show();
		SetProcess(true);
		parent = GetParent<Node2D>();
	}

	public void dropped(){
		Hide();
		SetProcess(false);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!(parent is Bandit)){
			var milestones = GetNode<MilestoneVar>("/root/MilestoneVar");
			if (name == "Revolver" && milestones.extraRevolverDamage){
				damage = (int)(damage * 1.1);
			}
			if (milestones.extraLoadedCapacity){
				maxLoadedCapacity = (int)(maxLoadedCapacity * 1.25);
			}
		}
		
		loaded = maxLoadedCapacity;
		sprite = (Sprite)GetChild(0);
		parent = GetParent<Node2D>();

		//Sets the target. There might be a better way to this without the redundant code in _Process.
		
		if (parent is WeaponPickup){
			dropped();
		} 
		
	}

// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		//The cooldown timer
		if (shotTimer <= 0) {
			canShoot = true;
		} else {
			shotTimer -= delta;
		}

		
		//Keeps the weapon sprite in the correct orientation
		if (270 > Math.Abs(RotationDegrees % 360) && 90 < Math.Abs(RotationDegrees % 360)) {
			sprite.FlipV = true;
			rotationOffset = .2;
		} else {
			sprite.FlipV = false;
			rotationOffset = -.2;
		}
		
	}
}
