using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] public int speed = 300;
	//The players weapons
	private Gun[] weapons;
	//The weapon that is currently equiped
	private Gun activeWeapon;
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
		}
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
