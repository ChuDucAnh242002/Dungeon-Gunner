using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable(){
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable(){
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();
        
        LoadRoomNodeTypeList();

    }

    private void LoadRoomNodeTypeList(){
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel){
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while(!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts){
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            while(!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph){
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;
                
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);

            }
            if (dungeonBuildSuccessful){

                InstantiateRoomGameobjects();
            }
        }
        return dungeonBuildSuccessful;
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList){
        if(roomNodeGraphList.Count > 0){
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else {
            Debug.Log("No room Node graphs in list");
            return null;
        }
    }

    private void LoadRoomTemplatesIntoDictionary(){
        roomTemplateDictionary.Clear();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList){
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid)){
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
            }
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph){
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null){
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else {
            Debug.Log("No Entrance Node");
            return false;
        }

        bool noRoomOverlaps = true;
        noRoomOverlaps = ProcessRommsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // Debug.Log(openRoomNodeQueue.Count);
        // Debug.Log(noRoomOverlaps);
        return openRoomNodeQueue.Count == 0 && noRoomOverlaps;
    }

    private bool ProcessRommsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps){
        

        while(openRoomNodeQueue.Count > 0 && noRoomOverlaps){
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode)){

                if(childRoomNode != null){
                    openRoomNodeQueue.Enqueue(childRoomNode);
                }
            }

            if(roomNode.roomNodeType.isEntrance){
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            } else {
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);  
            }
        }
        
        return noRoomOverlaps;
    }

    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom){

        while(true){
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();
            if(unconnectedAvailableParentDoorways.Count == 0){
                return false; // room overlaps
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            RoomTemplateSO roomTemplate = GetRandomForRoomConsistentWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            if (PlaceTheRoom(parentRoom, doorwayParent, room)){
                room.isPositioned = true;
                dungeonBuilderRoomDictionary.Add(room.id, room);
                return true;
            }
        }
        
    }

    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList){
        foreach (Doorway doorway in roomDoorwayList){
            if(!doorway.isConnected && !doorway.isUnavailable){
                // Debug.Log("unconnected doorway");
                yield return doorway;
            }
        }
    }

    private RoomTemplateSO GetRandomForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent){
        RoomTemplateSO roomTemplate = null;
        if (roomNode.roomNodeType.isCorridor){
            switch(doorwayParent.orientation){
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                
                case Orientation.none:
                    break;
                default:
                    break;
            }
        } else {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room){
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        if(doorway == null){
            doorwayParent.isUnavailable = true;
            return false;
        }

        // Calculate 'world' grid parent doorway
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        switch(doorway.orientation){
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.none:
                break;
            default: 
                break;
        }

        //room lowerbounds, upper bound
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if(overlappingRoom == null){
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isConnected = true;
            return true;
        } else {

            doorwayParent.isUnavailable = true;
            return false;
        }

    }

    private Room CheckForRoomOverlap(Room roomToTest){
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary){
            Room room = keyValuePair.Value;

            if (room.id == roomToTest.id || !room.isPositioned){
                continue;
            }
            if(IsOverLappingRoom(roomToTest, room)){
                return room;
            }
        }

        return null;
    }
    private bool IsOverLappingRoom(Room room1, Room room2){
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);
        return isOverlappingX && isOverlappingY;
    }

    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2){
        return Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2);
    }

    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorWayList){
        foreach(Doorway doorway in doorWayList){
            if (doorwayParent.orientation == Orientation.east && doorway.orientation == Orientation.west){
                return doorway;
            } 
            else if (doorwayParent.orientation == Orientation.west && doorway.orientation == Orientation.east){
                return doorway;
            } 
            else if (doorwayParent.orientation == Orientation.north && doorway.orientation == Orientation.south){
                return doorway;
            } 
            else if (doorwayParent.orientation == Orientation.south && doorway.orientation == Orientation.north){
                return doorway;
            } 
        }
        return null;
    }

    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType){
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();
        
        foreach(RoomTemplateSO roomTemplate in roomTemplateList){
            if(roomTemplate.roomNodeType == roomNodeType){
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if(matchingRoomTemplateList.Count == 0){
            return null;
        }

        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode){
        Room room = new Room();

        room.id = roomNode.id;
        room.templateID = roomTemplate.guid;
        room.prefab = roomTemplate.prefab;
        room.battleMusic = roomTemplate.battleMusic;
        room.ambientMusic = roomTemplate.ambientMusic;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        room.roomEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentRoomNodeIDList.Count == 0){
            room.parentRoomID = "";
            room.isPreviouslyVisisted = true;

            GameManager.Instance.SetCurrentRoom(room);
        } else {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0){
            room.isClearedOfEnemies = true;
        }

        return room;
    }

    private List<string> CopyStringList(List<string> childRoomNodeIDList){
        List<string> copyStringList = new List<string>();
        foreach(string childRoomNodeID in childRoomNodeIDList){
            copyStringList.Add(childRoomNodeID);
        }
        return copyStringList;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> doorwayList){
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in doorwayList){
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }


    private void InstantiateRoomGameobjects(){
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary){
            Room room = keyValuePair.Value;

            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            instantiatedRoom.Initialise(roomGameobject);

            room.instantiatedRoom = instantiatedRoom;
        }
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID){
        if(roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate)){
            return roomTemplate;
        } else {
            return null;
        }
    }

    public Room GetRoomByRoomID(string roomID){
        if(dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room)){
            return room;
        } else {
            return null;
        }
    }

    private void ClearDungeon(){
        if (dungeonBuilderRoomDictionary.Count > 0){
            foreach(KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary){
                Room room = keyvaluepair.Value;
                if(room.instantiatedRoom != null){
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
        }

        dungeonBuilderRoomDictionary.Clear();
    }
}
