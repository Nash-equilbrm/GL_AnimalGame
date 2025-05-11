using System.Collections.Generic;
using Commons;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Config;
using Patterns;
using System;
using System.Linq;
using DG.Tweening;
using Game.Level;


namespace Game.Map
{
    public class ProceduralMapGenerator : MonoBehaviour
    {
        public enum PositionInMatrixEnum
        {
            TOP_LEFT = 0,
            TOP_RIGHT = 1,
            BOTTOM_LEFT = 2,
            BOTTOM_RIGHT = 3,
        }

        [Serializable]
        public struct ObstacleOnMap
        {
            public string obstacleDecorateTag;
            public float step;
        }
        
        
        [Header("Prefabs")]
        public GameObject startPrefab;
        public GameObject endPrefab;
        public Material perlinNoiseMaterial;
        public RenderTexture _renderTexture;
        public ObstacleOnMap[] obstacles;
        private float[] _steps;

        [SerializeField] private int _totalEnemy = 0;
        [Tooltip("The area of walkable zone for enemies. The map generator will calculate and set enemies on these area in map.")]
        [SerializeField] private Vector2 _walkableAreaSize = new(3, 3);



        [Header("Map Generation")]
        [SerializeField] private int _width = 20;
        [SerializeField] private int _height = 20;
        [SerializeField] private float _scale = 30f;



        private float[,] _grid;
        private List<GameObject> _listBlockGameObjects = new();
        private Texture2D _noiseTexture;
        private List<Vector2> _walkableAreas = new();
        private Dictionary<PositionInMatrixEnum, Vector2> _extremeWalkableAreas = new();

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private List<List<Vector3>> enemies;


        private void Awake()
        {
            _steps = obstacles.Select(x => x.step).ToArray();
        }

        #region Props
        public Vector3 StartPosition { get => _startPosition; set => _startPosition = value; }
        public Vector3 EndPosition { get => _endPosition; set => _endPosition = value; }
        public List<GameObject> ListDestroyGameObjects { get => _listBlockGameObjects; set => _listBlockGameObjects = value; }
        public Vector2 MapSize => new(_width, _height);
        public List<List<Vector3>> Enemies { get => enemies; set => enemies = value; }

        #endregion


        private void GenerateNoiseTexture()
        {
            _renderTexture.enableRandomWrite = true;
            _renderTexture.Create();

            perlinNoiseMaterial.SetFloat("_Scale", _scale);
            perlinNoiseMaterial.SetFloat("_OffsetX", Random.Range(0f, 1000f));
            perlinNoiseMaterial.SetFloat("_OffsetY", Random.Range(0f, 1000f));

            RenderTexture.active = _renderTexture;
            Graphics.Blit(null, _renderTexture, perlinNoiseMaterial);
            RenderTexture.active = null;

            _noiseTexture = new Texture2D(_width, _height, TextureFormat.RFloat, false);
            RenderTexture.active = _renderTexture;
            _noiseTexture.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            _noiseTexture.Apply();
            RenderTexture.active = null;
        }

        private void GenerateGrid()
        {
            _grid = new float[_width, _height];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float perlinValue = _noiseTexture.GetPixel(x, y).r;
                    if (perlinValue <= 0.6f)
                    {
                        _grid[x, y] = 0;
                    }
                    else
                    {
                        _grid[x, y] = FindClosestStep(perlinValue);
                    }
                }
            }
        }

        // Union Find algorithm
        private List<HashSet<Vector2Int>> FindDisconnectedRegions()
        {
            List<HashSet<Vector2Int>> regions = new();
            HashSet<Vector2Int> visited = new();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    if (_grid[x, y] == 0f && !visited.Contains(cell))
                    {
                        HashSet<Vector2Int> region = new();
                        Queue<Vector2Int> queue = new();
                        queue.Enqueue(cell);
                        visited.Add(cell);

                        while (queue.Count > 0)
                        {
                            Vector2Int current = queue.Dequeue();
                            region.Add(current);

                            foreach (Vector2Int dir in new Vector2Int[]
                                     {
                                         new Vector2Int(1, 0), new Vector2Int(-1, 0),
                                         new Vector2Int(0, 1), new Vector2Int(0, -1)
                                     })
                            {
                                Vector2Int neighbor = current + dir;
                                if (neighbor.x >= 0 && neighbor.x < _width && neighbor.y >= 0 && neighbor.y < _height &&
                                    _grid[neighbor.x, neighbor.y] == 0f && !visited.Contains(neighbor))
                                {
                                    queue.Enqueue(neighbor);
                                    visited.Add(neighbor);
                                }
                            }
                        }

                        regions.Add(region);
                    }
                }
            }

            return regions;
        }

        private void EnsureConnectivity()
        {
            List<HashSet<Vector2Int>> regions = FindDisconnectedRegions();

            if (regions.Count <= 1)
                return;

            for (int i = 1; i < regions.Count; i++)
            {
                ConnectRegions(regions[0], regions[i]);
            }
        }

        private void ConnectRegions(HashSet<Vector2Int> regionA, HashSet<Vector2Int> regionB)
        {
            Vector2Int bestA = Vector2Int.zero;
            Vector2Int bestB = Vector2Int.zero;
            float bestDistance = float.MaxValue;

            foreach (Vector2Int cellA in regionA)
            {
                foreach (Vector2Int cellB in regionB)
                {
                    float distance = Vector2Int.Distance(cellA, cellB);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestA = cellA;
                        bestB = cellB;
                    }
                }
            }

            CreatePathBetween(bestA, bestB);
        }

        private void CreatePathBetween(Vector2Int start, Vector2Int end)
        {
            Vector2Int current = start;

            while (current != end)
            {
                _grid[current.x, current.y] = 0f;

                if (Mathf.Abs(end.x - current.x) > Mathf.Abs(end.y - current.y))
                {
                    current.x += (end.x > current.x) ? 1 : -1;
                }
                else
                {
                    current.y += (end.y > current.y) ? 1 : -1;
                }
            }
        }

        private void PlaceStartAndEndCheckPoint(Transform parent)
        {
            // place START checkpoint at top left area and END point at bottom right area
            Vector2 topLeftArea = _extremeWalkableAreas[PositionInMatrixEnum.TOP_LEFT];
            StartPosition = new(
                topLeftArea.x + (_walkableAreaSize.x - 1) / 2f,
                0f,
                topLeftArea.y + (_walkableAreaSize.y - 1) / 2f
                );
            var startCheckPoint = Instantiate(startPrefab, parent);
            startCheckPoint.transform.position = StartPosition;
            startCheckPoint.tag = Constants.START_CHECKPOINT_TAG;
            ListDestroyGameObjects.Add(startCheckPoint);

            Vector2 bottomRightArea = _extremeWalkableAreas[PositionInMatrixEnum.BOTTOM_RIGHT];
            EndPosition = new(
                bottomRightArea.x + (_walkableAreaSize.x - 1) / 2f,
                0f,
                bottomRightArea.y + (_walkableAreaSize.y - 1) / 2f
                );
            var endCheckPoint = Instantiate(endPrefab, parent);
            endCheckPoint.transform.position = EndPosition;
            endCheckPoint.tag = Constants.END_CHECKPOINT_TAG;
            ListDestroyGameObjects.Add(endCheckPoint);
            this.PubSubBroadcast(EventID.OnFinishPlacingSpawn, this);
        }

        private void PlaceEnemy(int totalEnemies)
        {
            if (_walkableAreas.Count < 2)
            {
                LogUtility.ValidInfo("Generator.PlaceEnemy broadcast", "no data");
                this.PubSubBroadcast(EventID.OnFinishInitAreaForEnemies, Enemies);
                return;
            }

            List<Vector2> availableAreas = new List<Vector2>(_walkableAreas);
            totalEnemies = Mathf.Min(totalEnemies, availableAreas.Count);

            Enemies = new List<List<Vector3>>();

            for (int i = 0; i < totalEnemies; i++)
            {
                int randomIndex = Random.Range(0, availableAreas.Count);
                Vector2 chosenArea = availableAreas[randomIndex];
                availableAreas.RemoveAt(randomIndex);

                List<Vector3> points;
                FindPatrolPoints(chosenArea, out points);
                if (points != null && points.Count > 0) 
                {
                    Enemies.Add(points);
                }
            }
            LogUtility.ValidInfo("Generator.PlaceEnemy broadcast", "has data");
            this.PubSubBroadcast(EventID.OnFinishInitAreaForEnemies, Enemies);
        }




        private void FindPatrolPoints(Vector2 area, out List<Vector3> listPoints)
        {
            LogUtility.ValidInfo("Generator.FindPatrolPoints");
            Vector3 basePos = new Vector3(area.x, 0, area.y) * Constants.CELL_SIZE;

            Vector3 topLeft = basePos + new Vector3(0, 0, _walkableAreaSize.y - 1);
            Vector3 topRight = basePos + new Vector3(_walkableAreaSize.x - 1, 0, _walkableAreaSize.y - 1);
            Vector3 bottomLeft = basePos;
            Vector3 bottomRight = basePos + new Vector3(_walkableAreaSize.x - 1, 0, 0);

            Vector3[] points = { topLeft, topRight, bottomLeft, bottomRight };


            var patrol3D = points.Where(x => (x - _startPosition).magnitude > 5f);
            if (patrol3D.Count() < 2)
            {
                listPoints = null;
                return;
            }
            List<Vector3> list = new (patrol3D);
            int max = Random.Range(2, list.Count);
            LogUtility.ValidInfo("Generator.FindPatrolPoints 1", $"max = {max}");

            listPoints = new();
            for (int i = 0; i < max; i++)
            {
                LogUtility.ValidInfo($"Generator.FindPatrolPoints.forloop {i}");
                int randIndex = Random.Range(i, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
                listPoints.Add(list[i]);
            }
            LogUtility.ValidInfo("Generator.FindPatrolPoints 2");

        }


        private List<Vector2> GetPatrolPointSetFrom(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 X)
        {
            List<Vector2> quadrilateral = new List<Vector2> { A, B, C, D };

            if (!Common.IsPointInQuadrilateral(A, B, C, D, X))
            {
                return quadrilateral;
            }

            List<Vector2[]> triangleCombinations = new List<Vector2[]>()
            {
                new Vector2[] { A, B, C },
                new Vector2[] { A, B, D },
                new Vector2[] { A, C, D },
                new Vector2[] { B, C, D }
            };

            foreach (var triangle in triangleCombinations)
            {
                if (!Common.IsPointInTriangle(triangle[0], triangle[1], triangle[2], X))
                {
                    return new List<Vector2> { triangle[0], triangle[1], triangle[2] };
                }
            }

            List<Vector2[]> lineCombinations = new List<Vector2[]>
        {
            new Vector2[] { A, B },
            new Vector2[] { A, C },
            new Vector2[] { A, D },
            new Vector2[] { B, C },
            new Vector2[] { B, D },
            new Vector2[] { C, D }
        };

            foreach (var line in lineCombinations)
            {
                if (!Common.IsPointOnLine(line[0], line[1], X))
                {
                    return new List<Vector2> { line[0], line[1] };
                }
            }

            return new List<Vector2>();
        }



        private void InstantiatePrefabs(int totalEnemies, Transform parent)
        {
            int cnt = 0;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Vector3 position = new Vector3(x, 0, y) * Constants.CELL_SIZE;
                    if (_grid[x, y] != 0f)
                    {
                        GameObject obj = ObjectPooling.Instance.GetPool(
                            ((x + y) % 2 == 0) ? Constants.BLOCK_1_POOL_TAG : Constants.BLOCK_2_POOL_TAG
                            ).Get(position + Vector3.up * -1f, Vector3.zero);

                        obj.transform.DOMove(obj.transform.position + Vector3.up * 1f, .5f).SetEase(Ease.OutCubic).SetDelay(cnt * .03f);
                        var randomScale = Random.Range(0f, 1f);
                        if (randomScale > 0.65f)
                        {
                            var obsPrefab = obstacles.FirstOrDefault(i => Math.Abs(i.step - _grid[x, y]) < Mathf.Epsilon);
                            //var obstacleObj = Instantiate(obsPrefab.prefab, parent: obj.transform);
                            var obstacleObj = ObjectPooling.Instance.GetPool(obsPrefab.obstacleDecorateTag).Get(parent: obj.transform);
                            obstacleObj.transform.SetLocalPositionAndRotation(new Vector3(0f, 1f, 0f), Quaternion.identity);
                            obstacleObj.transform.localScale = Vector3.zero;
                            obstacleObj.transform.DOScale(Vector3.one * randomScale, 1f).SetEase(Ease.InCubic).SetDelay(cnt * .05f);
                        }
                        obj.transform.SetParent(parent);
                        cnt++;
                    }
                }
            }
            PlaceStartAndEndCheckPoint(parent);
            LogUtility.ValidInfo("Generator.InstantiatePrefabs");
            PlaceEnemy(totalEnemies);
        }



        internal void GenerateMap(LevelConfig config, Transform parent)
        {
            _width = config.mapSize[0];
            _height = config.mapSize[1];
            _scale = config.noiseScale;
            int times = 0;
            int totalTry = 100;
            do
            {
                times++;
                Try();
            } while (!ValidGrid() && times < totalTry);

            LogUtility.Info("GenerateMap.Try", $"Map: (w: {_width}, h: {_height}, scale: {_scale}) - {times}/{totalTry} times");
            FindExtremeZeroSubmatrices();
            InstantiatePrefabs(totalEnemies: config.TotalEnemies, parent: parent);

            bool ValidGrid()
            {
                int rows = _grid.GetLength(0);
                int cols = _grid.GetLength(1);
                float totalCells = rows * cols;

                float zeroCount = 0;

                foreach (var value in _grid)
                {
                    if (value > 0f) continue;
                    zeroCount++;
                }
                
                bool enoughZeroQuorum = zeroCount / totalCells >= 0.6f;
                if (enoughZeroQuorum)
                {
                    LogUtility.ValidInfo("ValidGrid", $"Quorum: {zeroCount} / {totalCells} = {zeroCount/totalCells}");
                }
                bool enoughWalkableAreaForEnemies = true;
                FindAllZeroSubmatrices();
                
                return enoughZeroQuorum && enoughWalkableAreaForEnemies;
            }
            void Try()
            {
                ResetMapObjects();
                GenerateNoiseTexture();
                GenerateGrid();
                EnsureConnectivity();
            }
        }

        

        internal void ResetMapObjects()
        {
            foreach (var obj in ListDestroyGameObjects)
            {
                Destroy(obj);
            }

            ListDestroyGameObjects.Clear();

            //ObjectPooling.Instance.GetPool(Constants.BLOCK_1_POOL_TAG).RecycleAll();
            //ObjectPooling.Instance.GetPool(Constants.BLOCK_2_POOL_TAG).RecycleAll();
            foreach (var item in obstacles)
            {
                ObjectPooling.Instance.GetPool(item.obstacleDecorateTag).RecycleAll();
            }
        }

        private void PrintArray()
        {
            string result = "";

            for (int y = _height - 1; y >= 0; y--)
            {
                for (int x = 0; x < _width; x++)
                {
                    result += _grid[x, y] != 0f ? "█" : ".";
                }

                result += "\n";
            }

            Debug.Log("\n" + result);
        }


        private void FindAllZeroSubmatrices()
        {
            int rows = _grid.GetLength(0);
            int cols = _grid.GetLength(1);
            float[,] prefixSum = new float[rows + 1, cols + 1];

            for (int i = 1; i <= rows; i++)
            {
                for (int j = 1; j <= cols; j++)
                {
                    prefixSum[i, j] = _grid[i - 1, j - 1] +
                                      prefixSum[i - 1, j] +
                                      prefixSum[i, j - 1] -
                                      prefixSum[i - 1, j - 1];
                }
            }

            _walkableAreas?.Clear();
            _walkableAreas = new List<Vector2>();

            for (int i = 1; i + _walkableAreaSize.y - 1 <= rows; i++)
            {
                for (int j = 1; j + _walkableAreaSize.x - 1 <= cols; j++)
                {
                    int x1 = i, y1 = j;
                    int x2 = i + (int)_walkableAreaSize.y - 1, y2 = j + (int)_walkableAreaSize.x - 1;

                    float sum = prefixSum[x2, y2] - prefixSum[x1 - 1, y2] - prefixSum[x2, y1 - 1] + prefixSum[x1 - 1, y1 - 1];

                    if (sum == 0)
                    {
                        _walkableAreas.Add(new(i - 1, j - 1));
                    }
                }
            }
        }


        private void FindExtremeZeroSubmatrices()
        {
            if (_walkableAreas == null || _walkableAreas.Count == 0)
            {
                LogUtility.Error("FindExtremeZeroSubmatrices", "No zero submatrix available.");
                return;
            }

            Vector2 topLeft = _walkableAreas[0],
                        bottomRight = _walkableAreas[0],
                        bottomLeft = _walkableAreas[0],
                        topRight = _walkableAreas[0];

            foreach (var v2 in _walkableAreas)
            {
                var (row, col) = (v2.x, v2.y);
                // Top-left-most
                if (row < topLeft.x || (row == topLeft.x && col < topLeft.y))
                    topLeft = new(row, col);

                // Bottom-right-most
                if (row > bottomRight.x || (row == bottomRight.x && col > bottomRight.y))
                    bottomRight = new(row, col);

                // Bottom-left-most
                if (row > bottomLeft.x || (row == bottomLeft.x && col < bottomLeft.y))
                    bottomLeft = new(row, col);

                // Top-right-most
                if (row < topRight.x || (row == topRight.x && col > topRight.y))
                    topRight = new(row, col);
            }

            //LogUtility.ValidInfo("FindExtremeZeroSubmatrices", $"Top-left-most: ({topLeft.x}, {topLeft.y})");
            //LogUtility.ValidInfo("FindExtremeZeroSubmatrices", $"Bottom-right-most: ({bottomRight.x}, {bottomRight.y})");
            //LogUtility.ValidInfo("FindExtremeZeroSubmatrices", $"Bottom-left-most: ({bottomLeft.x}, {bottomLeft.y})");
            //LogUtility.ValidInfo("FindExtremeZeroSubmatrices", $"Top-right-most: ({topRight.x}, {topRight.y})");

            _extremeWalkableAreas?.Clear();
            _extremeWalkableAreas.Add(PositionInMatrixEnum.TOP_LEFT, topLeft);
            _extremeWalkableAreas.Add(PositionInMatrixEnum.TOP_RIGHT, topRight);
            _extremeWalkableAreas.Add(PositionInMatrixEnum.BOTTOM_LEFT, bottomLeft);
            _extremeWalkableAreas.Add(PositionInMatrixEnum.BOTTOM_RIGHT, bottomRight);
        }
        
        
        private float FindClosestStep(float value)
        {
            if (_steps is null || _steps.Length == 0) return 1;
            float closest = _steps[0];

            foreach (float step in _steps)
            {
                if (step <= value && step > closest)
                {
                    closest = step;
                }
            }

            return closest;
        }

        public Vector3 GetMapCenter()
        {
            float centerX = _width / 2f;
            float centerZ = _height / 2f;
            return new Vector3(centerX, 0, centerZ);
        }
    }
}
