using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public bool hasBoots = false;

    public bool hasArmor = false;

    public bool hasHelmet = false;

    public GameObject boots;

    public GameObject armor;

    public GameObject helmet;

    public GameObject helmetLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("boots"))
        {
            hasBoots = true;
            boots.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("armor"))
        {
            hasArmor = true;
            armor.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("helmet"))
        {
            hasHelmet = true;
            helmetLight.SetActive(true);
            helmet.SetActive(false);
        }
    }
}
