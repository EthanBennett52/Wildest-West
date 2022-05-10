using Godot;
using System;
using System.IO;

public class Bandit : KinematicBody2D, Damageable
{
	protected int health = 100;
	protected int speed = 150;
	private int scoreWorth = 10;
	private bool justPatroled = false;
	protected Gun weapon;
	protected Timer shotTimer;
	protected Timer patrolTimer;
	private Area2D approachRange;
	private Area2D inPosition;
	protected Player player;
	private AIState state = AIState.PATROL;
	protected Vector2 velocity = new Vector2();
	protected RandomNumberGenerator rand = new RandomNumberGenerator();
	

	protected enum AIState{
		PATROL,
		APPROACH,
		INPOSITION
	}

	public bool takeDamage(int damage){
		health = health - damage;
		return true;
	}

	public void Destroy(){
		QueueFree();
	}

	private void onDeath(){
		dropItem();

		QueueFree();

		updateScore(scoreWorth);
		addKillToMilestone();
	}
	
	private void dropItem(){
		
		int rInt = rand.RandiRange(0, 100); //for ints

		if(rInt <= 20){
			PackedScene weaponDrop = GD.Load<PackedScene>("res://Scenes/WeaponPickup.tscn");
			WeaponPickup drop = weaponDrop.Instance<WeaponPickup>();
			drop.Position = Position;
			GetParent().AddChild(drop);
			drop.setWeapon(weapon.name);
		}else if (rInt <= 60){
			PackedScene ammoDrop = GD.Load<PackedScene>("res://Scenes/AmmoPickup.tscn");
			AmmoPickup drop = ammoDrop.Instance<AmmoPickup>();
			drop.Position = Position;
			GetParent().AddChild(drop);
			drop.setAmmoAmount(20);
		}else if (rInt <= 100){
			PackedScene healthDrop = GD.Load<PackedScene>("res://Scenes/HealthPickup.tscn");
			HealthPickup drop = healthDrop.Instance<HealthPickup>();
			drop.Position = Position;
			GetParent().AddChild(drop);
			drop.setHealAmount(25);
		}
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

	// private void onTimeout(){
	// 	weapon.fire();
	// }

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

	protected virtual void PatrolState(){
		if (patrolTimer.IsStopped()){
			rand.Randomize();
			if (justPatroled){

				justPatroled = false;

				float patrolTime = rand.RandfRange((float)0.7, (float)1.5);
				patrolTimer.Start();

				velocity = new Vector2();

			} else {

				justPatroled = true;

				float patrolTime = rand.RandfRange((float)1.5, (float)3.5);
				patrolTimer.WaitTime = patrolTime;
				patrolTimer.Start();

				Vector2 patrolDestination = new Vector2(rand.RandfRange((float)-1.0, (float)1.0), rand.RandfRange((float)-1.0, (float)1.0));
				velocity = patrolDestination.Normalized() * speed/2;

			}
		}

		MoveAndAvoid(velocity);

		//shotTimer.Stop();
		weapon.LookAt(Position + velocity);
	}

	protected void MoveAndAvoid(Vector2 dest){
		MoveAndSlide(dest);
		if (IsOnWall()){
			velocity = velocity * -1;
		}	
	}
	protected virtual void ApproachState(){
		weapon.LookAt(player.Position);
		if (shotTimer.IsStopped()){
			weapon.fire();
			shotTimer.Start();
		}
		velocity = Position.DirectionTo(player.Position) * speed;
		MoveAndSlide(velocity);
	}
	protected virtual void InPositionState(){
		weapon.LookAt(player.Position);
		if (shotTimer.IsStopped()){
			weapon.fire();
			shotTimer.Start();
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("destroy_on_level_change");
		AddToGroup("Enemy");
		approachRange = (Area2D)FindNode("ApproachRange");
		inPosition = (Area2D)FindNode("InPosition");
		approachRange.Connect("body_entered", this, "PlayerSpotted");
		approachRange.Connect("body_exited", this, "OutOfRange");
		inPosition.Connect("body_entered", this, "InPosition");
		inPosition.Connect("body_exited", this, "PlayerSpotted");

		weapon = GetChild<Gun>(1);
		weapon.loaded = 999;
		weapon.soundEffectVolume(-25);
		shotTimer = (Timer)FindNode("ShotTimer");
		shotTimer.OneShot = true;
		patrolTimer = (Timer)FindNode("PatrolTimer");
		
		rand.Randomize();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  	public override void _Process(float delta)
  	{
		switch (state){
			case AIState.PATROL:
				PatrolState();
				break;
			case AIState.APPROACH:
				ApproachState();
				break;
			case AIState.INPOSITION:
				InPositionState();
				break;
	  }

	  if (health <= 0) {
		  onDeath();
	  }
  }
}
