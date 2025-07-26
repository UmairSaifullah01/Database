using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace THEBADDEST.DatabaseModule
{
    /// <summary>
    /// Example data structure for demonstration
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public int score;
        public int level;
        public string email;
        public bool isActive;
    }

    /// <summary>
    /// Example table implementation
    /// </summary>
    [CreateAssetMenu(menuName = "Database/Tables/PlayerTable", fileName = "PlayerTable")]
    public class PlayerTable : Table<PlayerData>
    {
        // Custom table logic can be added here
    }

    /// <summary>
    /// Comprehensive examples of how to use the enhanced table system
    /// </summary>
    public static class TableExamples
    {
        /// <summary>
        /// Demonstrates basic CRUD operations
        /// </summary>
        public static void BasicCRUDExample(PlayerTable playerTable)
        {
            // Add records
            playerTable.AddRecord(new PlayerData { playerName = "Alice", score = 100, level = 1, email = "alice@game.com", isActive = true });
            playerTable.AddRecord(new PlayerData { playerName = "Bob", score = 250, level = 3, email = "bob@game.com", isActive = true });
            playerTable.AddRecord(new PlayerData { playerName = "Charlie", score = 75, level = 1, email = "charlie@game.com", isActive = false });

            // Get record by index
            var player = playerTable.GetRecord(0);
            Debug.Log($"Player at index 0: {player.playerName}");

            // Update record
            playerTable.UpdateRecord(0, new PlayerData { playerName = "Alice Updated", score = 150, level = 2, email = "alice@game.com", isActive = true });

            // Remove record
            playerTable.RemoveRecord(2);
        }

        /// <summary>
        /// Demonstrates querying capabilities
        /// </summary>
        public static void QueryingExample(PlayerTable playerTable)
        {
            // Basic filtering
            var activePlayers = playerTable.Where(p => p.isActive);
            var highScorePlayers = playerTable.Where(p => p.score > 100);

            // First/Last operations
            var firstPlayer = playerTable.FirstOrDefault(p => p.level > 1);
            var lastActivePlayer = playerTable.LastOrDefault(p => p.isActive);

            // Counting
            int activeCount = playerTable.Count(p => p.isActive);
            int highLevelCount = playerTable.Count(p => p.level >= 3);

            // Checking conditions
            bool hasActivePlayers = playerTable.Any(p => p.isActive);
            bool allPlayersHaveEmail = playerTable.All(p => !string.IsNullOrEmpty(p.email));

            // Projection
            var playerNames = playerTable.Select(p => p.playerName);
            var playerScores = playerTable.Select(p => p.score);
        }

        /// <summary>
        /// Demonstrates sorting capabilities
        /// </summary>
        public static void SortingExample(PlayerTable playerTable)
        {
            // Sort by score (ascending)
            var sortedByScore = playerTable.OrderBy(p => p.score);

            // Sort by score (descending)
            var topPlayers = playerTable.OrderByDescending(p => p.score);

            // Sort by multiple criteria
            // var sortedByLevelThenScore = playerTable.OrderBy(p => p.level).ThenBy(p => p.score);

            // Get top 5 players
            var top5Players = playerTable.Top(p => p.score, 5);

            // Get bottom 3 players
            var bottom3Players = playerTable.Bottom(p => p.score, 3);
        }

        /// <summary>
        /// Demonstrates indexing for fast lookups
        /// </summary>
        public static void IndexingExample(PlayerTable playerTable)
        {
            // Create index on player name for fast lookups
            playerTable.CreateIndex(p => p.playerName, "PlayerNameIndex");

            // Create index on email
            playerTable.CreateIndex(p => p.email, "EmailIndex");

            // Fast lookup by indexed field
            var alicePlayers = playerTable.GetByIndex(p => p.playerName, "Alice");
            var bobPlayers = playerTable.GetByIndex(p => p.playerName, "Bob");

            // Rebuild indexes after significant changes
            playerTable.RebuildIndexes();
        }

        /// <summary>
        /// Demonstrates bulk operations
        /// </summary>
        public static void BulkOperationsExample(PlayerTable playerTable)
        {
            // Add multiple records at once
            var newPlayers = new List<PlayerData>
            {
                new PlayerData { playerName = "David", score = 300, level = 4, email = "david@game.com", isActive = true },
                new PlayerData { playerName = "Eve", score = 180, level = 2, email = "eve@game.com", isActive = true },
                new PlayerData { playerName = "Frank", score = 90, level = 1, email = "frank@game.com", isActive = false }
            };
            playerTable.AddRange(newPlayers);

            // Remove all inactive players
            int removedCount = playerTable.RemoveAll(p => !p.isActive);

            // Update all players with level 1 to have score 50
            playerTable.UpdateAll(p => p.level == 1, p => new PlayerData
            {
                playerName = p.playerName,
                score = 50,
                level = p.level,
                email = p.email,
                isActive = p.isActive
            });
        }

        /// <summary>
        /// Demonstrates utility methods
        /// </summary>
        public static void UtilityMethodsExample(PlayerTable playerTable)
        {
            // Get random player
            var randomPlayer = playerTable.GetRandom();

            // Get 3 random players
            var randomPlayers = playerTable.GetRandom(3);

            // Check if table contains specific player
            var testPlayer = new PlayerData { playerName = "Alice", score = 100 };
            bool containsPlayer = playerTable.Contains(testPlayer);

            // Find index of player
            int playerIndex = playerTable.IndexOf(testPlayer);

            // Convert to array/list
            var playerArray = playerTable.ToArray();
            var playerList = playerTable.ToList();
        }

        /// <summary>
        /// Demonstrates extension methods
        /// </summary>
        public static void ExtensionMethodsExample(PlayerTable playerTable)
        {
            // Range queries
            var midLevelPlayers = playerTable.Between(p => p.level, 2, 4);

            // Grouping
            var playersByLevel = playerTable.GroupBy(p => p.level);

            // Aggregations
            double averageScore = playerTable.Average(p => p.score);
            double totalScore = playerTable.Sum(p => p.score);
            int maxScore = playerTable.Max(p => p.score);
            int minScore = playerTable.Min(p => p.score);

            // Distinct values
            var distinctLevels = playerTable.Distinct(p => p.level);

            // Text search
            var playersWithA = playerTable.ContainsText(p => p.playerName, "a");
            var playersStartingWithA = playerTable.StartsWith(p => p.playerName, "A");
            var playersEndingWithE = playerTable.EndsWith(p => p.playerName, "e");

            // Pagination
            var page1Players = playerTable.GetPage(1, 10); // First 10 players
            var page2Players = playerTable.GetPage(2, 10); // Next 10 players
            int totalPages = playerTable.GetPageCount(10);

            // Shuffle
            var shuffledPlayers = playerTable.Shuffle();

            // Dictionary conversion
            var playerDict = playerTable.ToDictionary(p => p.playerName);
            var playerByName = playerTable.GetByKey(p => p.playerName, "Alice");
        }

        /// <summary>
        /// Demonstrates complex queries
        /// </summary>
        public static void ComplexQueriesExample(PlayerTable playerTable)
        {
            // Complex filtering with multiple conditions
            var activeHighLevelPlayers = playerTable.Where(p => p.isActive && p.level >= 3 && p.score > 200);

            // Chaining operations
            var topActivePlayers = playerTable
                .Where(p => p.isActive)
                .OrderByDescending(p => p.score)
                .Take(5);

            // Aggregation with filtering
            double averageScoreOfActivePlayers = playerTable
                .Where(p => p.isActive)
                .Average(p => p.score);

            // Group by level and get average score per level
            var averageScoreByLevel = playerTable
                .GroupBy(p => p.level)
                .Select(g => new { Level = g.Key, AverageScore = g.Average(p => p.score) });

            // Find players with duplicate emails
            var duplicateEmails = playerTable
                .GroupBy(p => p.email)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
        }

        /// <summary>
        /// Demonstrates performance optimization
        /// </summary>
        public static void PerformanceOptimizationExample(PlayerTable playerTable)
        {
            // Create indexes for frequently queried fields
            playerTable.CreateIndex(p => p.playerName, "NameIndex");
            playerTable.CreateIndex(p => p.email, "EmailIndex");
            playerTable.CreateIndex(p => p.level, "LevelIndex");

            // Use indexed lookups for better performance
            var alice = playerTable.GetByIndex(p => p.playerName, "Alice").FirstOrDefault();

            // Clear sorting cache when not needed
            playerTable.ClearSorting();

            // Rebuild indexes after bulk operations
            playerTable.RebuildIndexes();
        }
    }
}