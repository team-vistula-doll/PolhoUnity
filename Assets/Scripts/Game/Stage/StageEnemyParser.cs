using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public struct Enemy
{
    float spawnTime; //Enemies HAVE to be ordered by spawnTime from earliest to latest
    string enemyName;
    Vector2 spawnPosition;
    List<Vector2> waypoints;
    List<(float delay, int amount)> spawnRepeats; //delay between spawns, amount of spawns; OPTIONAL
}

public class StageEnemyParser
{
    public List<Enemy> Enemies { get; set; } = new List<Enemy>();

    //Single line structure of stage enemies data file:
    //spawnTime, enemyName, spawnPosition.X, spawnPosition.Y: (length1, expression1, angle1), (len2, exp2, ang2), ...|
    // spawnRepeats.delay1, spawnRepeats.amount1| spawnRepeats.delay2, spawnRepeats.amount2| ...;
    private string pattern =
    #region Line regex pattern
            @"\s*((?:(?:[+]?[0-9]*)[mM])|(?:(?:[+]?[0-9]*\.?[0-9]+)[sS])|(?:(?:[+]?[0-9]*)[mM])(?:(?:[+]?[0-9]*\.?[0-9]+)[sS]))\,\s*([\w]+)\,\s*([-+]?[0-9]*\.?[0-9]+)\,\s*([-+]?[0-9]*\.?[0-9]+)\:\s*([ \(\)\,a-zA-Z0-9-+*\/\^]+)\s*(?:\|\s*((?:\s*(?:(?:(?:[+]?[0-9]*)[mM])|(?:(?:[+]?[0-9]*\.?[0-9]+)[sS])|(?:(?:[+]?[0-9]*)[mM])(?:(?:[+]?[0-9]*\.?[0-9]+)[sS]))\,\s*(?:[+]?[0-9]+)\|?)*))?[;]";
    #endregion

    /// <summary>
    /// Parses stage enemies data from a file
    /// </summary>
    /// <param name="filepath">Path to the file, relative from "\Scripts\Game\Stage\"</param>
    public List<int> ParseEnemiesFile(string filepath)
    {
        List<int> badLines = new List<int>();

        int currentLineNumber = 1;
        foreach (string line in File.ReadLines(@filepath))
        {
            Match match;
            try { match = Regex.Match(line, pattern); }
            catch (ArgumentException)
            {
                badLines.Add(currentLineNumber);
                continue;
            }

            int minutes = -1, seconds = -1;

            string time = match.Groups[1].Value.ToLower();
            for (int mIndex = time.IndexOf('m'); mIndex != -1; mIndex = -1)
            {
                minutes = Int32.Parse(time.Substring(0, mIndex));
                time.Remove(0, mIndex + 1);
            }

            for (int sIndex = time.IndexOf('s'); sIndex != -1; sIndex = -1)
            {
                seconds = Int32.Parse(time.Substring(0, sIndex));
            }

            if (seconds == -1 && minutes == -1)
            {
                badLines.Add(currentLineNumber);
                continue;
            }

            string name = match.Groups[2].Value;

            if (name == "")
            {
                badLines.Add(currentLineNumber);
                continue;
            }
            currentLineNumber++;
            }
        return badLines;
    }
}
