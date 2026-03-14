using Godot;
using System;

public partial class Menu : Control
{
	
	private void graj(){
		GetTree().ChangeSceneToFile("res://play.tscn");
	}
	private void wyjdz()
	{
		GetTree().Quit();
		
		
	}
}
