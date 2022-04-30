using Godot;
using System;

public class Bandit : KinematicBody2D
{
	private int health = 100;
	protected int speed = 150;
	private Gun weapon;
	private Timer shotTimer;
	private Area2D approachRange;
    private Area2D inPosition;
	private Player player;
	private AIState state = AIState.PATROL;
	private Vector2 velocity = new Vector2();

	protected enum AIState{
		PATROL,
		APPROACH,
		INPOSITION
	}

	public void takeDamage(int damage){
		health = health - damage;
	}

	private void onDeath(){
		PackedScene weaponDrop = GD.Load<PackedScene>("res://Scenes/WeaponPickup.tscn");
		WeaponPickup drop = weaponDrop.Instance<WeaponPickup>();
		drop.Position = Position;
		GetParent().AddChild(drop);
		drop.setWeapon("MachineGun");

		QueueFree();
	}

	private void onTimeout(){
		weapon.fire();
	}

	private void PlayerSpotted(Node2D area){
		if (area is Player){
			player = (Player)area;
			state = AIState.APPROACH;
		}
		
	}
	private void OutOfRange(Node area){
		if (area is Player){
			state = AIState.PATROL;
		}
	}
	private void InPosition(Node area){
		if (area is Player){
			state = AIState.INPOSITION;
		}
	}



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		approachRange = (Area2D)FindNode("ApproachRange");
        inPosition = (Area2D)FindNode("InPosition");
		approachRange.Connect("body_entered", this, "PlayerSpotted");
		approachRange.Connect("body_exited", this, "OutOfRange");
		inPosition.Connect("body_entered", this, "InPosition");
		inPosition.Connect("body_exited", this, "PlayerSpotted");

		weapon = (Gun)FindNode("Gun");
		weapon.loaded = 999;
		shotTimer = (Timer)FindNode("ShotTimer");
		shotTimer.OneShot = false;
		//shotTimer.Start();
		shotTimer.Connect("timeout", this, "onTimeout");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  	public override void _Process(float delta)
  	{
		switch (state){
			case AIState.PATROL:
				shotTimer.Stop();
				break;
			case AIState.APPROACH:
				if (shotTimer.IsStopped()){
					shotTimer.Start();
				}
				velocity = Position.DirectionTo(player.Position) * speed;
				MoveAndSlide(velocity);
				break;
			case AIState.INPOSITION:
				if (shotTimer.IsStopped()){
					shotTimer.Start();
				}
				break;
	  }

	  if (health <= 0) {
		  onDeath();
	  }
  }
}
