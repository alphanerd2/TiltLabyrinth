// using UnityEngine;
// public class FloorScript : MonoBehaviour 
// {
//     public GameObject LevelParent;
//     public int AxisLock = 0; // 0 = both, 1 = vertical, 2 = horizontal, 4 = disabled
//     private GameObject Player;
//     private Rigidbody PlayerRigidbody;
//     private float Springyness = 0.95f; // how aggressively it springs back to a flat position
//     private float RotationBoundry = 35; // how many degrees in each direction the floor can turn
//     private void Start()
//     {
//         Player = GameObject.FindGameObjectWithTag("Player");
//         PlayerRigidbody = Player.GetComponent<Rigidbody>();
//     }
//
//     void FixedUpdate() 
//     {
//         // Get Current Rotation and clamp it so that it's not rotated farther than is allowed and also spring it back towards being flat
//         Vector3 rot = this.transform.rotation.eulerAngles;
//         if(rot.x < 150) // it behaves differently depending on whether it's below 0/360 or above - bit awkward to explain. 
//             rot.x = Mathf.Clamp(rot.x * Springyness, -RotationBoundry, RotationBoundry);
//         else
//             rot.x = Mathf.Clamp(((rot.x - 360) * Springyness) + 360, 360 - RotationBoundry, 360 + RotationBoundry);
//
//         rot.y = 0;
//
//         if (rot.z < 150)
//             rot.z = Mathf.Clamp(rot.z * Springyness, -35, 35);
//         else
//             rot.z = Mathf.Clamp(((rot.z - 360) * Springyness) + 360, 360 - RotationBoundry, 360 + RotationBoundry);
//
//
//         // swap the parent around so that the floor is rotated relative to where the ball is rather than where it was before
//         this.transform.rotation = Quaternion.Euler(Vector3.zero);
//         LevelParent.transform.parent = null;
//         this.transform.position = Player.transform.position;
//         LevelParent.transform.parent = this.transform;
//         this.transform.eulerAngles = rot;
//
//
//         // Add a force to the ball to make it a bit more responsive
//         Vector3 force = new Vector3(0, 0, 0);
//         if(AxisLock != 1)
//             force.x = Mathf.Clamp(/*Input.GetAxisRaw("Mouse Y") + */Input.GetAxis("Horizontal"), -1, 1);
//         else
//             force.x = Mathf.Clamp(/*Input.GetAxisRaw("Mouse Y") + */ 0, -1, 1);
//
//         force.y = 0;
//
//         if(AxisLock != 2)
//             force.z = -Mathf.Clamp(/*Input.GetAxisRaw("Mouse X") + */-Input.GetAxis("Vertical"), -1, 1);
//         else
//             force.z = -Mathf.Clamp(/*Input.GetAxisRaw("Mouse X") + */0, -1, 1);
//
//         PlayerRigidbody.AddForce(force / 15, ForceMode.Impulse);
//
//         // Add new rotation to the floor.
//         if (AxisLock != 4)
//         {
//             Vector3 addedRotation = new Vector3(0, 0, 0);
//             if (AxisLock != 2)
//                 addedRotation.x = Mathf.Clamp(/*Input.GetAxisRaw("Mouse Y") + */Input.GetAxis("Vertical"), -1, 1);
//             else
//                 addedRotation.x = Mathf.Clamp(/*Input.GetAxisRaw("Mouse Y") + */0, -1, 1);
//
//             addedRotation.y = 0;
//             if (AxisLock != 1)
//                 addedRotation.z = -Mathf.Clamp(/*Input.GetAxisRaw("Mouse X") + */Input.GetAxis("Horizontal"), -1, 1);
//             else
//                 addedRotation.z = -Mathf.Clamp(/*Input.GetAxisRaw("Mouse X") + */0, -1, 1);
//
//             this.transform.Rotate(addedRotation);
//         }
//         
//         // Return to main menu on pressing escape
//         if (Input.GetKey(KeyCode.Escape)) 
//         {
//             GameObject.FindGameObjectWithTag("Fade").GetComponent<FunctionsScript>().SceneToLoad = "MainMenu";
//             GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>().Play("FadeOnAnim");
//         }
//     }
// }