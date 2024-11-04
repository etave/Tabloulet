using System;

namespace Tabloulet.Helpers
{
    public static class Constants
    {
        public static readonly string DatabasePath;

        static Constants()
        {
            DatabasePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DatabasePath = System.IO.Path.Combine(DatabasePath, "Tabloulet");
            System.IO.Directory.CreateDirectory(DatabasePath);
            DatabasePath = System.IO.Path.Combine(DatabasePath, "Tabloulet.db");
        }

        public const string ScenarioTable = "Scenario";
        public const string PageTable = "Page";
        public const string ButtonTable = "Button";
        public const string TextTable = "Text";
        public const string ImageTable = "Image";
        public const string VideoTable = "Video";
        public const string AudioTable = "Audio";
        public const string ModelTable = "Model";
        public const string ScenarioPageTable = "ScenarioPage";

        public const string CreateScenarioTable =
            $"CREATE TABLE IF NOT EXISTS {ScenarioTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Name TEXT NOT NULL, Description TEXT, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreatePageTable =
            $"CREATE TABLE IF NOT EXISTS {PageTable} (Id TEXT PRIMARY KEY)";
        public const string CreateButtonTable =
            $"CREATE TABLE IF NOT EXISTS {ButtonTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Content TEXT NOT NULL, Color TEXT NOT NULL, Height REAL NOT NULL, Width REAL NOT NULL, PositionX REAL NOT NULL, PositionY REAL NOT NULL, Rotation REAL NOT NULL, IsMovable BOOLEAN NOT NULL, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreateTextTable =
            $"CREATE TABLE IF NOT EXISTS {TextTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Content TEXT NOT NULL, Font TEXT, FontSize INTEGER NOT NULL, Height REAL NOT NULL, Width REAL NOT NULL, PositionX REAL NOT NULL, PositionY REAL NOT NULL, Rotation REAL NOT NULL, IsMovable BOOLEAN NOT NULL, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreateImageTable =
            $"CREATE TABLE IF NOT EXISTS {ImageTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Path TEXT NOT NULL, Height REAL NOT NULL, Width REAL NOT NULL, PositionX REAL NOT NULL, PositionY REAL NOT NULL, Rotation REAL NOT NULL, IsMovable BOOLEAN NOT NULL, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreateVideoTable =
            $"CREATE TABLE IF NOT EXISTS {VideoTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Path TEXT NOT NULL, Height REAL NOT NULL, Width REAL NOT NULL, PositionX REAL NOT NULL, PositionY REAL NOT NULL, Rotation REAL NOT NULL, IsMovable BOOLEAN NOT NULL, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreateAudioTable =
            $"CREATE TABLE IF NOT EXISTS {AudioTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Path TEXT NOT NULL, Height REAL NOT NULL, Width REAL NOT NULL, PositionX REAL NOT NULL, PositionY REAL NOT NULL, Rotation REAL NOT NULL, IsMovable BOOLEAN NOT NULL, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreateModelTable =
            $"CREATE TABLE IF NOT EXISTS {ModelTable} (Id TEXT PRIMARY KEY, PageId TEXT NOT NULL, Path TEXT NOT NULL, Height REAL NOT NULL, Width REAL NOT NULL, PositionX REAL NOT NULL, PositionY REAL NOT NULL, Rotation REAL NOT NULL, IsMovable BOOLEAN NOT NULL, FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
        public const string CreateScenarioPageTable =
            $"CREATE TABLE IF NOT EXISTS {ScenarioPageTable} (Id TEXT PRIMARY KEY, ScenarioId TEXT NOT NULL, PageId TEXT NOT NULL, FOREIGN KEY(ScenarioId) REFERENCES {ScenarioTable}(Id), FOREIGN KEY(PageId) REFERENCES {PageTable}(Id))";
    }
}
