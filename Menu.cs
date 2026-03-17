using Godot;
using System;

/**
 * @class Menu
 * @brief Main menu controller.
 * 
 * This class handles user interactions in the main menu,
 * such as starting the game or exiting the application.
 */
public partial class Menu : Control
{
	/**
	 * @brief Starts the game.
	 * 
	 * Changes the current scene to the gameplay scene.
	 * 
	 * @note The gameplay scene must exist at:
	 * "res://play.tscn".
	 */
	private void graj()
	{
		GetTree().ChangeSceneToFile("res://play.tscn");
	}

	/**
	 * @brief Exits the game.
	 * 
	 * Closes the application.
	 */
	private void wyjdz()
	{
		GetTree().Quit();
	}
}
