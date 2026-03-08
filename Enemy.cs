using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	
	public int _damage = 10;
	public int max_health = 50;
	private int health;
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
	private void BodyCollision(Node body)
	{
		if(body is Player player)
		{
			player.Damage(_damage);
			
		}
	}
	
}
