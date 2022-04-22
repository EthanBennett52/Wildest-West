using Godot;
using System;

public class MachineGun : Gun
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		fireRate = .1;
		damage = 10;
		maxLoadedCapacity = 20;
		loaded = maxLoadedCapacity;
		reloadTime = 3;
	}


}
