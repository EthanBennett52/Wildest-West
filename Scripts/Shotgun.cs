using Godot;
using System;

public class Shotgun : Gun
{
	public int bulletsPerShot = 5;
	public double spreadAngle = Math.PI/8;
	public override void _Ready()
	{
		fireRate = .5;
		damage = 15;
		maxLoadedCapacity = 5;
		maxAmmo = 36;
		ammo = 24;
		loaded = maxLoadedCapacity;
		reloadTime = 2;
		name = "Shotgun";
		base._Ready();
	}

	public override bool fire(){
		if (canShoot && loaded > 0){
			soundEffect.Stop();
			Vector2 startVector = new Vector2((float)Math.Cos(Rotation),(float)Math.Sin(Rotation)).Rotated((float)-spreadAngle/2);
			double angleDiff = spreadAngle / bulletsPerShot;
			for (int x = 1; x <= bulletsPerShot; x++){
				CreateBullet(startVector.Rotated((float)angleDiff * x));
			}
			canShoot = false;
			shotTimer = fireRate;
			loaded--;
			soundEffect.Play();
			anim.Play("MuzzleFlash");
			sprite.Frame = 1;
			return true;
		}
		return false;
	}

	public override void reload(){
		base.reload();
		sprite.Frame = 1;
	}

	public override void _Process(float delta)
	{
	  if (shotTimer <= 0){
		  sprite.Frame = 0;
	  }
	  base._Process(delta);
	}


}
