/*
Select, build, action
*/
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public Collider island;
    public Transform selector, cursorIndicator;

    public GameObject selectedGO;

    public Buildable building;
    private GameObject buildingGO;
    Island i;
    RaycastHit hit;

    private bool displayUI = false;


   struct ClickData
    {
        public static GameObject clickedObject;
        public static Tile clickedTile;
        public static Vector3 hitPoint;
        public static int dirr;
    }

    private void Start() {
        i = island.GetComponent<Island>();
    }

    private void Update() {
        SetClickData();
        
        cursorIndicator.transform.position = ClickData.hitPoint + (Vector3.up * 0.001f);
        if(buildingGO != null){
            buildingGO.transform.position = ClickData.hitPoint;
            if(building.isRotatable){
                if(Input.GetKeyDown(KeyCode.A)){
                        buildingGO.transform.localEulerAngles += Vector3.up * 90;
                }else if(Input.GetKeyDown(KeyCode.D)){
                        buildingGO.transform.localEulerAngles -= Vector3.up * 90;
                }
            }
        }
        
        if( Input.GetMouseButtonDown(0)){
            SetClickData(true);
            if(buildingGO != null){
                // Build
                Build(ClickData.clickedTile);
            }else{

                string clickTag = "";
                if(ClickData.clickedObject != null )
                    clickTag = ClickData.clickedObject.tag;

                // Select
                if(clickTag != "agent" && selectedGO != null && selectedGO.tag == "agent"){
                    AgentController ac = selectedGO.GetComponent<AgentController>();
                    if(clickTag == "island"){
                        ac.GeneratePathToTarget(ClickData.clickedTile, true);
                    }else{
                        ac.TargetToAction(ClickData.clickedObject.transform);
                    }
                }else if(clickTag == "agent" && selectedGO != null && selectedGO.tag == "charger"){
                    Charger charger = selectedGO.GetComponent<Charger>();
                    charger.SetTarget(ClickData.clickedObject.GetComponent<AgentController>());
                }else{
                    selectedGO = ClickData.clickedObject;
                    selector.gameObject.SetActive(true);
                }
            }

        }else if(Input.GetMouseButtonDown(1)){
            DisableSelector();
            if(buildingGO != null){
                Destroy(buildingGO);
                ClearBuildingData();                
            }
        }

        if(selectedGO != null){
            float yPos = Mathf.Floor(selectedGO.transform.position.y);
            yPos += 0.001f;
            selector.position = new Vector3(
                selectedGO.transform.position.x,
                yPos,
                selectedGO.transform.position.z
            );
        }
    }

    public void SetClickData(bool ubdateClick = false){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(ubdateClick){
            ClickData.clickedObject = null;
        }

        if (island.Raycast(ray, out hit, 100)) {
            if(hit.normal.y == 1){
                int x = Mathf.RoundToInt(hit.point.x);
                int z = Mathf.RoundToInt(hit.point.z);
                ClickData.hitPoint = new Vector3(x, hit.point.y, z);

                if(ubdateClick){
                    Tile tile = i.GetTileByWorldCoords(x, z);
                    ClickData.clickedTile = tile;
                    ClickData.clickedObject = tile.building;
                }
            }
        }
        if(ClickData.clickedObject == null && ubdateClick){
            if (Physics.Raycast(ray, out hit )) {
                ClickData.clickedObject = hit.collider.gameObject;
            }
        }
    }

    public void Build(Tile tile){
        if(PlayerData.HasEnoughResources(building.requiredResources)){
            //GameObject b = (GameObject)Instantiate(buildingGO);
            if (!i.PlaceObject(tile, buildingGO, true)){
                Destroy(buildingGO);
            } else {
                PlayerData.RemoveResources(building.requiredResources);
                SetBuildingComponents(buildingGO, true);
                ClearBuildingData();
            }
        }
    }

    public void ClearBuildingData(){
        buildingGO = null;
        building = null;
    }

    public void SetBuilding(Buildable building){
        if(buildingGO != null){
            Destroy(buildingGO);
        }
        this.building = building;
        buildingGO = (GameObject)Resources.Load("Prefabs/Builds/" + building.prefabPath);
        GameObject b = (GameObject)Instantiate(buildingGO);
        SetBuildingComponents(b, false);
        buildingGO = b;

        DisableSelector();
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
        if(displayUI){

        }
    }

    private void DisableSelector(){
        selectedGO = null;
        selector.gameObject.SetActive(false);
    }

    private void SetBuildingComponents(GameObject build, bool state){
        BoxCollider boxCollider = build.GetComponent<BoxCollider>();
        if(boxCollider) boxCollider.enabled = state;

        ResourceCollector resourceCollector = build.GetComponent<ResourceCollector>();
        if(resourceCollector) resourceCollector.enabled = state;

        ResourceConverter resourceConverter = build.GetComponent<ResourceConverter>();
        if(resourceConverter) resourceConverter.enabled = state;

        AgentSpawner agentSpawner = build.GetComponent<AgentSpawner>();
        if(agentSpawner) agentSpawner.enabled = state;

        Animator animator = build.GetComponent<Animator>();
        if(animator) animator.enabled = state;
        
        TreeSibling treeSibling = build.GetComponent<TreeSibling>();
        if(treeSibling) treeSibling.enabled = state;
    }

}
