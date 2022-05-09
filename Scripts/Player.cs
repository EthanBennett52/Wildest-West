using Godot;
using System;
using System.IO;

public class Player : KinematicBody2D, Damageable
{
	public int speed = 300;

	private int maxSpeed = 300;
	private int dodgeSpeed = 900;
	//The weapons the player has
	private Gun[] weapons = new Gun[2];
	//The weapon that is currently equiped
	private Gun activeWeapon;
	private int activeWeaponIndex = 0;
	//The players max health
	private int maxHealth = 100;
	//The players current health
	protected int health;
	private AnimationPlayer animations;
	public bool isDodging = false;
	private bool dodgeOnCooldown = false;
	private Timer dodgeCooldownTimer;
	private Vector2 velocity = new Vector2();
	
	AudioStreamPlayer soundEffect;

	[Signal]
	delegate void changeHealth(int change, int max);
	
	[Signal]
	delegate void changeMaxHealth(int change, int max);
	
	[Signal]
	delegate void updateAmmo(int change, int max);

	[Signal]
	delegate void death();

	//number variable is one or two
	[Signal]
	delegate void updateHotbarGun(string name, int number);

	
	public bool takeDamage(int damage){
		if(!isDodging){
			health = health - damage;
			if(health <= 0){
				EmitSignal("death");
			}
			EmitSignal("changeHealth", health, maxHealth);
			soundEffect.Play();
			return true;
		}
		return false;
		
	}

	//Swaps the active weapon.
	public void swapWeapon(int i){
		if (weapons[i] != null && weapons[i] != activeWeapon){
			activeWeapon.Hide();
			activeWeapon.SetProcess(false);
			activeWeapon = weapons[i];
			activeWeapon.SetProcess(true);
			activeWeapon.Show();

			activeWeaponIndex = i;
			EmitSignal("updateHotbarGun", activeWeapon.name, activeWeaponIndex + 1);
		}
		EmitSignal("updateAmmo", activeWeapon.loaded, activeWeapon.ammo);
	}

	public void pickupWeapon(Gun weapon){
		weapon.Hide();
		weapon.SetProcess(false);
		AddChild(weapon);

		foreach (Gun w in weapons) {
			if (w != null && w.name == weapon.name){
				w.pickupAmmo(weapon.ammo);
				EmitSignal("updateAmmo", activeWeapon.loaded, activeWeapon.ammo);
				weapon.QueueFree();
				return;
			}
		}
		
		if (isFreeWeaponSlot()){
			for (int x = 0; x < weapons.Length; x++){
				if (weapons[x] == null){
					weapons[x] = weapon;
					//AddChild(weapons[x]);
					weapons[x].pickedUp();
					swapWeapon(x);
					
				}
			}

		} else {

			Gun oldGun = activeWeapon;
			oldGun.dropped();
			RemoveChild(oldGun);

			weapons[activeWeaponIndex] = weapon;
			//AddChild(weapon);
			activeWeapon = weapon;
			activeWeapon.pickedUp();

			//Drops the old weapon as a weaponPickup
			PackedScene weaponPickupScn = GD.Load<PackedScene>("res://Scenes/WeaponPickup.tscn");
			WeaponPickup weaponPickup = weaponPickupScn.Instance<WeaponPickup>();
			GetParent().AddChild(weaponPickup);
			weaponPickup.setWeapon(oldGun);
			weaponPickup.GlobalPosition = GlobalPosition;
		}
		
	}

	//Adds ammo to active weapon.
	public void pickupAmmo(int amount){
		activeWeapon.pickupAmmo(amount);
		EmitSignal("updateAmmo", activeWeapon.loaded, activeWeapon.ammo);
	}

	//Heals the player
	public void heal(int amount){
		if ((health + amount) >= maxHealth){
			health = maxHealth;
		} else {
			health += amount;
		}
		EmitSignal("changeHealth", health, maxHealth);
	}

	//Checks if there is a free weapon slot.
	private bool isFreeWeaponSlot(){
		foreach (Gun x in weapons){
			if (x == null){
				return true;
			}
		}
		return false;
	}

	public void startDodge(){
		if (!dodgeOnCooldown){
			
			isDodging = true;
			speed = dodgeSpeed;
			
			if (velocity.x > 0){
				animations.Play("Dodge");
			} else {
				animations.PlayBackwards("Dodge");
			}
		}
		
	}
	public void endDodge(){
		isDodging = false;
		speed = maxSpeed;
		dodgeOnCooldown = true;
		dodgeCooldownTimer.Start();
		animations.Play("Stand");
	}

	private void _OnDodgeCooldownTimeout(){
		dodgeOnCooldown = false;
	}

	private void animationEnded(String animName){
		if (animName == "Dodge"){
			endDodge();
		}
	}

	//Processes the players inputs. Should be called every frame.
	public void GetInput() {
		//LookAt(GetGlobalMousePosition());
		if(!isDodging){
			velocity = new Vector2();

			if (Input.IsActionPressed("right")){
				velocity.x += 1;
			}

			if (Input.IsActionPressed("left")) {
				velocity.x -= 1;
			}

			if (Input.IsActionPressed("up")){
				velocity.y -= 1;
			}

			if (Input.IsActionPressed("down")){
				velocity.y += 1;
			}
			if (Input.IsActionPressed("dodge")){
				startDodge();
			}

			//Fires the active weapon
			if (Input.IsActionPressed("shoot")){
				activeWeapon.fire();
				EmitSignal("updateAmmo", activeWeapon.loaded, activeWeapon.ammo);
			}
			velocity = velocity.Normalized() * speed;
		}


			//Swaps to weapon 1. Bound to "1"
			if (Input.IsActionJustPressed("weapon1")){
				swapWeapon(0);
			}
			//Swaps to weapon 2. Bound to "2"
			if(Input.IsActionJustPressed("weapon2")){
				swapWeapon(1);
			}
			
			//Reloads the active weapon. Bound to "R"
			if (Input.IsActionJustPressed("reload")){
				activeWeapon.reload();
				EmitSignal("updateAmmo", activeWeapon.loaded, activeWeapon.ammo);
			}
			if (Input.IsActionJustReleased("next_weapon")){
				if (activeWeaponIndex + 1 < weapons.Length){
					swapWeapon(activeWeaponIndex + 1);
				} else {
					swapWeapon(0);
				}
			}
			if (Input.IsActionJustReleased("prev_weapon")){
				if (activeWeaponIndex > 0){
					swapWeapon(activeWeaponIndex - 1);
				} else {
					swapWeapon(weapons.Length -1);
				}
			}
	}

	
	public override void _Ready()
	{
		animations = GetNode<AnimationPlayer>("Animations");
		animations.Connect("animation_finished", this, "animationEnded");
		dodgeCooldownTimer = GetNode<Timer>("DodgeCooldown");
		dodgeCooldownTimer.Connect("timeout", this, "_OnDodgeCooldownTimeout");
		
		var milestones = GetNode<MilestoneVar>("/root/MilestoneVar");
		milestones.updateMilestones();
		if(milestones.extraExtraMaxHealth){
			maxHealth += 75;
		} else if (milestones.extraMaxHealth){
			maxHealth += 25;
		}
		if (milestones.lowerDodgeCooldown){
			dodgeCooldownTimer.WaitTime = (float)(dodgeCooldownTimer.WaitTime * .75);
		}

		

		speed = maxSpeed;
		//Initializes the spawn weapon
		PackedScene startingGun = GD.Load<PackedScene>("res://Scenes/Gun.tscn");
		weapons[0] = startingGun.Instance<Gun>();
		AddChild(weapons[0]);
		activeWeapon = weapons[0];
		
		
		health = maxHealth;
		EmitSignal("changeHealth", health, maxHealth);
		
		EmitSignal("updateAmmo", activeWeapon.loaded, activeWeapon.ammo);
		
		soundEffect = FindNode("SoundEffect") as Godot.AudioStreamPlayer;
		
		//sets the score to 0
		System.IO.File.WriteAllText("interface/Score.txt" , 0.ToString());
		
		//This is only here to test weapon swapping
		//PackedScene machineGun = GD.Load<PackedScene>("res://Scenes/MachineGun.tscn");
		//weapons[1] = machineGun.Instance<Gun>();
		//AddChild(weapons[1]);
		//weapons[1].Hide();
		//weapons[1].SetProcess(false);
		

	}
	
	
	public override void _Process(float delta)
	{
		
		GetInput();

		activeWeapon.LookAt(GetGlobalMousePosition());
		
		
		velocity = MoveAndSlide(velocity);
	}
}

