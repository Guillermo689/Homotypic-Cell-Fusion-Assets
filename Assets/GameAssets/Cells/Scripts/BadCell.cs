using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BadCell : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMain player;
   
    public int oxygenConsumed;
    [SerializeField] private float bumpForce;
    [SerializeField] private float bumpRadius;
   
    [SerializeField] private float oxygenTime;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float cellRadius;
    [SerializeField] private Vector2 moveDistance;
    [SerializeField] private float waitTime;
    [SerializeField] private LayerMask cellLayer;
 
    [SerializeField] private GameObject oxygenText;
   
    [SerializeField] private Collider2D[] cells;
    [SerializeField] private List<GameObject> closerCell;

    [SerializeField] private GameObject badParticles;
    private AudioSource audioSource;
    [SerializeField] private AudioClip cellEat;
    private GameObject oxygen;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        audioSource = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<CellFusion>().audioSource;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(CellCheck());
        StartMovement();
        oxygen = Instantiate(oxygenText, transform.position, Quaternion.identity, transform);
        StartCoroutine(ConsumeOxygen());

    }

    // Update is called once per frame
    void Update()
    {
      
       
    }
    private void FixedUpdate()
    {
        playerBump();
    }
    public void StartMovement()
    {
        
        StartCoroutine(Movement());
    }
    IEnumerator Movement()
    {
       //=========== If there is a cell close, chase it to eat it =================================
            if (closerCell.Count > 0 && closerCell[0] != null)
            {
                rb.velocity = closerCell[0].transform.position * movementSpeed * Time.deltaTime;
                //transform.Translate(closerCell[0].transform.position * movementSpeed * Time.deltaTime);
            }
            //=========== if there is not a cell close, select a random distance and go there ===================
            else
            {
                Vector2 distance = new Vector2(Random.Range(-moveDistance.x, moveDistance.x), Random.Range(-moveDistance.y, moveDistance.y));
          
                rb.velocity = distance * movementSpeed * Time.deltaTime;
                //transform.Translate(distance * movementSpeed * Time.deltaTime);
            }
        yield return new WaitForSeconds(waitTime);
        StartMovement();
    }
    private void playerBump()
    {
        //============== if player is close, harden so player cannot move it ====================================
        float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= bumpRadius)
        {
            rb.isKinematic = true;            
        }
        else
        {
            rb.isKinematic = false;
        }
    }
    IEnumerator CellCheck()
    {
        //=================== Check cells in radius ==========================================
        cells = Physics2D.OverlapCircleAll(transform.position, cellRadius, cellLayer);
        //=================== Loop for all the cells in radius and check if is not on the closerCell list, then add it =====================
        for (int i = 0; i < cells.Length; i++)
        {
            if (!closerCell.Contains(cells[i].gameObject) && cells[i].gameObject != this.gameObject)
            {
                closerCell.Add(cells[i].gameObject);
            }
        }
        //========================= Check if any cell in the closerCell list is null, then remove it ==========================
        for (int i = 0; i < closerCell.Count; i++)
        {
            if (closerCell[i] == null)
            {
                closerCell.Remove(closerCell[i]);
            }
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(CellCheck());
    }
    IEnumerator ConsumeOxygen()
    {
        //================== Consume oxigen and  instantiate text ======================================
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
        //=================== if collision with a cell, consume it and consume more oxygen =========================
        if (collision.gameObject.CompareTag("Cell"))
        {
            if (!collision.gameObject.GetComponent<CellMain>().isGrabbed)
            {
                oxygenConsumed += collision.gameObject.GetComponent<CellMain>().oxygenProduced;
                Instantiate(badParticles,collision.transform.position, Quaternion.identity);
                audioSource.PlayOneShot(cellEat);
                Destroy(collision.gameObject);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bumpRadius);
    }
}
