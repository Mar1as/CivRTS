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
        Debug.Log("Save: Za��tek ukl�d�n� dat hexagonu.");

        // Ulo�en� typu ter�nu jako byte
        writer.Write((byte)data.TerrainTypeIndex);
        Debug.Log($"1. Ulo�en TerrainTypeIndex: {data.TerrainTypeIndex}");

        // Ulo�en� v��ky ter�nu jako byte
        writer.Write((byte)data.Elevation);
        Debug.Log($"2. Ulo�ena Elevation: {data.Elevation}");

        // Ulo�en� �rovn� vody jako byte
        writer.Write((byte)data.waterScript.WaterLevel);
        Debug.Log($"3. Ulo�ena WaterLevel: {data.waterScript.WaterLevel}");

        // Ulo�en� �rovn� urbanizace jako byte
        writer.Write((byte)data.featuresHexCell.UrbanLevel);
        Debug.Log($"4. Ulo�ena UrbanLevel: {data.featuresHexCell.UrbanLevel}");

        // Ulo�en� �rovn� farmy jako byte
        writer.Write((byte)data.featuresHexCell.FarmLevel);
        Debug.Log($"5. Ulo�ena FarmLevel: {data.featuresHexCell.FarmLevel}");

        // Ulo�en� �rovn� rostlin jako byte
        writer.Write((byte)data.featuresHexCell.PlantLevel);
        Debug.Log($"6. Ulo�ena PlantLevel: {data.featuresHexCell.PlantLevel}");

        // Ulo�en� speci�ln�ho indexu jako byte
        writer.Write((byte)data.featuresHexCell.SpecialIndex);
        Debug.Log($"7. Ulo�en SpecialIndex: {data.featuresHexCell.SpecialIndex}");

        // Ulo�en� informace o tom, zda je hexagon opevn�n� (Walled) jako boolean
        writer.Write(data.wallsScript.Walled);
        Debug.Log($"8. Ulo�eno Walled: {data.wallsScript.Walled}");

        // Ulo�en� informace o p��choz� �ece
        writer.Write(data.river.hasIncomingRiver);
        Debug.Log($"9. Ulo�eno hasIncomingRiver: {data.river.hasIncomingRiver}");
        writer.Write((int)data.river.incomingRiver);
        Debug.Log($"10. Ulo�en incomingRiver: {data.river.incomingRiver}");

        // Ulo�en� informace o odchoz� �ece
        writer.Write(data.river.hasOutgoingRiver);
        Debug.Log($"11. Ulo�eno hasOutgoingRiver: {data.river.hasOutgoingRiver}");
        writer.Write((int)data.river.outgoingRiver);
        Debug.Log($"12. Ulo�en outgoingRiver: {data.river.outgoingRiver}");

        // Ulo�en� informac� o silnic�ch
        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            writer.Write(data.roadScript.roads[i]);
            Debug.Log($"13.{i}. Ulo�ena silnice [{i}]: {data.roadScript.roads[i]}");
        }

        Debug.Log("Save: Ukl�d�n� dat hexagonu dokon�eno.");
    }

    public void Load(BinaryReader reader)
    {
        Debug.Log("Load: Za��tek na��t�n� dat hexagonu.");

        // Na�ten� typu ter�nu
        data.TerrainTypeIndex = reader.ReadByte();
        Debug.Log($"1. Na�ten TerrainTypeIndex: {data.TerrainTypeIndex}");

        // Na�ten� v��ky ter�nu
        data.Elevation = reader.ReadByte();
        Debug.Log($"2. Na�tena Elevation: {data.Elevation}");
        RefreshPosition();

        // Na�ten� �rovn� vody
        data.waterScript.WaterLevel = reader.ReadByte();
        Debug.Log($"3. Na�tena WaterLevel: {data.waterScript.WaterLevel}");

        // Na�ten� �rovn� urbanizace
        data.featuresHexCell.UrbanLevel = reader.ReadByte();
        Debug.Log($"4. Na�tena UrbanLevel: {data.featuresHexCell.UrbanLevel}");

        // Na�ten� �rovn� farmy
        data.featuresHexCell.FarmLevel = reader.ReadByte();
        Debug.Log($"5. Na�tena FarmLevel: {data.featuresHexCell.FarmLevel}");

        // Na�ten� �rovn� rostlin
        data.featuresHexCell.PlantLevel = reader.ReadByte();
        Debug.Log($"6. Na�tena PlantLevel: {data.featuresHexCell.PlantLevel}");

        // Na�ten� speci�ln�ho indexu
        data.featuresHexCell.SpecialIndex = reader.ReadByte();
        Debug.Log($"7. Na�ten SpecialIndex: {data.featuresHexCell.SpecialIndex}");

        // Na�ten� informace o tom, zda je hexagon opevn�n� (Walled)
        data.wallsScript.Walled = reader.ReadBoolean();
        Debug.Log($"8. Na�teno Walled: {data.wallsScript.Walled}");

        // Na�ten� informace o p��choz� �ece
        data.river.hasIncomingRiver = reader.ReadBoolean();
        Debug.Log($"9. Na�teno hasIncomingRiver: {data.river.hasIncomingRiver}");
        data.river.incomingRiver = (HexDirection)reader.ReadInt32();
        Debug.Log($"10. Na�ten incomingRiver: {data.river.incomingRiver}");

        // Na�ten� informace o odchoz� �ece
        data.river.hasOutgoingRiver = reader.ReadBoolean();
        Debug.Log($"11. Na�teno hasOutgoingRiver: {data.river.hasOutgoingRiver}");
        data.river.outgoingRiver = (HexDirection)reader.ReadInt32();
        Debug.Log($"12. Na�ten outgoingRiver: {data.river.outgoingRiver}");

        // Na�ten� informac� o silnic�ch
        for (int i = 0; i < data.roadScript.roads.Length; i++)
        {
            data.roadScript.roads[i] = reader.ReadBoolean();
            Debug.Log($"13.{i}. Na�tena silnice [{i}]: {data.roadScript.roads[i]}");
        }

        Debug.Log("Load: Na��t�n� dat hexagonu dokon�eno.");
    }
}
