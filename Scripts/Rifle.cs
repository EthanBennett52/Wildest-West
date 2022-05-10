using Godot;
using System;

public class Rifle : Gun
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        fireRate = .9;
		damage = 75;
		maxLoadedCapacity = 5;
        maxAmmo = 30;
        ammo = 15;
		loaded = maxLoadedCapacity;
		reloadTime = 2;
		incaccuarcy = 0.0;
		name = "Rifle";
		base._Ready();
    }

    protected override void CreateBullet(Vector2 bulletTarget){
		Bullet bullet = (Bullet)BULLET.Instance(); 
		bullet.Position = new Vector2((float) (GlobalPosition.x + 50 * Math.Cos(Rotation)), (float) (GlobalPosition.y + 50 * Math.Sin(Rotation))); 
		bullet.RotationDegrees = RotationDegrees;
		bullet.setDamage(damage);
        bullet.EnablePenetration(2);
        bullet.speed = 1000;
		bullet.SetTargetVector(bulletTarget);  
		GetNode("/root/Main").AddChild(bullet);
	}

    public override bool fire(){
        if(base.fire()){
            sprite.Frame = 1;
            return true;
        }
        return false;
    }

    public override void reload(){
        base.reload();
        sprite.Frame = 1;
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
      if (shotTimer <= 0){
          sprite.Frame = 0;
      }
      base._Process(delta);
    }
}
