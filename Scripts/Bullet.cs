using Godot;
using System;

public class Bullet : Area2D
{
	//Speed of the bullet
	public int speed = 700;
	//The bullet's vector
	public Vector2 bulletVelocity = new Vector2();
	public NodePath creator;
	//How much damage the bullet deals. This should be controlled by the gun.
	private int damage;
	
	public override void _Ready()
	{
		Gun gun = GetNode<Gun>(creator);
		damage = gun.damage;
		bulletVelocity = gun.GlobalPosition.DirectionTo(gun.target) * speed;
		Connect("body_entered", this, "OnBodyEntered");   
	}

	public override void _Process(float delta)
	{
		Position += bulletVelocity * delta;
	}

	//This is called when the bullet enters an area2D node
	public void OnBodyEntered(Node2D area){
		if (area is Bandit){
			Bandit enemy = (Bandit)area;
			enemy.takeDamage(damage);
			QueueFree();
			//GetParent().RemoveChild(this);
		} else if (area is Player){
			Player player = (Player)area;
			player.takeDamage(damage);
			QueueFree();
		}
	}
	
}
