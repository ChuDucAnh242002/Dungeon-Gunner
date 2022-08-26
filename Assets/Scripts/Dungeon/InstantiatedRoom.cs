using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
[DisallowMultipleComponent]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake(){
        boxCollider2D = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider2D.bounds;
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom()){

            this.room.isPreviouslyVisisted = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialise(GameObject roomGameobject){
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorway();

        AddDoorToRooms();

        DisableCollisionTilemapRenderer();
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameobject){
        grid = roomGameobject.GetComponentInChildren<Grid>();
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps){
            if (tilemap.gameObject.tag == "groundTilemap"){
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap"){
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap"){
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap"){
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap"){
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap"){
                minimapTilemap = tilemap;
            }
            
        }
    }

    private void BlockOffUnusedDoorway(){
        foreach (Doorway doorway in room.doorWayList){
            if(doorway.isConnected){
                continue;
            }

            if(groundTilemap != null){
                BlockADorrwayOnTilemapLayer(groundTilemap, doorway);
            }
            if(decoration1Tilemap != null){
                BlockADorrwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if(decoration2Tilemap != null){
                BlockADorrwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if(frontTilemap != null){
                BlockADorrwayOnTilemapLayer(frontTilemap, doorway);
            }
            if(collisionTilemap != null){
                BlockADorrwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if(minimapTilemap != null){
                BlockADorrwayOnTilemapLayer(minimapTilemap, doorway);
            }

        }
    }

    private void BlockADorrwayOnTilemapLayer(Tilemap tilemap, Doorway doorway){
        switch(doorway.orientation){
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }
    }

    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway){
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for(int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++){
            for(int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++){
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), 
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway){
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for(int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++){
            for(int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++){
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), 
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    private void AddDoorToRooms(){
        if(room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return; 

        foreach (Doorway doorway in room.doorWayList){
            if(doorway.doorPrefab != null && doorway.isConnected){
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;
                door = Instantiate(doorway.doorPrefab, gameObject.transform);

                if(doorway.orientation == Orientation.north){
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south){
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east){
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west){
                    door.transform.localPosition = new Vector3(doorway.position.x , doorway.position.y + tileDistance * 1.25f, 0f);
                }

                Door doorComponent = door.GetComponent<Door>();
                if(room.roomNodeType.isBossRoom){
                    doorComponent.isBossRoomDoor = true;

                    doorComponent.LockDoor();
                }
            }
        }
    }

    private void DisableCollisionTilemapRenderer(){
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
