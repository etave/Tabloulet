using System.Collections.Generic;
using Godot;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Scenes.Database;

public partial class Database : Node
{
    private SQLiteConnection _connection;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        _connection = new SQLiteConnection(Constants.DatabasePath);

        CreateTables();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private bool ExecuteQuery(string query)
    {
        try
        {
            int result = _connection.Execute(query);
            return result > 0;
        }
        catch (SQLiteException e)
        {
            GD.PrintErr($"Error executing query: {e.Message}");
            // TODO: Inform the user about the error in a more user-friendly way
            return false;
        }
    }

    private void CreateTables()
    {
        var createTableStatements = new List<string>()
        {
            Constants.CreateScenarioTable,
            Constants.CreatePageTable,
            Constants.CreateButtonTable,
            Constants.CreateTextTable,
            Constants.CreateImageTable,
            Constants.CreateVideoTable,
            Constants.CreateAudioTable,
            Constants.CreateModelTable,
        };

        foreach (var statement in createTableStatements)
        {
            ExecuteQuery(statement);
        }
    }
}
