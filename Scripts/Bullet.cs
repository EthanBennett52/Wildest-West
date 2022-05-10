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
	private bool penetration = false;
	private int penetrationLayers = 1;
	private int penetrated = 1;
	
	public override void _Ready()
	{
		AddToGroup("Bullets");
		AddToGroup("destroy_on_level_change");
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

	public void EnablePenetration(int layers){
		penetration = true;
		penetrationLayers = layers;
	}

	private void Destroy(){
		QueueFree();
	}

	//This is called when the bullet enters an area2D node
	public void OnBodyEntered(Node2D area){

		//This is bad. Hit detection should be handled by the Player/Bandit nodes.
		
		if (area is Damageable){
			if(((Damageable)area).takeDamage(damage)){
				if(!penetration || penetrated == penetrationLayers){
					QueueFree();
				} else{
					penetrated++;
				}
				
			}
		} else if (area.Name ==  "TopMap"){
			Vector2 tilePosition = ((TileMap)area).WorldToMap(GlobalPosition);
			((TileMap)area).SetCellv(tilePosition, -1); 
			if(!penetration || penetrated == penetrationLayers){
					QueueFree();
				} else{
					penetrated++;
				}
		}
	}
	
}
