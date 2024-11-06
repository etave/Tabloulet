using System;
using System.Collections.Generic;
using Godot;
using SQLite;
using Tabloulet.DatabaseNS.Models;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS
{
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

        public bool Insert(IDatabaseModel obj)
        {
            try
            {
                int result = _connection.Insert(obj);
                return result > 0;
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error inserting object: {e.Message}");
                // TODO: Inform the user about the error in a more user-friendly way
                return false;
            }
        }

        public bool Delete(IDatabaseModel obj)
        {
            try
            {
                int result = _connection.Delete(obj);
                return result > 0;
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error deleting object: {e.Message}");
                // TODO: Inform the user about the error in a more user-friendly way
                return false;
            }
        }

        public bool Delete(IDatabaseModelComponent obj)
        {
            try
            {
                int result = _connection.Delete(obj);
                return result > 0;
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error deleting object: {e.Message}");
                // TODO: Inform the user about the error in a more user-friendly way
                return false;
            }
        }

        public T GetById<T>(Guid guid)
            where T : IDatabaseModel, new()
        {
            try
            {
                return _connection.Table<T>().FirstOrDefault(x => x.Id == guid);
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error getting object by id: {e.Message}");
                // TODO: Inform the user about the error in a more user-friendly way
                return default;
            }
        }

        public TableQuery<T> GetTableComponentsByPageId<T>(Guid pageId)
            where T : IDatabaseModelComponent, new()
        {
            return _connection.Table<T>().Where(x => x.PageId == pageId);
        }

        public SQLite.TableQuery<T> GetAllByType<T>()
            where T : IDatabaseModel, new()
        {
            try
            {
                return _connection.Table<T>();
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error getting object by id: {e.Message}");
                // TODO: Inform the user about the error in a more user-friendly way
                return null;
            }
        }

        public List<Page> GetPagesByScenario(Guid scenarioId)
        {
            SQLite.TableQuery<ScenarioPage> scenarioPages = _connection
                .Table<ScenarioPage>()
                .Where(x => x.ScenarioId == scenarioId);
            List<Page> pages = [];
            foreach (var page in scenarioPages)
            {
                pages.Add(GetById<Page>(page.PageId) as Page);
            }
            return pages;
        }

        public List<IDatabaseModel> GetElementsByPage(Guid pageId)
        {
            List<IDatabaseModel> elements = new List<IDatabaseModel>();
            elements.AddRange(_connection.Table<Models.Button>().Where(x => x.PageId == pageId));
            elements.AddRange(_connection.Table<Models.Text>().Where(x => x.PageId == pageId));
            elements.AddRange(_connection.Table<Models.Image>().Where(x => x.PageId == pageId));
            elements.AddRange(_connection.Table<Models.Video>().Where(x => x.PageId == pageId));
            elements.AddRange(_connection.Table<Models.Audio>().Where(x => x.PageId == pageId));
            elements.AddRange(_connection.Table<Models.Model>().Where(x => x.PageId == pageId));
            return elements;
        }

        public ScenarioPage GetScenarioPageByPage(Guid pageId)
        {
            return _connection.Table<ScenarioPage>().FirstOrDefault(x => x.PageId == pageId);
        }

        public bool Update(IDatabaseModel obj)
        {
            try
            {
                int result = _connection.Update(obj);
                return result > 0;
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error updating object: {e.Message}");
                // TODO: Inform the user about the error in a more user-friendly way
                return false;
            }
        }

        public bool Update(IDatabaseModelComponent obj)
        {
            try
            {
                int result = _connection.Update(obj);
                return result > 0;
            }
            catch (SQLiteException e)
            {
                GD.PrintErr($"Error updating object: {e.Message}");
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
                Constants.CreateScenarioPageTable,
            };

            foreach (var statement in createTableStatements)
            {
                ExecuteQuery(statement);
            }
        }
    }
}
