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
	private int damage = 10;
	
	public override void _Ready()
	{
		AddToGroup("Bullets");
		Connect("body_entered", this, "OnBodyEntered");
	}
	public void setDamage(int damage){
		this.damage = damage;
	}

	public void SetTargetVector(Vector2 target){
		bulletVelocity = target;
	}

	public override void _Process(float delta)
	{
		Position += bulletVelocity * speed * delta;
	}

	private void Destroy(){
		QueueFree();
	}

	//This is called when the bullet enters an area2D node
	public void OnBodyEntered(Node2D area){

		//This is bad. Hit detection should be handled by the Player/Bandit nodes.
		
		if (area is Damageable){
			if(((Damageable)area).takeDamage(damage)){
				QueueFree();
			}
		} else if (area.Name ==  "TopMap"){
			QueueFree();
		}
	}
	
}
