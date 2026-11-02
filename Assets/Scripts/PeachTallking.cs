using UnityEngine;
using TMPro;

public class PeachTallking : MonoBehaviour
{
    //bring in player position
    public Transform player;
    //bring in Peach's text
    public TMP_Text peachText;
    //define player's distance to Peach
    public float peachDistance;

    void Start()
    {
        //make sure the text is not active until player get close to Peach
        peachText.gameObject.SetActive(false);
    }

    void Update()
    {
        //update player's distance to the game object 
        float playerDistance = Vector2.Distance(transform.position, player.position);

        //if player get close to Peach, display the text
        if (playerDistance <= peachDistance)
        {
            peachText.gameObject.SetActive(true);
        }
        else
        //if player is not close to Peach, do not display text
        {
            peachText.gameObject.SetActive(false);
        }
    }

    //if Peach is destroyed, set text display to false
    void OnDisable()
    {
        if (peachText != null)
        {
            peachText.gameObject.SetActive(false);
        }
    }
}