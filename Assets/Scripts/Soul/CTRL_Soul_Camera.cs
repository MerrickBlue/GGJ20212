using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class, will make the camera to which is attached to follow the soul Player. It'll find the player by the CTRL_Soul.cs
/// </summary>
public class CTRL_Soul_Camera : MonoBehaviour
{
    //This is how far from the character this 
    [SerializeField] protected float HeightFromPlayer;

    //The private and protected variables that are needed to follow the player.
    protected CTRL_Soul _playerController;
    protected Transform _playerTransform;
    protected float yPosition;
    

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GameObject.FindObjectOfType<CTRL_Soul>();

        if (_playerController == null)
        {
            Debug.LogError("There's no soul character in the scene");
            return;
        }

        //If we've already found a Player controller, we get the transform from it. Eventually if there's time we can set a threshold of movement from the player without the camera followign it.
        _playerTransform = _playerController.GetComponent<Transform>();

        //The Y position of the cámera is given by the height of the player floating + the offset we've set via the inspector.
        yPosition = _playerTransform.position.y + HeightFromPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(_playerTransform.position.x, yPosition, _playerTransform.position.z);
    }
}