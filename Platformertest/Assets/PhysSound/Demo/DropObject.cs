using UnityEngine;
using System.Collections;

public class DropObject : MonoBehaviour 
{
    public GameObject Object;

    public Transform DropLocation;
    public float RandomForce;

    void Start()
    {
        Object.GetComponent<Rigidbody>().maxAngularVelocity = 1000;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Object.transform.position = DropLocation.position;
            Object.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-RandomForce, RandomForce), Random.Range(-RandomForce, RandomForce), 0);
            Object.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * RandomForce;
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    Application.LoadLevel(0);
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //    Application.LoadLevel(1);
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //    Application.LoadLevel(2);
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //    Application.LoadLevel(3);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, 50));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Box(Application.loadedLevelName);
        GUILayout.Box("Press 'Q' to drop object.");
        //GUILayout.Box("Current Object: " + Target.name);
        //GUILayout.Box("'1' '2' '3' or '4' to load different scenes.");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
