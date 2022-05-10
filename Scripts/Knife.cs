using Godot;
using System;

public class Knife : Gun
{
	private Area2D stabArea;
	private bool areaEnabled = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = GetParent<Node2D>();
		sprite = (Sprite)GetChild(0);
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		stabArea = GetNode<Area2D>("StabArea");
		stabArea.Connect("body_entered", this, "_OnBodyEntered");
		stabArea.Connect("area_entered", this, "_OnAreaEntered");

		

		if (parent is WeaponPickup){
			dropped();
		} 
		soundEffect = GetNode<AudioStreamPlayer2D>("SoundEffect");
		name = "Knife";
		damage = 100;
	}

	private void _OnBodyEntered(Node2D area){
		if(area == parent){
			return;
		}
		if (area is Damageable){
			((Damageable)area).takeDamage(damage);
		}
	}

	private void _OnAreaEntered(Node2D area){
		if(area is Bullet){
			area.QueueFree();
		}
		
	}

	public override void reload(){
		
	}

	public void ToggleKnifeHitbox(){
		if (areaEnabled){
			areaEnabled = false;
			stabArea.Monitoring = false;
		}else {
			areaEnabled = true;
			stabArea.Monitoring = true;
		}
	}

	public override bool fire(){
		if (canShoot){
			soundEffect.Stop();
			
			canShoot = false;
			shotTimer = fireRate;
			
			soundEffect.Play();
			anim.Play("Stab");
			return true;
		} 
		return false;
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
