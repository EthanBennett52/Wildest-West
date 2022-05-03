using Godot;
using System;
using System.IO;

public class Bandit : KinematicBody2D
{
	private int health = 100;
	protected int speed = 150;
	private int scoreWorth = 10;
	private Gun weapon;
	private Timer shotTimer;
	private Area2D approachRange;
	private Area2D inPosition;
	private Player player;
	private AIState state = AIState.PATROL;
	

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

		updateScore(scoreWorth);
		addKillToMilestone();
	}
	
	private void updateScore(int amount){
		String[] score = System.IO.File.ReadAllLines("interface/Score.txt");
		System.IO.File.WriteAllText("interface/Score.txt" , (int.Parse(score[0]) + amount).ToString());
	}
	
	private void addKillToMilestone(){
		string[] lines = System.IO.File.ReadAllLines("milestone_screen/milestones.txt");
		string updatedText = "";
		foreach (String line in lines)
		{
			
			string[] split = line.Split(",");
			
			if(split[0] == "Kill"){
				int kills = int.Parse(split[2]) + 1;
				updatedText += (split[0] + "," + split[1] + "," + kills + "," + split[3] + "," + split[4] + "," + split[5] + "\n");
			}else{
				updatedText += (line + "\n");
			}
		}
		System.IO.File.WriteAllText("milestone_screen/milestones.txt", updatedText);
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
				Position = Position.MoveToward(player.Position, delta * speed);
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
