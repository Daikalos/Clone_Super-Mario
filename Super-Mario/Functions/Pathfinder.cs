using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Super_Mario
{
    static class Pathfinder
    {
        public static List<Tile> FindPath(Vector2 aStart, Vector2 aGoal) //BFS (Breadth First Search)
        {
            List<Tile> tempResult = new List<Tile>();
            List<Tile> tempVisited = new List<Tile>();
            Queue<Tile> tempWork = new Queue<Tile>();

            Tile tempStart = Level.GetTileAtPos(aStart).Item1;
            Tile tempGoal = Level.GetClosestTile(aGoal);

            tempStart.History = new List<Tile>();
            tempVisited.Add(tempStart);
            tempWork.Enqueue(tempStart);

            while (tempWork.Count > 0)
            {
                Tile tempCurrent = tempWork.Dequeue();
                if (tempCurrent == tempGoal)
                {
                    tempResult = tempCurrent.History;
                    tempResult.Add(tempCurrent);
                    return tempResult;
                }
                else
                {
                    for (int x = -1; x <= 1; x += 2)
                    {
                        Tuple<Tile, bool> tempCheckTile = Level.GetTileAtPos(new Vector2(tempCurrent.GetCenter().X + x * Level.TileSize.X, tempCurrent.GetCenter().Y));
                        if (tempCheckTile.Item2)
                        {
                            if (tempCheckTile.Item1.TileType != '#')
                            {
                                Tile tempCurrentNeighbor = tempCheckTile.Item1;
                                if (!tempVisited.Contains(tempCurrentNeighbor))
                                {
                                    tempCurrentNeighbor.History = new List<Tile>(tempCurrent.History);
                                    tempCurrentNeighbor.History.Add(tempCurrent);
                                    tempVisited.Add(tempCurrentNeighbor);
                                    tempWork.Enqueue(tempCurrentNeighbor);
                                }
                            }
                        }
                    }
                    for (int y = -1; y <= 1; y += 2)
                    {
                        Tuple<Tile, bool> tempCheckTile = Level.GetTileAtPos(new Vector2(tempCurrent.GetCenter().X, tempCurrent.GetCenter().Y + y * Level.TileSize.Y));
                        if (tempCheckTile.Item2)
                        {
                            if (tempCheckTile.Item1.TileType != '#')
                            {
                                Tile tempCurrentNeighbor = tempCheckTile.Item1;
                                if (!tempVisited.Contains(tempCurrentNeighbor))
                                {
                                    tempCurrentNeighbor.History = new List<Tile>(tempCurrent.History);
                                    tempCurrentNeighbor.History.Add(tempCurrent);
                                    tempVisited.Add(tempCurrentNeighbor);
                                    tempWork.Enqueue(tempCurrentNeighbor);
                                }
                            }
                        }
                    }
                }
            }
            return new List<Tile>();
        }

        private static float ManhattanDistance(Vector2 aCurrent, Vector2 aGoal)
        {
            return Math.Abs(aCurrent.X - aGoal.X) + Math.Abs(aCurrent.Y + aGoal.Y);
        }
    }
}
