using Godot;
using System;

public class Gun : Node2D
{
    PackedScene BULLET = GD.Load<PackedScene>("res://Bullet.tscn");
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        LookAt(GetGlobalMousePosition());
        if (Input.IsActionJustPressed("shoot")) {
            var bullet = BULLET.Instance(); 
            AddChild(bullet);
        }
    }
}
