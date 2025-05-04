using System;
using System.Collections.Generic;
using System.Linq;

namespace test_dot_hotel;

class RobotsLabyrinth
{
    // Константы для символов ключей и дверей
    static readonly char[] keys_char = Enumerable.Range('a', 26).Select(i => (char)i).ToArray();
    static readonly char[] doors_char = keys_char.Select(char.ToUpper).ToArray();
    
    // Метод для чтения входных данных
    private static List<List<char>> GetInput()
    {
        var data = new List<List<char>>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            data.Add(line.ToCharArray().ToList());
        }
        return data;
    }

    private static int Solve(List<List<char>> data)
    {
        var rows = data.Count;
        var cols = data[0].Count;
        
        List<(int r, int c)> robots = [];
        var allKeys = new HashSet<char>();
        
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var cell = data[r][c];
                if (cell == '@')
                {
                    robots.Add((r, c));
                    data[r][c] = '.';
                }
                else if (keys_char.Contains(cell))
                {
                    allKeys.Add(cell);
                }
            }
        }
        
        var queue = new Queue<(List<(int r, int c)> pos, HashSet<char> keys, int steps)>();
        
        var visited = new HashSet<string>();
        
        var initialKeys = new HashSet<char>();
        queue.Enqueue((robots, initialKeys, 0));
        visited.Add(GetStateKey(robots, initialKeys));
        
        int[] dr = [-1, 0, 1, 0];
        int[] dc = [0, 1, 0, -1];
        
        while (queue.Count > 0)
        {
            var (positions, collectedKeys, steps) = queue.Dequeue();
            
            if (collectedKeys.Count == allKeys.Count)
                return steps;
            
            for (var i = 0; i < positions.Count; i++)
            {
                var (r, c) = positions[i];
                
                for (var d = 0; d < 4; d++)
                {
                    var newR = r + dr[d];
                    var newC = c + dc[d];
                    
                    if (newR < 0 || newR >= rows || newC < 0 || newC >= cols)
                        continue;
                    
                    var cell = data[newR][newC];
                    
                    if (cell == '#')
                        continue;
                    
                    if (doors_char.Contains(cell))
                    {
                        var requiredKey = char.ToLower(cell);
                        if (!collectedKeys.Contains(requiredKey))
                            continue;
                    }
                    
                    var newCollectedKeys = new HashSet<char>(collectedKeys);
                    
                    if (keys_char.Contains(cell))
                    {
                        newCollectedKeys.Add(cell);
                    }
                    
                    var newPositions = new List<(int r, int c)>(positions)
                    {
                        [i] = (newR, newC)
                    };
                    
                    var stateKey = GetStateKey(newPositions, newCollectedKeys);
                    
                    if (visited.Contains(stateKey))
                        continue;
                    
                    queue.Enqueue((newPositions, newCollectedKeys, steps + 1));
                    visited.Add(stateKey);
                }
            }
        }

        return -1;
    }
    
    private static string GetStateKey(List<(int r, int c)> positions, HashSet<char> keys)
    {
        var sortedPositions = positions
            .OrderBy(p => p.r * 1000 + p.c)
            .Select(p => $"{p.r},{p.c}")
            .ToList();
        
        var sortedKeys = keys.OrderBy(k => k).ToList();
        
        return string.Join("|", sortedPositions) + "#" + string.Join("", sortedKeys); // в таком роде r1,c1|r2,c2#ab
    }

    public static void Main()
    {
        var data = GetInput();
        var result = Solve(data);
        
        if (result == -1)
        {
            Console.WriteLine("No solution found");
        }
        else
        {
            Console.WriteLine(result);
        }
    }
}