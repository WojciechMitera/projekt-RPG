using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	
	public int _damage = 10;
	public int max_health = 50;
	private int health;
	public bool contact = false;
	public float damagetime = 1f;
	public override void _Ready(){
		health = max_health;
		
	}
	
	public void Damage(int damage)
	{
		health -= damage;
		if (health < 0)
		{

			//GetTree().ChangeSceneToFile("res://menu.tscn");
			health = 0;

		}
		else if (health == 0)
		{
			QueueFree();
			
		}
		
		GD.Print(health);
	}
	private async void BodyCollision(Node body)
	{d
		if(body is Player player)
		{
			contact = true;
			while (contact)
			{
				await ToSignal(GetTree().CreateTimer(damagetime), "timeout");
				if(contact == true)
					player.Damage(_damage);
			}

		}
	}
	private void BodyCollisionOut(Node body)
	{
		if(body is Player player)
		{
			contact = false;
			//GD.Print("afa");
		}
	}
	

	}
