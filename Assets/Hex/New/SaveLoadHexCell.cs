using System.IO;
using UnityEngine;

public class SaveLoadHexCell
{
    MainHexCell mainHexCell;
    DataHexCell data;

    public SaveLoadHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
        data = mainHexCell.dataHexCell;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)data.TerrainTypeIndex);
        writer.Write((byte)data.Elevation);
        writer.Write((byte)data.waterScript.WaterLevel);
        writer.Write((byte)data.featuresHexCell.UrbanLevel);
        writer.Write((byte)data.featuresHexCell.FarmLevel);
        writer.Write((byte)data.featuresHexCell.PlantLevel);

        writer.Write(data.wallsScript.Walled);

        writer.Write(data.river.hasIncomingRiver);
        writer.Write((byte)data.river.incomingRiver);

        writer.Write(data.river.hasOutgoingRiver);
        writer.Write((byte)data.river.outgoingRiver);

        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            writer.Write(data.roadScript.roads[i]);
        }
    }

    public void Load(BinaryReader reader)
    {
        data.TerrainTypeIndex = reader.ReadByte();
        data.Elevation = reader.ReadByte();
        data.waterScript.WaterLevel = reader.ReadByte();
        data.featuresHexCell.UrbanLevel = reader.ReadByte();
        data.featuresHexCell.FarmLevel = reader.ReadByte();
        data.featuresHexCell.PlantLevel = reader.ReadByte();

        data.wallsScript.Walled = reader.ReadBoolean();

        data.river.hasIncomingRiver = reader.ReadBoolean();
        data.river.incomingRiver = (HexDirection)reader.ReadByte();

        data.river.hasOutgoingRiver = reader.ReadBoolean();
        data.river.outgoingRiver = (HexDirection)reader.ReadByte();

        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            data.roadScript.roads[i] = reader.ReadBoolean();
        }

    }
}
