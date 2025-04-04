using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSelectUnits : MonoBehaviour
{
    [SerializeField] RectTransform selectionBox;
    [SerializeField] GameObject prefab;
    Orders orders;
    [SerializeField] LayerMask friendlyLayerMask;
    [SerializeField] public float distance = 1;
    Select select;
    List<GameObject> listAllPlayerUnits;
    List<GameObject> listSelectedUnits;
    Camera mainCamera;
    Vector2 startPosition;
    Vector3 beginVector;
    Vector3 finalVector;
    TeamsConstructor player;

    [SerializeField] private Vector3[] zoneForSelection = new Vector3[4]; // Z�na pro v�b�r jednotek

    private void Start()
    {
        player = Teams.listOfPlayers[0];
        listAllPlayerUnits = player.listUnits;
        listSelectedUnits = player.listSelectedUnits;
        orders = new Orders(prefab, listSelectedUnits, this);
        mainCamera = Camera.main;
        select = new Select(listSelectedUnits);

        UpdateSelectionBox();
        selectionBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (Input.GetMouseButtonDown(0))
            ClickOnUnit();
        if (Input.GetMouseButton(0))
            UpdateSelectionBox();

        if (Input.GetMouseButtonUp(0))
            ReleaseSelectionBox();

        if (Input.GetMouseButtonDown(1))
            GetBeginVector();

        if (Input.GetMouseButton(1))
            UpdateFinalDest();

        if (Input.GetMouseButtonUp(1))
            RightClick();
    }

    public void ClickOnUnit()
    {
        // Kontrola, zda je kurzor my�i v z�n� pro v�b�r
        if (!IsMousePositionInZone())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        try
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, friendlyLayerMask))
                select.ClickOnUnit(hit.collider.transform.root.gameObject);

            else if (!EventSystem.current.IsPointerOverGameObject())
            {
                foreach (GameObject item in listAllPlayerUnits)
                    select.DeselectUnit(item);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void UpdateSelectionBox()
    {
        // Kontrola, zda je kurzor my�i v z�n� pro v�b�r
        if (!IsMousePositionInZone())
            return;

        if (!selectionBox.gameObject.activeInHierarchy)
        {
            startPosition = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }

        float width = Input.mousePosition.x - startPosition.x;
        float height = Input.mousePosition.y - startPosition.y;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPosition + new Vector2(width / 2, height / 2);
    }

    void ReleaseSelectionBox()
    {
        // Kontrola, zda je kurzor my�i v z�n� pro v�b�r
        if (!IsMousePositionInZone())
        {
            selectionBox.gameObject.SetActive(false);
            return;
        }

        listSelectedUnits.Clear();
        selectionBox.gameObject.SetActive(false);
        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        Debug.Log("K");
        Debug.Log(listAllPlayerUnits.Count);

        foreach (GameObject unit in listAllPlayerUnits)
        {
            Debug.Log(unit.name);
            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                select.SelectUnit(unit);
        }

        Debug.Log("Selected " + listSelectedUnits.Count);
        if (listSelectedUnits.Count > 0)
            new Battalions(player.listBattalions, new List<GameObject>(listSelectedUnits), player);
    }

    void GetBeginVector()
    {
        // Kontrola, zda je kurzor my�i v z�n� pro v�b�r
        if (!IsMousePositionInZone())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            beginVector = hit.point;
    }

    void UpdateFinalDest()
    {
        // Kontrola, zda je kurzor my�i v z�n� pro v�b�r
        if (!IsMousePositionInZone())
            return;

        if (listSelectedUnits.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                finalVector = hit.point;

            orders.GenerateObdelnikFormation(listSelectedUnits.Count, beginVector, finalVector); // Pouze pro UI
            orders.og.UpdateListPrefabs(beginVector, finalVector);
        }
    }

    void RightClick()
    {
        // Kontrola, zda je kurzor my�i v z�n� pro v�b�r
        if (!IsMousePositionInZone())
            return;

        orders.og.DestroyListPrefabs();
        if (listSelectedUnits.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log("Move");
                UnitStats unitStats = listSelectedUnits[0].GetComponent<UnitStats>();
                if (unitStats.enemyLayerMask == (unitStats.enemyLayerMask | (1 << hit.collider.gameObject.layer))) // Attack
                    orders.AttackSelectedUnit(listSelectedUnits, hit);
                else if (unitStats.terrain == (unitStats.terrain | (1 << hit.collider.gameObject.layer))) // Move
                    orders.MoveSelector(beginVector, finalVector, listSelectedUnits, distance);
            }
        }
    }

    bool IsMousePositionInZone()
    {
        // Kontrola, zda je pozice kurzoru my�i v z�n� pro v�b�r
        if (zoneForSelection.Length < 4)
        {
            Debug.LogError("ZoneForSelection must have at least 4 points to define a zone.");
            return true;
        }

        // Z�na je definov�na jako obd�ln�k v XZ rovin�
        float minX = Mathf.Min(zoneForSelection[0].x, zoneForSelection[1].x, zoneForSelection[2].x, zoneForSelection[3].x);
        float maxX = Mathf.Max(zoneForSelection[0].x, zoneForSelection[1].x, zoneForSelection[2].x, zoneForSelection[3].x);
        float minZ = Mathf.Min(zoneForSelection[0].z, zoneForSelection[1].z, zoneForSelection[2].z, zoneForSelection[3].z);
        float maxZ = Mathf.Max(zoneForSelection[0].z, zoneForSelection[1].z, zoneForSelection[2].z, zoneForSelection[3].z);

        // P�evod pozice kurzoru my�i na sv�tov� sou�adnice
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 worldPos = hit.point;
            return worldPos.x >= minX && worldPos.x <= maxX && worldPos.z >= minZ && worldPos.z <= maxZ;
        }

        return false;
    }
}