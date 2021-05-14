using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Common;

namespace Assets.Scripts.ScreepsArena3D.Views
{
    public class TerrainView : MonoBehaviour /*,IRoomViewComponent*/
    {
        private const char TERRAIN_WALL = '1';
        private const char TERRAIN_SWAMP = '2';
        [SerializeField] private MeshFilter _swampMesh = default;
        [SerializeField] private MeshFilter _wallMesh = default;
        [SerializeField] private MeshFilter _terrainMesh = default;

        private bool _hasTerrainData;
        private string _terrain;
        //private List<Vector2Int> _lairPositions;
        //private List<Vector2Int> _roadPositions;
        //private List<Vector2Int> _sourcePositions;
        //private List<Vector2Int> _controllerPositions;
        //private List<Vector2Int> _mineralPositions;
        //private List<Vector2Int> _powerBankPositions;

        private int _x;
        private int _y;
        private bool[,] _wallPositions;
        private bool[,] _swampPositions;
        private bool[,] _plainPositions;
        private bool isInitialized;
        private bool isInitializing;

        private int size;
        public void Init(string terrain, int size)
        {
            _terrain = terrain;
            _hasTerrainData = true;
            this.size = size;
            // dirty fix to arrange terrain correctly
            //this.transform.position = new Vector3(size - 1f, 0f, 0f);
            //this.transform.Rotate(Vector3.up, -90f);
        }

        private void Update()
        {
            if (isInitialized)
                return;
            if (!_hasTerrainData)
                return;
            if (!isInitializing)
            {
                isInitializing = true;
                _wallPositions = new bool[size, size];
                _swampPositions = new bool[size, size];
                _plainPositions = new bool[size, size];
                _x = 0;
                _y = 0;
            }

            //Debug.Log($"terrain: {_terrain.Length}");
            //Debug.Log($"terrain: {_terrain}");
            //var time = Time.time;
            for (; _x < size; _x++)
            {
                for (; _y < size; _y++)
                {
                    _swampPositions[_x, _y] = false;
                    _wallPositions[_x, _y] = false;

                    var unit = _terrain[_x + _y * size];
                    if (unit == '0' /*|| unit == TERRAIN_WALL*/)
                    {
                        _plainPositions[_x, _y] = true;
                    }
                    if (unit == TERRAIN_SWAMP || unit == '3')
                    {
                        //Debug.Log($"SWAMP: {_x}, {_y}");
                        _swampPositions[_x, _y] = true;
                    }
                    if (unit == TERRAIN_WALL || unit == '3')
                    {
                        _wallPositions[_x, _y] = true;
                    }
                    //if (Time.time - time > .001f)
                    //{
                    //    return;
                    //}
                }
                _y = 0;
            }

            isInitializing = false;
            isInitialized = true;

            Deform();
        }

        private float getRandom(int x, int z)
        {

            var seed = ((int)gameObject.transform.position.x + x) * 1000 + ((int)gameObject.transform.position.z + z);
            UnityEngine.Random.InitState(seed);
            return UnityEngine.Random.value;
        }
        private float getY(int x, int z, int depth)
        {
            const int maxDepth = 3;
            const float wallDepth = 2.0f;
            const float wallRandom = 1.0f;
            const float wallStep = 0.25f;
            const float wallConstant = 0.0f;

            float Y = wallConstant
                + (float)Math.Round(getRandom(x, z) * wallRandom / wallStep) * wallStep
                + Math.Min(maxDepth, depth) * wallDepth;
            return Y;
        }

        private void generateWalls2()
        {
            // calculate wall depth
            var wallDepth = new int[size, size];
            const int someHighNumber = 50;
            for (int y = 0; y < size; ++y)
                for (int x = 0; x < size; ++x)
                    wallDepth[x, y] = someHighNumber;

            for (int y = 1; y < size - 1; ++y)
                for (int x = 1; x < size - 1; ++x)
                {
                    var z = size - 1 - y;
                    if (!_wallPositions[x, y])
                        wallDepth[x, z] = 0;
                    else
                    {
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x, z + 1] + 1);
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x - 1, z] + 1);
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x - 1, z + 1] + 1);
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x + 1, z + 1] + 1);
                    }
                }
            for (int y = size - 2; y > 0; --y)
                for (int x = size - 2; x > 0; --x)
                {
                    var z = size - 1 - y;
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x, z - 1] + 1);
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x + 1, z] + 1);
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x + 1, z - 1] + 1);
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x - 1, z - 1] + 1);
                }

            for (int y = 1; y < size - 1; ++y)
                for (int x = 1; x < size - 1; ++x)
                {
                    if (wallDepth[x, y] != wallDepth[x + 1, y]
                        && wallDepth[x, y] != wallDepth[x - 1, y]
                        && wallDepth[x, y] != wallDepth[x, y + 1]
                        && wallDepth[x, y] != wallDepth[x, y - 1])
                        --wallDepth[x, y];
                }

            //// calculate wall heights
            var wallHeight = new float[size, size];
            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                {
                    if (_wallPositions[x, y])
                    {
                        var z = size - 1 - y;
                        wallHeight[x, y] = getY(((int)gameObject.transform.position.x) + x, ((int)gameObject.transform.position.z) + z, wallDepth[x, z]);
                        //if ((y == 99f || x == 0) || (x == 99f || y == 0))
                        //{
                        //    Debug.Log($"{x},{y} height = {wallHeight[x, y]}");
                        //}
                    }
                    else
                    {
                        wallHeight[x, y] = 0.0f;
                    }
                }
            //foreach (var pos in _sourcePositions)
            //{
            //    wallHeight[pos.x, pos.y] = 3.75f;
            //}
            //foreach (var pos in _lairPositions)
            //{
            //    for (int x = pos.x - 1; x <= pos.x + 1; ++x)
            //        for (int y = pos.y - 1; y <= pos.y + 1; ++y)
            //            if (x >= 0 && x <= size - 1 && y >= 0 && y <= size - 1)
            //                wallHeight[x, y] = Math.Min(wallHeight[x, y], 0.35f + getRandom(x, y) * 0.05f);
            //}
            //foreach (var pos in _mineralPositions)
            //{
            //    for (int x = pos.x - 1; x <= pos.x + 1; ++x)
            //        for (int y = pos.y - 1; y <= pos.y + 1; ++y)
            //            if (x >= 0 && x <= size - 1 && y >= 0 && y <= size - 1)
            //                wallHeight[x, y] = Math.Min(wallHeight[x, y], 0.5f + getRandom(x, y) * 0.25f);
            //}
            //foreach (var pos in _controllerPositions)
            //{
            //    for (int x = pos.x - 1; x <= pos.x + 1; ++x)
            //        for (int y = pos.y - 1; y <= pos.y + 1; ++y)
            //            if (x >= 0 && x <= size - 1 && y >= 0 && y <= size - 1)
            //                wallHeight[x, y] = Math.Min(wallHeight[x, y], .35f + getRandom(x, y) * 0.5f);
            //}
            //foreach (var pos in _powerBankPositions)
            //{
            //    for (int x = pos.x - 1; x <= pos.x + 1; ++x)
            //        for (int y = pos.y - 1; y <= pos.y + 1; ++y)
            //            if (x >= 0 && x <= size - 1 && y >= 0 && y <= size - 1)
            //                wallHeight[x, y] = Math.Min(wallHeight[x, y], 0.45f + getRandom(x, y) * 0.25f);
            //}


            // make mesh
            var wallCount = 0;
            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                    if (_wallPositions[x, y])
                        ++wallCount;
            const int quadsPerWall = 5;
            int vertCount = wallCount * 4 * quadsPerWall;
            int triangleCount = wallCount * 6 * quadsPerWall;

            var vertices = new Vector3[vertCount];
            var uv = new Vector2[vertCount];
            var triangles = new int[triangleCount];

            var index = 0;
            var tIndex = 0;

            Action<Vector3, Vector3, Vector3, Vector3> addQuad = (Vector3 A, Vector3 B, Vector3 C, Vector3 D) =>
            {
                vertices[index] = A;
                vertices[index + 1] = B;
                vertices[index + 2] = C;
                vertices[index + 3] = D;
                uv[index] = new Vector2(0, 0);
                uv[index + 1] = new Vector2(0, 1);
                uv[index + 2] = new Vector2(1, 0);
                uv[index + 3] = new Vector2(1, 1);
                triangles[tIndex] = index;
                triangles[tIndex + 1] = index + 1;
                triangles[tIndex + 2] = index + 2;
                triangles[tIndex + 3] = index + 3;
                triangles[tIndex + 4] = index + 2;
                triangles[tIndex + 5] = index + 1;
                index += 4;
                tIndex += 6;
            };

            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                {
                    if (_wallPositions[x, y])
                    {
                        float h = wallHeight[x, y];
                        float h2;
                        var z = size - 1 - y;

                        addQuad(
                             new Vector3(x, h, z),
                             new Vector3(x, h, z + 1),
                             new Vector3(x + 1, h, z),
                             new Vector3(x + 1, h, z + 1));

                        h2 = x > 0 ? wallHeight[x - 1, y] : 0.0f;
                        addQuad(
                            new Vector3(x, h, z + 1),
                            new Vector3(x, h, z),
                            new Vector3(x, h2, z + 1),
                            new Vector3(x, h2, z));

                        h2 = x < (size -1) ? wallHeight[x + 1, y] : 0.0f;
                        addQuad(
                            new Vector3(x + 1, h, z),
                            new Vector3(x + 1, h, z + 1),
                            new Vector3(x + 1, h2, z),
                            new Vector3(x + 1, h2, z + 1));

                        h2 = y > 0 ? wallHeight[x, y - 1] : 0.0f;
                        addQuad(
                            new Vector3(x + 1, h, z + 1),
                            new Vector3(x, h, z + 1),
                            new Vector3(x + 1, h2, z + 1),
                            new Vector3(x, h2, z + 1));

                        h2 = y < (size -1) ? wallHeight[x, y + 1] : 0.0f;
                        addQuad(
                            new Vector3(x, h, z),
                            new Vector3(x + 1, h, z),
                            new Vector3(x, h2, z),
                            new Vector3(x + 1, h2, z));
                    }
                }

            Mesh mesh = new Mesh();
            // http://answers.unity.com/answers/1673043/view.html
            /* If you create a Mesh from code the default indexFormat is still 16 bit as it'S enough for more cases and it requires just half the memory than a 32 bit index buffer. 
             * Since they added the indexFormat setting you can now simply set it to 32 bit and the generated index buffer will be 32 bit per index (so it supports up to 4 billion vertices).
             */
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            _wallMesh.mesh = mesh;
        }

        private void generatePlains()
        {
            var plainCount = 0;
            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                    if (_plainPositions[x, y] || _swampPositions[x, y])
                        ++plainCount;
            const int quadsPerWall = 1;
            int vertCount = plainCount * 4 * quadsPerWall;
            int triangleCount = plainCount * 6 * quadsPerWall;

            var vertices = new Vector3[vertCount];
            var uv = new Vector2[vertCount];
            var triangles = new int[triangleCount];

            var index = 0;
            var tIndex = 0;

            Action<Vector3, Vector3, Vector3, Vector3> addQuad = (A, B, C, D) =>
            {
                vertices[index] = A;
                vertices[index + 1] = B;
                vertices[index + 2] = C;
                vertices[index + 3] = D;
                uv[index] = new Vector2(0, 0);
                uv[index + 1] = new Vector2(0, 1);
                uv[index + 2] = new Vector2(1, 0);
                uv[index + 3] = new Vector2(1, 1);
                triangles[tIndex] = index;
                triangles[tIndex + 1] = index + 1;
                triangles[tIndex + 2] = index + 2;
                triangles[tIndex + 3] = index + 3;
                triangles[tIndex + 4] = index + 2;
                triangles[tIndex + 5] = index + 1;
                index += 4;
                tIndex += 6;
            };

            const float terrainSwampHole = -0.3f;
            const float swampRandom = 0.0f;

            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                {
                    var isPlains = _plainPositions[x, y];
                    var isSwamp = _swampPositions[x, y];
                    if (isPlains /*|| isSwamp*/)
                    {
                        var h = isPlains ? 0f : terrainSwampHole + UnityEngine.Random.value * swampRandom;
                        var z = size - 1 - y;

                        addQuad(
                            new Vector3(x, h, z),
                            new Vector3(x, h, z + 1),
                            new Vector3(x + 1, h, z),
                            new Vector3(x + 1, h, z + 1));
                    }
                }

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            _terrainMesh.mesh = mesh;
        }

        private void generateSwamp()
        {
            var swampCount = 0;
            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                    if (_swampPositions[x, y])
                        ++swampCount;
            const int quadsPerWall = 1;
            int vertCount = swampCount * 4 * quadsPerWall;
            int triangleCount = swampCount * 6 * quadsPerWall;

            var vertices = new Vector3[vertCount];
            var uv = new Vector2[vertCount];
            var triangles = new int[triangleCount];

            var index = 0;
            var tIndex = 0;

            Action<Vector3, Vector3, Vector3, Vector3> addQuad = (A, B, C, D) =>
            {
                vertices[index] = A;
                vertices[index + 1] = B;
                vertices[index + 2] = C;
                vertices[index + 3] = D;
                uv[index] = new Vector2(0, 0);
                uv[index + 1] = new Vector2(0, 1);
                uv[index + 2] = new Vector2(1, 0);
                uv[index + 3] = new Vector2(1, 1);
                triangles[tIndex] = index;
                triangles[tIndex + 1] = index + 1;
                triangles[tIndex + 2] = index + 2;
                triangles[tIndex + 3] = index + 3;
                triangles[tIndex + 4] = index + 2;
                triangles[tIndex + 5] = index + 1;
                index += 4;
                tIndex += 6;
            };

            for (int x = 0; x < size; ++x)
                for (int y = 0; y < size; ++y)
                {
                    if (_swampPositions[x, y])
                    {
                        var z = size - 1 - y;

                        addQuad(
                            new Vector3(x, 0, z),
                            new Vector3(x, 0, z + 1),
                            new Vector3(x + 1, 0, z),
                            new Vector3(x + 1, 0, z + 1));
                    }
                }

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            _swampMesh.mesh = mesh;
        }

        private void Deform()
        {
            generateSwamp();
            generatePlains();
            generateWalls2();

            _wallPositions = null;
            _swampPositions = null;
            _plainPositions = null;
        }
    }
}