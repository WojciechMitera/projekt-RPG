using Godot;
using System;

/**
 * @class Enemy
 * @brief Represents an enemy character in the game.
 * 
 * This class handles enemy behavior such as taking damage,
 * tracking health, and dealing damage to the player upon contact.
 */
public partial class Enemy : CharacterBody2D
{
	/**
	 * @brief Damage dealt to the player on contact.
	 */
	public int _damage = 10;

	/**
	 * @brief Maximum health of the enemy.
	 */
	public int max_health = 50;

	/**
	 * @brief Current health of the enemy.
	 */
	private int health;

	/**
	 * @brief Indicates whether the enemy is in contact with the player.
	 */
	public bool contact = false;

	/**
	 * @brief Time interval between damage ticks while in contact.
	 */
	public float damagetime = 1f;

	/**
	 * @brief Initializes the enemy.
	 * 
	 * Sets the current health to the maximum health value.
	 */
	public override void _Ready()
	{
		health = max_health;
	}

	/**
	 * @brief Applies damage to the enemy.
	 * 
	 * Reduces the enemy's health by the given amount.
	 * If health reaches zero, the enemy is removed from the scene.
	 * 
	 * @param damage Amount of damage to apply.
	 */
	public void Damage(int damage)
	{
		health -= damage;

		if (health < 0)
		{
			health = 0;
		}
		else if (health == 0)
		{
			QueueFree();
		}

		GD.Print(health);
	}

	/**
	 * @brief Handles collision with another body.
	 * 
	 * If the colliding object is a player, the enemy starts
	 * dealing damage over time until the contact ends.
	 * 
	 * @param body The colliding node.
	 */
	private async void BodyCollision(Node body)
	{
		if (body is Player player)
		{
			contact = true;

			while (contact)
			{
				await ToSignal(GetTree().CreateTimer(damagetime), "timeout");

				if (contact == true)
					player.Damage(_damage);
			}
		}
	}

	/**
	 * @brief Handles when a body exits collision.
	 * 
	 * Stops dealing damage when the player is no longer in contact.
	 * 
	 * @param body The node leaving the collision.
	 */
	private void BodyCollisionOut(Node body)
	{
		if (body is Player player)
		{
			contact = false;
		}
	}
}
