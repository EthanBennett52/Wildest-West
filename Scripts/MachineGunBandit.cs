using Godot;
using System;

public class MachineGunBandit : Bandit
{
    private Timer shooting;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        health = 100;
        speed = 120;
        weapon.damage = 10;
        shooting = GetNode<Timer>("ShootFor");
        shooting.Connect("timeout", this, "_OnTimeout");
        weapon.soundEffectVolume(-20);
    }

    protected override void ApproachState(){
		weapon.LookAt(player.Position);

		if (shotTimer.IsStopped()){
			weapon.fire();

            if(shooting.IsStopped()){
                shooting.Start();
            }
		}

		velocity = Position.DirectionTo(player.Position) * speed;
		MoveAndSlide(velocity);
	}

    protected override void InPositionState(){
		weapon.LookAt(player.Position);
		if (shotTimer.IsStopped()){
			weapon.fire();

			if(shooting.IsStopped()){
                shooting.Start();
            }
		}
	}

    private void _OnTimeout(){
        shotTimer.Start();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
