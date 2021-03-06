using Godot;
using System;

public class WeaponPickup : Area2D
{
	[Export] String startWeapon = "Knife";
	public Gun weapon;
	private Sprite weaponSprite;
	private Sprite keySprite;
	private Player player;
	private bool playerInRange = false;
	
	

	//Changes what weapon the pickup contains. The new weapon must be instanced beforehand.
	public void setWeapon(Gun weapon) {
		AddChild(weapon);
		this.weapon = weapon;
		weaponSprite.Texture = weapon.sprite.Texture;
		if (weapon is Rifle || weapon is Shotgun){
			weaponSprite.Hframes = 2;
			weaponSprite.Frame = 0; 
		}
	}

	//Sets the weapon pickup to contain a newly created weapon. Options: Revolver, MachineGun
	public void setWeapon(String weaponString){
		PackedScene weaponScene;
		Gun temp;
		switch(weaponString){
			case "Revolver":
				weaponScene = GD.Load<PackedScene>("res://Scenes/Gun.tscn");
				temp = weaponScene.Instance<Gun>();
				setWeapon(temp);
				break;
			case "MachineGun":
				weaponScene = GD.Load<PackedScene>("res://Scenes/MachineGun.tscn");
				temp = weaponScene.Instance<Gun>();
				setWeapon(temp);
				break;
			case "Shotgun":
				weaponScene = GD.Load<PackedScene>("res://Scenes/Shotgun.tscn");
				temp = weaponScene.Instance<Gun>();
				setWeapon(temp);
				break;
			case "Rifle":
				weaponScene = GD.Load<PackedScene>("res://Scenes/Rifle.tscn");
				temp = weaponScene.Instance<Gun>();
				setWeapon(temp);
				break;
			case "Knife":
				weaponScene = GD.Load<PackedScene>("res://Scenes/Knife.tscn");
				temp = weaponScene.Instance<Gun>();
				setWeapon(temp);
				break;
		}
	}

	//Detects when the player is range of the pickup. Displays the press "E" hint.
	private void OnBodyEntered(Node2D area){
		if (area is Player){
			player = (Player)area;
			playerInRange = true;
			keySprite.Show();
		}
	}

	//Detects when the player goes out of range of the pickup. Hides "E" hint.
	private void OnBodyExited(Node2D area){
		if (area is Player){
			playerInRange = false;
			keySprite.Hide();
		}
	}
	public void Destroy(){
		QueueFree();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("destroy_on_level_change");
		weaponSprite = (Sprite)FindNode("WeaponSprite");
		keySprite = (Sprite)FindNode("KeySprite");
		//keySprite.Hide();
		Connect("body_entered", this, "OnBodyEntered");
		Connect("body_exited", this, "OnBodyExited");


		//For testing.
		setWeapon(startWeapon);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	  if (playerInRange && Input.IsActionJustPressed("pickup")){
		  RemoveChild(weapon);
		  player.pickupWeapon(weapon);
		  QueueFree();
	  }

  }
}
