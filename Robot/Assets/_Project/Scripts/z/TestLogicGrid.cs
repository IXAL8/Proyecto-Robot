using UnityEngine;

namespace Robot
{
    public class TestLogicGrid : MonoBehaviour
    {
        [Header("Referencias de escena")]
        [SerializeField] private N_InventoryGridView gridView;
        [SerializeField] private GameObject itemPrefab;

        [Header("Configuración del grid")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;

        [Header("Ítems de prueba")]
        [SerializeField] private N_ChipsData pistolData;
        [SerializeField] private N_ChipsData arrowData;
        [SerializeField] private N_ChipsData lShapeData;

        private N_InventoryGrid grid;

        void Start()
        {
            // 1. Crear el modelo
            grid = new N_InventoryGrid(gridWidth, gridHeight);

            // 2. Inicializar la vista del grid (dibuja las celdas de fondo)
            gridView.Initialize(grid);

            // 3. Crear y colocar ítems de prueba
            //SpawnItem(pistolData, new Vector2Int(0, 0));
            SpawnItem(arrowData, new Vector2Int(0, 0));
            //SpawnItem(lShapeData, new Vector2Int(7, 0));
        }

        private void SpawnItem(N_ChipsData data, Vector2Int position)
        {
            N_ChipsClass item = new N_ChipsClass(data);

            if (!grid.PlaceChip(item, position))
            {
                Debug.LogWarning($"No se pudo colocar {data.name} en {position}");
                return;
            }

            GameObject itemGO = Instantiate(itemPrefab);
            N_InventoryItemView itemView = itemGO.GetComponent<N_InventoryItemView>();
            //itemView.Initialize(item, grid, gridView);
        }
    }
}
