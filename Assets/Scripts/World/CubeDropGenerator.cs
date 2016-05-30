using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CubeDropGenerator : MonoBehaviour 
{
    float TextureOffset = 1F / 16F;
    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangulos = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    Block bloc = null;

    public void StartCube(Block b)
    {
        bloc = b;
        StartCoroutine(GenerateCube(b));
    }

    IEnumerator GenerateCube(Block b)
    {
        AddCubeFront(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeBack(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeTop(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeBottom(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeRight(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeLeft(0, 0, 0, b);

        mesh = new Mesh();
        yield return new WaitForEndOfFrame();
        mesh.vertices = vertices.ToArray();
        yield return new WaitForEndOfFrame();
        mesh.triangles = triangulos.ToArray();
        yield return new WaitForEndOfFrame();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        yield return new WaitForEndOfFrame();
        this.GetComponent<MeshFilter>().mesh = mesh;

        this.gameObject.AddComponent<BoxCollider>();
    }
    GameObject pl;
    void Update()
    {
        if (pl == null)
            pl = GameObject.FindGameObjectWithTag("Player");

        if(Vector3.Distance(this.transform.position, pl.transform.position - new Vector3(0,1,0)) < 1f)
        {
            PlayerInventory.AddItem(bloc, 1);
            Destroy(this.gameObject);
        }
    }

    public void AddCubeFront(int x, int y, int z, Block b)
    {
        //x = x /*+ Mathf.FloorToInt(transform.position.x)*/;
        //y = y /*+ Mathf.FloorToInt(transform.position.y)*/;
        //z = z /*+ Mathf.FloorToInt(transform.position.z*/);

        z++;

        int offset = 1;
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeBack(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeTop(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureX, TextureOffset * b.TextureY));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, TextureOffset * b.TextureY));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, (TextureOffset * b.TextureY) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureX, (TextureOffset * b.TextureY) + TextureOffset));
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }
    public void AddCubeBottom(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        y--;

        int offset = 1;
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, TextureOffset * b.TextureYBottom));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, TextureOffset * b.TextureYBottom));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, (TextureOffset * b.TextureYBottom) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, (TextureOffset * b.TextureYBottom) + TextureOffset));
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }
    public void AddCubeRight(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeLeft(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        // y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        x--;

        int offset = 1;
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
}
