using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Tommy;
using UnityEngine;
using EnemyStruct;
using WaypointPath;

public class StageEnemyParser
{
    /// <summary>
    /// Parses stage enemies data from a TOML file
    /// </summary>
    /// <param name="filepath">Path to the file, relative from "\Scripts\Game\Stage\"</param>
    /// <param name="ids">List of IDs passed by reference</param>
    public List<Enemy> ParseStageEnemies(string filepath, ref List<int> ids)
    {
        List<Enemy> enemies = new();

        using (StreamReader reader = File.OpenText(filepath))
        {
            TomlTable table;
            int id = ids.Max() + 1;

            try
            {
                table = TOML.Parse(reader);
            }
            catch (TomlParseException ex)
            {
                table = ex.ParsedTable;

                foreach(TomlSyntaxException syntaxEx in ex.SyntaxErrors)
                    Console.WriteLine($"Error on {syntaxEx.Column}:{syntaxEx.Line}: {syntaxEx.Message}");
            }

            foreach(TomlNode node in table["Enemy"])
            {
                Enemy enemy = new();

                enemy.ID = id;
                ids.Add(id++);
                enemy.Name = node.HasValue ? node : "Enemy";
                if (node["SpawnTime"].HasValue) enemy.SpawnTime = node["SpawnTime"];
                else
                {
                    Debug.LogError($"ParseEnemiesFile: {enemy.Name} doesn't have a spawn time, discarding!");
                    continue;
                }
                if (node["SpawnPosition"].HasValue) enemy.SpawnPosition = new Vector2(node["SpawnPosition"][0], node["SpawnPosition"][1]);
                else
                {
                    Debug.LogError($"ParseEnemiesFile: {enemy.Name} doesn't have a spawn position, discarding!");
                    continue;
                }

                List<Vector2> Waypoints = new();

                if(!(node["PathLengths"].HasValue && node["PathExpressions"].HasValue && node["PathAngles"].HasValue))
                {
                    Debug.LogWarning($"ParseEnemiesFile: {enemy.Name} could not generate any path, discarding!");
                    continue;
                }
                else
                {
                    List<float> lengths = new();
                    foreach (float length in node["PathLengths"].AsArray)
                    {
                        lengths.Add(length);
                    }
                    List<string> expressions = new();
                    foreach (string expression in node["PathExpressions"].AsArray)
                    {
                        expressions.Add(expression);
                    }
                    List<float> angles = new();
                    foreach (float angle in node["PathAngles"].AsArray)
                    {
                        angles.Add(angle);
                    }

                    //Waypoints.AddRange(WaypointPathCreator.GeneratePathFromExpression(enemy.SpawnPosition, lengths[0], expressions[0], angles[0]));
                    //int w = 1;
                    //for (; w < lengths.Count && w < expressions.Count && w < angles.Count; w++)
                    //{
                    //    Waypoints.AddRange(WaypointPathCreator.GeneratePathFromExpression(Waypoints.Last(), lengths[w], expressions[w], angles[w]));
                    //}

                    //if (w < lengths.Count || w < expressions.Count || w < angles.Count)
                    //{
                    //    Debug.LogWarning($"ParseEnemiesFile: {enemy.Name} only generated {w + 1} paths, rest discarded because of missing path data");
                    //}
                }

                if (node["SpawnRepeatsDelays"].HasValue && node["SpawnRepeatsAmounts"].HasValue)
                {
                    List<(float delay, int amount)> SpawnRepeats = new();

                    List<float> delays = new();
                    foreach (float delay in node["SpawnRepeatsDelays"].AsArray)
                    {
                        delays.Add(delay);
                    }
                    List<int> amounts = new();
                    foreach (int amount in node["SpawnRepeatsAmounts"].AsArray)
                    {
                        amounts.Add(amount);
                    }

                    int r = 0;
                    for (; r < delays.Count && r < amounts.Count; r++)
                    {
                        SpawnRepeats.Add((delays[r], amounts[r]));
                    }

                    if (r < delays.Count || r < amounts.Count)
                    {
                        Debug.LogWarning($"ParseEnemiesFile: {enemy.Name} has missing spawn repeat data, will still add with default values");
                    }
                    for (int n = r; n < delays.Count; n++)  //If there are more delay values, repeat them once
                    {
                        SpawnRepeats.Add((delays[n], 1));
                    }
                    for (int n = r; n < amounts.Count; n++) //If there are more amount values, use the previous delay time
                    {
                        SpawnRepeats.Add((delays[r], amounts[n]));
                    }

                    enemy.SpawnRepeats = SpawnRepeats;
                }
                else if(node["SpawnRepeatsDelays"].HasValue ^ node["SpawnRepeatsAmounts"].HasValue)
                {
                    Debug.LogWarning($"ParseEnemiesFile: {enemy.Name} could not generate spawn repeat info, either delays or amounts are empty");
                }

                enemies.Add(enemy);
            }

        }

        return enemies;
    }
}
