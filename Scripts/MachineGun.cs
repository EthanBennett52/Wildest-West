using Godot;
using System;

public class MachineGun : Gun
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fireRate = .1;
		damage = 10;
		maxLoadedCapacity = 20;
		loaded = maxLoadedCapacity;
		reloadTime = 2;
		incaccuarcy = Math.PI/12;
		name = "MachineGun";
		base._Ready();
	}


}
