using Godot;
using System;

public class Bullet : KinematicBody2D
{
    private int speed = 1000;
    public Vector2 bulletVelocity = new Vector2();
    
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bulletVelocity = Position.DirectionTo(GetGlobalMousePosition()) * speed;
        bulletVelocity = MoveAndSlide(bulletVelocity);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
