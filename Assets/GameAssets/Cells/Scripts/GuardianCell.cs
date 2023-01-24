using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuardianCell : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMain player;

    public int oxygenConsumed;
   
    public Transform grabPoint;
   

    [SerializeField] private float oxygenTime;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float cellRadius;
    [SerializeField] private float moveDistance;
    [SerializeField] private float waitTime;
    [SerializeField] private LayerMask cellLayer;

    [SerializeField] private GameObject oxygenText;

    [SerializeField] private Collider2D[] cells;
    [SerializeField] private List<GameObject> closerCell;
    public bool isGrabbed;

    [SerializeField] private GameObject guardianCellParticles;
    private AudioSource audioSource;
    [SerializeField] private AudioClip guardianDestroy;
    private GameObject oxygen;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        audioSource = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<CellFusion>().audioSource;
        rb = GetComponent<Rigidbody2D>();
        StartMovement();
        oxygen = Instantiate(oxygenText, transform.position, Quaternion.identity, transform);
        StartCoroutine(ConsumeOxygen());


    }

    // Update is called once per frame
    void Update()
    {
        CellCheck();
       
    }
    private void FixedUpdate()
    {
        CellGrabbed();
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
                if (closerCell[0].CompareTag("BadCell"))
                {
                    rb.velocity = closerCell[0].transform.position * movementSpeed * Time.deltaTime;
                }
               
                //transform.Translate(closerCell[0].transform.position * movementSpeed * Time.deltaTime);
            }
            else
            {
                Vector2 distance = new Vector2(Random.Range(-moveDistance, moveDistance), Random.Range(-moveDistance, moveDistance));
                //transform.Translate(distance * movementSpeed * Time.deltaTime);
                rb.velocity = distance * movementSpeed * Time.deltaTime;
            }
        }



        yield return new WaitForSeconds(waitTime);
        StartMovement();
    }
   
    private void CellCheck()
    {
        cells = Physics2D.OverlapCircleAll(transform.position, cellRadius, cellLayer);


        for (int i = 0; i < cells.Length; i++)
        {
            if (!closerCell.Contains(cells[i].gameObject) && cells[i].gameObject != this.gameObject)
            {
                if (cells[i].CompareTag("BadCell"))
                {
                    closerCell.Add(cells[i].gameObject);
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
    IEnumerator ConsumeOxygen()
    {
        player.oxygen -= oxygenConsumed;
        oxygen.transform.position = transform.position;
        oxygen.GetComponentInChildren<TMP_Text>().text = oxygenConsumed.ToString();
        oxygen.SetActive(true);
        yield return new WaitForSeconds(1f);
        oxygen.SetActive(false);
        StartCoroutine(ConsumeOxygen());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BadCell"))
        {
            if (!isGrabbed)
            {
                player.guardianCreated = false;
                Instantiate(guardianCellParticles, grabPoint.position, Quaternion.identity);
                audioSource.PlayOneShot(guardianDestroy);
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
            
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, cellRadius);
    }
}
