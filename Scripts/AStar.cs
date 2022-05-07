using Godot;
using System;

public class AStar : Node2D
{
    AStar2D astar;
    TileMap baseTileMap;
    TileMap collisionTileMap;
    TileMap roadMap;
    Rect2 usedRect;

    public void CreateNavigationMap(TileMap tilemap){
        astar = new AStar2D();
        baseTileMap = tilemap;
        usedRect = tilemap.GetUsedRect();

        Godot.Collections.Array tiles = tilemap.GetUsedCells();

        AddTraversableTiles(tiles);
        ConnectTraversableTiles(tiles);


    }


    private void AddTraversableTiles(Godot.Collections.Array tiles){
        foreach (Vector2 tile in tiles){
            int id = GetIDForPoint(tile);
            astar.AddPoint(id, tile);
        }
    }

    private void ConnectTraversableTiles(Godot.Collections.Array tiles){
        foreach (Vector2 tile in tiles){
            int id = GetIDForPoint(tile);

            for (int x = 0; x < 3; x++){
                for (int y = 0; y < 3; y++){
                    Vector2 target = tile + new Vector2(x -1, y -1);
                    int targetID = GetIDForPoint(target);

                    if (tile == target || !astar.HasPoint(targetID)){
                        continue;
                    } else {
                        astar.ConnectPoints(id, targetID, true);
                    }

                }
            }
        }
    }

    public void DisableCollisionTiles(TileMap collisionTiles){
        Godot.Collections.Array tiles = collisionTiles.GetUsedCells();
        foreach (Vector2 tile in tiles){
            if (!(collisionTiles.GetCellv(tile) == 3)){
                astar.SetPointDisabled(GetIDForPoint(tile));
            }
        }
    } 

    private int GetIDForPoint(Vector2 Point){
        var x = Point.x - usedRect.Position.x;
        var y = Point.y - usedRect.Position.y;
        return (int)(x + y * usedRect.Size.x);
    }

    public Vector2[] GetPointPath(Vector2 fromVector, Vector2 toVector){
        int fromId = GetIDForPoint(fromVector);
        int toId = GetIDForPoint(toVector);
        Vector2[] path = astar.GetPointPath(fromId, toId);
        return path;
    }
    public Vector2[] GetPointPath(int fromId, int toId){
        Vector2[] path = astar.GetPointPath(fromId, toId);
        return path;
    }

}
