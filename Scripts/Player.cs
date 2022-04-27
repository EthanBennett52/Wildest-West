using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] public int speed = 300;
	//The weapons the player has
	private Gun[] weapons;
	//The weapon that is currently equiped
	private Gun activeWeapon;
	private int activeWeaponIndex = 0;
	//The players max health
	private int maxHealth = 100;
	//The players current health
	protected int health;
	private Vector2 velocity = new Vector2();

	public void takeDamage(int damage){
		health = health - damage;
	}

	public void swapWeapon(int i){
		if (weapons[i] != null && weapons[i] != activeWeapon){
			activeWeapon.Hide();
			activeWeapon.SetProcess(false);
			activeWeapon = weapons[i];
			activeWeapon.SetProcess(true);
			activeWeapon.Show();

			activeWeaponIndex = i;
		}
	}

	public void pickupWeapon(Gun weapon){
		weapon.Hide();
		weapon.SetProcess(false);

		foreach (Gun w in weapons) {
			if (w.Name == weapon.Name){
				w.pickupAmmo(weapon.ammo);
				return;
			}
		}
		
		if (isFreeWeaponSlot()){
			for (int x = 0; x < weapons.Length; x++){
				if (weapons[x] == null){
					weapons[x] = weapon;
					AddChild(weapons[x]);
					weapons[x].pickedUp();
					swapWeapon(x);
				}
			}

		} else {

			Gun oldGun = activeWeapon;
			oldGun.dropped();
			RemoveChild(oldGun);

			weapons[activeWeaponIndex] = weapon;
			AddChild(weapon);
			activeWeapon = weapon;
			activeWeapon.pickedUp();

			PackedScene weaponPickupScn = GD.Load<PackedScene>("res://Scenes/WeaponPickup.tscn");
			WeaponPickup weaponPickup = weaponPickupScn.Instance<WeaponPickup>();
			GetParent().AddChild(weaponPickup);
			weaponPickup.setWeapon(oldGun);
			weaponPickup.GlobalPosition = GlobalPosition;
		}
		
	}

	public void pickupAmmo(int amount){
		activeWeapon.pickupAmmo(amount);
	}

	public void heal(int amount){
		GD.Print("Health before: " + health);
		if ((health + amount) >= maxHealth){
			health = maxHealth;
		} else {
			health += amount;
		}
		GD.Print("Health after: " + health);
	}

	private bool isFreeWeaponSlot(){
		foreach (Gun x in weapons){
			if (x == null){
				return true;
			}
		}
		return false;
	}

	//Processes the players inputs. Should be called every frame.
	public void GetInput() {
		//LookAt(GetGlobalMousePosition());
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
		//Fires the active weapon
		if (Input.IsActionPressed("shoot")){
			activeWeapon.fire();
		}
		//Swaps to weapon 1. Bound to "1"
		if (Input.IsActionJustPressed("weapon1")){
			swapWeapon(0);
		}
		//Swaps to weapon 2. Bound to "2"
		if(Input.IsActionJustPressed("weapon2")){
			swapWeapon(1);
		}
		//Swaps to weapon 3. Bound to "3"
		if(Input.IsActionJustPressed("weapon3")){
			swapWeapon(2);
		}
		//Reloads the active weapon. Bound to "R"
		if (Input.IsActionJustPressed("reload")){
			activeWeapon.reload();
		}
		
		velocity = velocity.Normalized() * speed;

	}

	
	public override void _Ready()
	{
		weapons = new Gun[3];

		//Initializes the spawn weapon
		PackedScene startingGun = GD.Load<PackedScene>("res://Scenes/Gun.tscn");
		weapons[0] = startingGun.Instance<Gun>();
		AddChild(weapons[0]);
		activeWeapon = weapons[0];

		health = maxHealth;

		//This is only here to test weapon swapping
		PackedScene machineGun = GD.Load<PackedScene>("res://Scenes/MachineGun.tscn");
		weapons[1] = machineGun.Instance<Gun>();
		AddChild(weapons[1]);
		weapons[1].Hide();
		weapons[1].SetProcess(false);
		

	}

	public override void _Process(float delta)
	{
		GetInput();
		velocity = MoveAndSlide(velocity);
	}
}
