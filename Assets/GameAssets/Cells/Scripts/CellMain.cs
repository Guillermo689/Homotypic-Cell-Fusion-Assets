using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CellMain : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMain player;
    public int cellType;
    public int oxygenProduced;
    public bool isGrabbed;
    public Transform grabPoint;
    private CellFusion fusionScript;
    [SerializeField] private float oxygenTime;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float cellRadius;
    [SerializeField] private float moveDistance;
    [SerializeField] private float waitTime;
    [SerializeField] private LayerMask cellLayer;
    public GameObject newCell;
    [SerializeField] private GameObject oxygenText;
    private bool isFused = false;
    private bool canFusion = false;
    [SerializeField] private Collider2D[] cells;
    [SerializeField] private List<GameObject> closerCell;

    [SerializeField] private float cellHealth;
    private GameObject oxygen;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(CellCheck());
        StartMovement();
        oxygen = Instantiate(oxygenText, transform.position, Quaternion.identity, transform);
        StartCoroutine(ProduceOxygen());
        fusionScript = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<CellFusion>();

    }

    
    private void FixedUpdate()
    {
        CellGrabbed();
       
    }
    private void CellWither()
    {
        if (cellHealth <= 5f)
        {
            //Wither animation;
        }
    }
    private void CellDeath()
    {
        if (cellHealth <= 0)
        {
            if (!isGrabbed)
            {
                Destroy(gameObject);
            }
            
        }
    }
    IEnumerator CellCheck()
    {
        if (!isGrabbed)
        {
            cells = Physics2D.OverlapCircleAll(transform.position, cellRadius, cellLayer);
            for (int i = 0; i < cells.Length; i++)
            {
                if (!closerCell.Contains(cells[i].gameObject) && cells[i].gameObject != this.gameObject)
                {
                    if (cells[i].TryGetComponent(out CellMain cellScript))
                    {
                        if (!cellScript.isGrabbed)
                        {
                            closerCell.Add(cells[i].gameObject);
                        }
                    }
                }
            }
            for (int i = 0; i < closerCell.Count; i++)
            {
                if (closerCell[i] == null)
                {
                    closerCell.Remove(closerCell[i]);
                }
            }
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(CellCheck());
    }
    private void CellGrabbed()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (isGrabbed)
        {
            collider.enabled = false;
            transform.position = grabPoint.position;
                StopCoroutine(Movement());
        }
        
        else
        {
            collider.enabled = true;
        }

    }
    IEnumerator ProduceOxygen()
    {
        player.oxygen += oxygenProduced;
        oxygen.transform.position = transform.position;
        oxygen.SetActive(true);
        oxygen.GetComponentInChildren<TMP_Text>().text = oxygenProduced.ToString();        
        yield return new WaitForSeconds(1f);
        oxygen.SetActive(false);
        StartCoroutine(ProduceOxygen());
    }
    public void StartMovement()
    {
        StartCoroutine(Movement());
    }
   IEnumerator Movement()
    {
        if (!isGrabbed)
        {
            if (closerCell.Count > 0 && closerCell[0] != null)
            {

                rb.velocity = closerCell[0].transform.position * movementSpeed * Time.deltaTime;
                //transform.Translate(closerCell[0].transform.position * movementSpeed * Time.deltaTime);
            }
            else
            {
                Vector2 distance = new Vector2(Random.Range(-moveDistance, moveDistance), Random.Range(-moveDistance, moveDistance));
                if (distance.x > 6 || distance.x < -6 || distance.y > 4 || distance.y < -4)
                {
                    yield return null;
                    StartMovement();
                }
                rb.velocity = distance * movementSpeed * Time.deltaTime;
                //transform.Translate(distance * movementSpeed * Time.deltaTime);
            }


        }
        yield return new WaitForSeconds(waitTime);
        StartMovement();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, cellRadius);
    }
    private void OnTriggerEnter(Collider collider)
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cell"))
        {
            if (!fusionScript.cellsToFusion.Contains(gameObject))
            {
                fusionScript.cellsToFusion.Add(gameObject);

            }
        }
    }
}
