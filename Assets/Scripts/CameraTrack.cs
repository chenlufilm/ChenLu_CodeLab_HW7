using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    //set which player to follow
    public Transform player;
    
    //camera's relative position to the player's position in the x
    private float cameraOffsetX;
    
    //camera's fixed y and z value
    private float cameraY;
    private float cameraZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set camera's y and z to fixed values, set camera's movement offset to player's x movement
        cameraOffsetX = transform.position.x - player.position.x;
        cameraY = transform.position.y;
        cameraZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //move the camera alongside's player's new movement in its x, tracking the player
        transform.position = new Vector3(player.position.x + cameraOffsetX, cameraY, cameraZ);
    }
}
