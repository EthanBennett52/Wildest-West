using Godot;
using System;

public class Bandit : KinematicBody2D
{
	private int health = 100;
	private Gun weapon;
	private Timer shotTimer;

	public void takeDamage(int damage){
		health = health - damage;
	}

	private void onDeath(){
		QueueFree();
	}

	private void onTimeout(){
		weapon.fire();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		weapon = (Gun)FindNode("Gun");
		weapon.loaded = 999;
		shotTimer = (Timer)FindNode("ShotTimer");
		shotTimer.OneShot = false;
		shotTimer.Start();
		shotTimer.Connect("timeout", this, "onTimeout");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
	  if (health <= 0) {
		  onDeath();
	  }
  }
}
