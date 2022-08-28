using UnityEngine;

public class GridNodes 
{
    private int width;
    private int height;

    private Node[,] gridNode;

    public GridNodes(int width, int height){
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                gridNode[i, j] = new Node(new Vector2Int(i, j));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition){
        if (xPosition < width && yPosition < height){
            return gridNode[xPosition, yPosition];
        } else {
            Debug.Log("Requested grid node is out of range");
            return null;
        }

    }
}
