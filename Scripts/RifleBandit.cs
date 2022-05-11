using Godot;
using System;

public class RifleBandit : Bandit
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		health = 50;
		speed = 100;
		weapon.damage = 50;
		((Rifle)weapon).penetrate = false;
		weapon.soundEffectVolume(-10);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
