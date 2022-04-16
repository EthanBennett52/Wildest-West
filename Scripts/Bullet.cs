using Godot;
using System;

public class Bullet : Area2D
{
    private int speed = 700;
    public Vector2 bulletVelocity = new Vector2();
    public NodePath creator;

    private int damage = 25;
    
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Gun gun = GetNode<Gun>(creator);
        bulletVelocity = gun.GlobalPosition.DirectionTo(gun.target) * speed;
        Connect("body_entered", this, "OnBodyEntered");   
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Position += bulletVelocity * delta;
    }

    public void OnBodyEntered(Node area){
        if (area.Name == "Bandit"){
            Bandit enemy = (Bandit)area;
            enemy.takeDamage(damage);
            QueueFree();
            //GetParent().RemoveChild(this);
        } else if (area.Name == "Player"){
            Player player = (Player)area;
            player.takeDamage(damage);
            QueueFree();
        }
    }
    
}
