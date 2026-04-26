using Godot;
using System;

/**
 * @class Menu
 * @brief Controls the main menu interface.
 * 
 * This class handles navigation between scenes such as starting the game,
 * opening settings, and exiting the application.
 */
public partial class Menu : Control
{
    /**
	 * @brief Starts the game.
	 * 
	 * Switches the current scene to the main gameplay scene.
	 */
    private void graj()
    {
        GetTree().ChangeSceneToFile("res://play.tscn");
    }

    /**
	 * @brief Opens the settings menu.
	 * 
	 * Switches the current scene to the settings scene.
	 */
    private void ustawienia()
    {
        GetTree().ChangeSceneToFile("res://ustawienia.tscn");
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