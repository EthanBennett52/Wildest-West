using Godot;
using System;

public class KnifeBandit : Bandit
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        health = 30;
        speed = 300;
        weapon.damage = 75;
    }

    protected override void ApproachState(){
		weapon.LookAt(player.Position);
		velocity = Position.DirectionTo(player.Position) * speed;
		MoveAndSlide(velocity);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
