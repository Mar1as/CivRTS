using System.IO;
using UnityEngine;

public class SaveLoadHexCell
{
    MainHexCell mainHexCell;
    DataHexCell data { get => mainHexCell.dataHexCell;}

    int kokot = 0;
    public SaveLoadHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }

    void RefreshPosition()
    {
        Vector3 position = mainHexCell.transform.localPosition;
        position.y = data.Elevation * HexMetrics.elevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;

        mainHexCell.transform.localPosition = position;

        Vector3 uiPosition = data.uiRect.localPosition;
        uiPosition.z = -position.y;
        data.uiRect.localPosition = uiPosition;
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

        if (data.river.hasIncomingRiver)
        {
            writer.Write((byte)(data.river.incomingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        if (data.river.hasOutgoingRiver)
        {
            writer.Write((byte)(data.river.outgoingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        int roadFlags = 0;
        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            if (data.roadScript.roads[i])
            {
                roadFlags |= 1 << i;
            }
        }
        writer.Write((byte)roadFlags);
    }

    public void Load(BinaryReader reader)
    {
        data.TerrainTypeIndex = reader.ReadByte();
        data.Elevation = reader.ReadByte();
        RefreshPosition();
        data.waterScript.WaterLevel = reader.ReadByte();
        data.featuresHexCell.UrbanLevel = reader.ReadByte();
        data.featuresHexCell.FarmLevel = reader.ReadByte();
        data.featuresHexCell.PlantLevel = reader.ReadByte();
        data.wallsScript.Walled = reader.ReadBoolean();

        byte riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            data.river.hasIncomingRiver = true;
            data.river.incomingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            data.river.hasIncomingRiver = false;
        }

        riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            data.river.hasOutgoingRiver = true;
            data.river.outgoingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            data.river.hasOutgoingRiver = false;
        }

        int roadFlags = reader.ReadByte();
        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            data.roadScript.roads[i] = (roadFlags & (1 << i)) != 0;
        }
    }
}
