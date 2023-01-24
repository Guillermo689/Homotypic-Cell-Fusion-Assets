using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellFusion : MonoBehaviour
{
    [SerializeField] private float spawnTimer;
    [SerializeField] private Vector2 spawnCoords;
    [SerializeField] private float newCellTimer;
    [SerializeField] private GameObject badCell;
    private GameObject[] cellsInRoom;
    public List<GameObject> activeCells;
    public List<GameObject> cellsToFusion;
    public List<GameObject> cellsToSpawn;
    public List<GameObject> cellLibrary;
    private Vector2 spawnPosition;
    private PlayerMain player;
    private bool minOxygen = false;
    private bool maxOxygen = false;
   
    [SerializeField] private GameObject badParticles;
    [SerializeField] private GameObject nothingParticles;
    [SerializeField] private GameObject cellParticles;
    [SerializeField] private GameObject splats;
  

    [SerializeField] private AudioClip cellSpawn;
    [SerializeField] private AudioClip cellSpawnGood;
    [SerializeField] private AudioClip cellSpawnBad;
    [SerializeField] private AudioClip cellSpawnNothing;
    public AudioClip gameOver;
 

   public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(AddNewCell());
        StartCoroutine(SpawnCell());
    }
    void Update()
    {
       
        //==================== Check all cells in hierarchy ===========================
        cellsInRoom = GameObject.FindGameObjectsWithTag("Cell");
        for (int i = 0; i < cellsInRoom.Length; i++)
        {
            //============== Loop through them and add them to the active cells ==================
            if (!activeCells.Contains(cellsInRoom[i]))
            {
                activeCells.Add(cellsInRoom[i]);
            }
        }
        //============== if there are more than one cell in the fusion list, start the function ===============
        if (cellsToFusion.Count > 1)
        {
            FusioCell();
        }
        
        CheckNullCells();
        RayCheck();
    }

    IEnumerator AddNewCell()
    {
        //========== If library contain cells, add them to the cells to spawn =================
        if (cellLibrary.Count > 0)
        {
            if (!cellsToSpawn.Contains(cellLibrary[0]))
            {
                cellsToSpawn.Add(cellLibrary[0]);
                cellLibrary.Remove(cellLibrary[0]);
            }
        }
        //============= If library is empty, stop the coroutine =================================
        else
        {
            StopCoroutine(AddNewCell());
        }
        yield return new WaitForSeconds(newCellTimer);
        //============== If there are cells in library, start the coroutine again ==================
        if (cellLibrary.Count > 0)
        {
            StartCoroutine(AddNewCell());
        }
       
    }
    private void RayCheck()
    {
        //=================== Check if spawn point don't collide with a cell ========================================
        Vector2 spawnCheck = new Vector2(Random.Range(-spawnCoords.x, spawnCoords.x), Random.Range(-spawnCoords.y, spawnCoords.y));
        RaycastHit2D spawnHit = Physics2D.Raycast(Camera.main.transform.position, spawnCheck);
        if (!spawnHit.collider.CompareTag("Cell") || !spawnHit.collider.CompareTag("BadCell"))
        {
            spawnPosition = spawnCheck;
        }
    }
    IEnumerator SpawnCell()
    {
        //============ Select a random cell from the cells to spawn ========================
        int cellNumber = Random.Range(0, cellsToSpawn.Count);
        //============ Spawn that cell in the spawn position ======================================================
        Instantiate(splats, spawnPosition, Quaternion.identity, transform.parent);
        GameObject newCell = Instantiate(cellsToSpawn[cellNumber], spawnPosition, Quaternion.identity, transform.parent);
        audioSource.PlayOneShot(cellSpawn);
        yield return new WaitForSeconds(spawnTimer);
        //================== wait some seconds and start spawning a cell again, but time gets shorter till 1 second per spawn ============
       if (spawnTimer > 1)
        {
            spawnTimer -= 0.1f;
        }
        StartCoroutine(SpawnCell());

    }
    private void CheckNullCells()
    {
        //=========== Check if there are not null cells in active cells list ======================
        for (int i = 0; i < activeCells.Count; i++)
        {
            if (activeCells[i] == null)
            {
                activeCells.Remove(activeCells[i]);
            }
        }
        //=========== Check if there are not null cells in fusion cells list ======================
        for (int i = 0; i < cellsToFusion.Count; i++)
        {
            if (cellsToFusion[i] == null)
            {
                cellsToFusion.Remove(cellsToFusion[i]);
            }
        }
    }
    

    private void FusioCell()
    {
        //============== cell 0 and 1 are not null, get the main components ==========================
        if (cellsToFusion[0] != null && cellsToFusion[1] != null)
        {
            var cell1 = cellsToFusion[0].GetComponent<CellMain>();
            var cell2 = cellsToFusion[1].GetComponent<CellMain>();
            //============== Check if each cell are not grabbed ==========================
            if (!cell1.isGrabbed && !cell2.isGrabbed)
            {
                //============== if both are the same type, fusion both cells to create a new stronger one ============================
                if (cell1.cellType == cell2.cellType)
                {
                    if (cell1.oxygenProduced == cell2.oxygenProduced)
                    {
                        GameObject cell = Instantiate(cell1.newCell, cell1.transform.position, Quaternion.identity, transform.parent);
                        cell.GetComponent<CellMain>().oxygenProduced = cell1.oxygenProduced + cell2.oxygenProduced;
                    }
                    if (cell1.oxygenProduced > cell2.oxygenProduced)
                    {
                        GameObject cell = Instantiate(cellsToFusion[0], cell1.transform.position, Quaternion.identity, transform.parent);
                        cell.GetComponent<CellMain>().oxygenProduced = cell1.oxygenProduced + cell2.oxygenProduced;
                    }
                    if (cell1.oxygenProduced < cell2.oxygenProduced)
                    {
                        GameObject cell = Instantiate(cellsToFusion[1], cell1.transform.position, Quaternion.identity, transform.parent);
                        cell.GetComponent<CellMain>().oxygenProduced = cell1.oxygenProduced + cell2.oxygenProduced;
                    }


                    Instantiate(cellParticles, cell1.transform.position, Quaternion.identity, transform.parent);
                    audioSource.PlayOneShot(cellSpawnGood);
                   
                    DestroyCell(cell1, cell2);
                }
                //============== if both cells are different, roll probability if they are destroyed of spawn a Bad Cell ===================
                if (cell1.cellType != cell2.cellType)
                {
                    float spawnProbability;
                    if (!maxOxygen)
                    {
                        spawnProbability = Random.Range(0, 100);
                    }
                    else
                    {
                        spawnProbability = 100;
                    }
                    if (spawnProbability > 60)
                    {
                        if (player.oxygen > 100)
                        {
                            minOxygen = true;
                        }
                        if (minOxygen)
                        {
                            //============== Check if Bad cell is not already spawned then spawn it and run the function to destroy both cells ==========
                            if (!activeCells.Contains(badCell))
                            {
                                GameObject cell = Instantiate(badCell, cell1.transform.position, Quaternion.identity, transform.parent);
                                audioSource.PlayOneShot(cellSpawnBad);
                                if (player.oxygen > 500)
                                {
                                    cell.GetComponent<BadCell>().oxygenConsumed = 20;
                                    if (player.oxygen > 1000)
                                    {
                                        cell.GetComponent<BadCell>().oxygenConsumed = 60;
                                        if (player.oxygen > 1500)
                                        {
                                            cell.GetComponent<BadCell>().oxygenConsumed = 100;
                                           
                                        }
                                    }
                                }
                                cell.GetComponent<BadCell>().oxygenConsumed += cell1.oxygenProduced + cell2.oxygenProduced;


                                Instantiate(badParticles, cell1.transform.position, Quaternion.identity, transform.parent);

                                DestroyCell(cell1, cell2);
                            }
                            else
                            {
                                DestroyCell(cell1, cell2);
                                Instantiate(nothingParticles, cell1.transform.position, Quaternion.identity, transform.parent);
                                audioSource.PlayOneShot(cellSpawnNothing);
                            }
                        }
                        if (player.oxygen > 500)
                        {
                            maxOxygen = true;
                        }
                        
                        
                    }
                    //================= If probability roll is less than 60, just run the function to destroy both cells ======================================
                    else
                    {
                        DestroyCell(cell1, cell2);
                        Instantiate(nothingParticles, cell1.transform.position, Quaternion.identity, transform.parent);
                        audioSource.PlayOneShot(cellSpawnNothing);
                    }

                }
                
            }


        }

    }
    private void DestroyCell(CellMain cell1, CellMain cell2)
    {
        //============ Clear the fusion list and destroy both cells ===============================
        cellsToFusion.Clear();
        Destroy(cell1.gameObject);
        Destroy(cell2.gameObject);
    }
}
