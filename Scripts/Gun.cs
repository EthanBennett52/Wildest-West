using Godot;
using System;

public class Gun : Node2D
{
	protected PackedScene BULLET = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
	
	public Sprite sprite;
	protected Sprite muzzleFlash;
	
	//How often the gun shoots
	protected double fireRate = .25;
	//Weapon inaccuracy in radians
	protected double incaccuarcy = Math.PI/36;
	//How long it takes to reload (in seconds).
	protected double reloadTime = 1.5;
	//Cooldown timer between shots
	protected double shotTimer = 0;
	//False if weapon is cooling down between shots
	protected Boolean canShoot = true;
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
	protected RandomNumberGenerator rand;
	protected AnimationPlayer anim;
	

	protected AudioStreamPlayer soundEffect;

	//Fires the gun
	public virtual bool fire(){
		if (canShoot && loaded > 0){
			soundEffect.Stop();
			Vector2 target = new Vector2((float)Math.Cos(Rotation),(float)Math.Sin(Rotation));
			float offset = rand.RandfRange((float)-incaccuarcy/2, (float)incaccuarcy/2);
			CreateBullet(target.Rotated(offset));
			canShoot = false;
			shotTimer = fireRate;
			loaded--;
			
			soundEffect.Play();
			anim.Play("MuzzleFlash");
			return true;
		} 
		return false;
		
	}
	public void soundEffectVolume(int amount){
		soundEffect.VolumeDb = amount;
	}
	
	protected virtual void CreateBullet(Vector2 bulletTarget){
		Bullet bullet = (Bullet)BULLET.Instance(); 
		bullet.Position = new Vector2((float) (GlobalPosition.x + 50 * Math.Cos(Rotation)), (float) (GlobalPosition.y + 50 * Math.Sin(Rotation))); 
		bullet.RotationDegrees = RotationDegrees;
		bullet.setDamage(damage);
		bullet.SetTargetVector(bulletTarget);
		GetNode("/root/Main").AddChild(bullet);
	}

	//Reloads the gun
	public virtual void reload(){
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
		rand = new RandomNumberGenerator();

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
		muzzleFlash = (Sprite)sprite.GetChild(0);
		parent = GetParent<Node2D>();
		anim = GetNode<AnimationPlayer>("AnimationPlayer");


		//Sets the target. There might be a better way to this without the redundant code in _Process.
		
		if (parent is WeaponPickup){
			dropped();
		} 
		soundEffect = FindNode("SoundEffect") as Godot.AudioStreamPlayer;
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
			
		} else {
			sprite.FlipV = false;
		}
		
	}
}
