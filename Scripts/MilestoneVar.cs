using Godot;
using System;
using System.IO;

public class MilestoneVar : Node{
	public bool extraMaxHealth = false;
	public bool extraScore = false;
	public bool extraRevolverDamage = false;
	public bool lowerDodgeCooldown = false;
	public bool extraLoadedCapacity = false;
	public bool extraExtraMaxHealth = false;

	public override void _Ready(){
		updateMilestones();
	}

	public void updateMilestones(){
		string[] lines = System.IO.File.ReadAllLines("Data/milestones.txt");
		foreach (String line in lines)
		{
			
			string[] split = line.Split(",");
			
			if(split[0] == "Kill"){

				if (split[5] == "enabled"){
					extraMaxHealth = true;
				} else {
					extraMaxHealth = false;
				}

			}else if(split[0] == "Score"){

				if (split[5] == "enabled"){
					extraScore = true;
				} else {
					extraScore = false;
				}	

			}else if(split[0] == "Revolver"){

				if (split[5] == "enabled"){
					extraRevolverDamage = true;
				} else {
					extraRevolverDamage = false;
				}	
				
			}else if(split[0] == "Damage"){
				
				if (split[5] == "enabled"){
					lowerDodgeCooldown = true;
				} else {
					lowerDodgeCooldown = false;
				}	

			}else if(split[0] == "Ammo"){
				
				if (split[5] == "enabled"){
					extraLoadedCapacity = true;
				} else {
					extraLoadedCapacity = false;
				}

			}else if(split[0] == "Deaths"){
				
				if (split[5] == "enabled"){
					extraExtraMaxHealth = true;
				} else {
					extraExtraMaxHealth = false;
				}

			}
		}
	}


}

