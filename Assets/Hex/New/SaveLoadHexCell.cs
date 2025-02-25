using System.IO;
using UnityEngine;

public class SaveLoadHexCell
{
    MainHexCell mainHexCell;
    DataHexCell data { get => mainHexCell.dataHexCell;}

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
        Debug.Log("Save: Zaèátek ukládání dat hexagonu.");

        // Uložení typu terénu jako byte
        writer.Write((byte)data.TerrainTypeIndex);
        Debug.Log($"1. Uložen TerrainTypeIndex: {data.TerrainTypeIndex}");

        // Uložení výšky terénu jako byte
        writer.Write((byte)data.Elevation);
        Debug.Log($"2. Uložena Elevation: {data.Elevation}");

        // Uložení úrovnì vody jako byte
        writer.Write((byte)data.waterScript.WaterLevel);
        Debug.Log($"3. Uložena WaterLevel: {data.waterScript.WaterLevel}");

        // Uložení úrovnì urbanizace jako byte
        writer.Write((byte)data.featuresHexCell.UrbanLevel);
        Debug.Log($"4. Uložena UrbanLevel: {data.featuresHexCell.UrbanLevel}");

        // Uložení úrovnì farmy jako byte
        writer.Write((byte)data.featuresHexCell.FarmLevel);
        Debug.Log($"5. Uložena FarmLevel: {data.featuresHexCell.FarmLevel}");

        // Uložení úrovnì rostlin jako byte
        writer.Write((byte)data.featuresHexCell.PlantLevel);
        Debug.Log($"6. Uložena PlantLevel: {data.featuresHexCell.PlantLevel}");

        // Uložení speciálního indexu jako byte
        writer.Write((byte)data.featuresHexCell.SpecialIndex);
        Debug.Log($"7. Uložen SpecialIndex: {data.featuresHexCell.SpecialIndex}");

        // Uložení informace o tom, zda je hexagon opevnìný (Walled) jako boolean
        writer.Write(data.wallsScript.Walled);
        Debug.Log($"8. Uloženo Walled: {data.wallsScript.Walled}");

        // Uložení informace o pøíchozí øece
        writer.Write(data.river.hasIncomingRiver);
        Debug.Log($"9. Uloženo hasIncomingRiver: {data.river.hasIncomingRiver}");
        writer.Write((int)data.river.incomingRiver);
        Debug.Log($"10. Uložen incomingRiver: {data.river.incomingRiver}");

        // Uložení informace o odchozí øece
        writer.Write(data.river.hasOutgoingRiver);
        Debug.Log($"11. Uloženo hasOutgoingRiver: {data.river.hasOutgoingRiver}");
        writer.Write((int)data.river.outgoingRiver);
        Debug.Log($"12. Uložen outgoingRiver: {data.river.outgoingRiver}");

        // Uložení informací o silnicích
        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            writer.Write(data.roadScript.roads[i]);
            Debug.Log($"13.{i}. Uložena silnice [{i}]: {data.roadScript.roads[i]}");
        }

        Debug.Log("Save: Ukládání dat hexagonu dokonèeno.");
    }

    public void Load(BinaryReader reader)
    {
        Debug.Log("Load: Zaèátek naèítání dat hexagonu.");

        // Naètení typu terénu
        data.TerrainTypeIndex = reader.ReadByte();
        Debug.Log($"1. Naèten TerrainTypeIndex: {data.TerrainTypeIndex}");

        // Naètení výšky terénu
        data.Elevation = reader.ReadByte();
        Debug.Log($"2. Naètena Elevation: {data.Elevation}");
        RefreshPosition();

        // Naètení úrovnì vody
        data.waterScript.WaterLevel = reader.ReadByte();
        Debug.Log($"3. Naètena WaterLevel: {data.waterScript.WaterLevel}");

        // Naètení úrovnì urbanizace
        data.featuresHexCell.UrbanLevel = reader.ReadByte();
        Debug.Log($"4. Naètena UrbanLevel: {data.featuresHexCell.UrbanLevel}");

        // Naètení úrovnì farmy
        data.featuresHexCell.FarmLevel = reader.ReadByte();
        Debug.Log($"5. Naètena FarmLevel: {data.featuresHexCell.FarmLevel}");

        // Naètení úrovnì rostlin
        data.featuresHexCell.PlantLevel = reader.ReadByte();
        Debug.Log($"6. Naètena PlantLevel: {data.featuresHexCell.PlantLevel}");

        // Naètení speciálního indexu
        data.featuresHexCell.SpecialIndex = reader.ReadByte();
        Debug.Log($"7. Naèten SpecialIndex: {data.featuresHexCell.SpecialIndex}");

        // Naètení informace o tom, zda je hexagon opevnìný (Walled)
        data.wallsScript.Walled = reader.ReadBoolean();
        Debug.Log($"8. Naèteno Walled: {data.wallsScript.Walled}");

        // Naètení informace o pøíchozí øece
        data.river.hasIncomingRiver = reader.ReadBoolean();
        Debug.Log($"9. Naèteno hasIncomingRiver: {data.river.hasIncomingRiver}");
        data.river.incomingRiver = (HexDirection)reader.ReadInt32();
        Debug.Log($"10. Naèten incomingRiver: {data.river.incomingRiver}");

        // Naètení informace o odchozí øece
        data.river.hasOutgoingRiver = reader.ReadBoolean();
        Debug.Log($"11. Naèteno hasOutgoingRiver: {data.river.hasOutgoingRiver}");
        data.river.outgoingRiver = (HexDirection)reader.ReadInt32();
        Debug.Log($"12. Naèten outgoingRiver: {data.river.outgoingRiver}");

        // Naètení informací o silnicích
        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            data.roadScript.roads[i] = reader.ReadBoolean();
            Debug.Log($"13.{i}. Naètena silnice [{i}]: {data.roadScript.roads[i]}");
        }

        Debug.Log("Load: Naèítání dat hexagonu dokonèeno.");
    }
}
