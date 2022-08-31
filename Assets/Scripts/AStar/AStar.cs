using System.Collections.Generic;
using UnityEngine;

public static class AStar 
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition){
        startGridPosition -= (Vector3Int) room.templateLowerBounds;
        endGridPosition -= (Vector3Int) room.templateLowerBounds;

        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHastSet = new HashSet<Node>();

        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHastSet, room.instantiatedRoom);

        if(endPathNode != null){
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    public static Node FindShortestPath(Node startNode, Node targetNode,
        GridNodes gridNodes, List<Node> openNodeList, 
        HashSet<Node> closedNodeHastSet, InstantiatedRoom instantiatedRoom){

        openNodeList.Add(startNode);
        while(openNodeList.Count > 0){
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            if(currentNode == targetNode){
                return currentNode;
            }

            closedNodeHastSet.Add(currentNode);

            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHastSet, instantiatedRoom);

        }
        return null;

    }

    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, 
        GridNodes gridNodes, List<Node> openNodeList, 
        HashSet<Node> closedNodeHastSet, InstantiatedRoom instantiatedRoom){
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for(int i = -1; i <= 1; i++){
            for (int j = -1; j <= 1; j++){
                if (i == 0 && j == 0){
                    continue;
                }

                // Max 8 NeighbourNode
                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j,
                    gridNodes, closedNodeHastSet, instantiatedRoom);

                if(validNeighbourNode != null){
                    int newCostToNeighbour;

                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x,
                        validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList){
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if(!isValidNeighbourNodeInOpenList){
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    public static int GetDistance(Node nodeA, Node nodeB){
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
        int dst = 14 * dstX + 10 * (dstY - dstX);

        if (dstX > dstY){
            dst = 14 * dstY + 10 * (dstX - dstY);
        }
        return dst;
    }

    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, 
        GridNodes gridNodes, HashSet<Node> closedNodeHastSet, InstantiatedRoom instantiatedRoom){

        // Beyond the grid
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0 ||
            neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0){
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        int itemObstaclesForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];

        if (closedNodeHastSet.Contains(neighbourNode) || movementPenaltyForGridSpace == 0 || itemObstaclesForGridSpace == 0){
            return null;
        } else {
            return neighbourNode;
        }
    }

    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room){
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null){
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));
            
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }
}
