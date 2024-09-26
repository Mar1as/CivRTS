using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hex/GenerationSettings")]
public class HexTileGenerationSettings : ScriptableObject
{
    public enum TileType
    {
        Plain,
        Water
    }

    public GameObject plain;
    public GameObject water;

    public GameObject GetTile(TileType tile)
    {
        switch (tile)
        {
            case TileType.Plain:
                return plain;
            case TileType.Water:
                return water;
        }
        return null;
    }
}
