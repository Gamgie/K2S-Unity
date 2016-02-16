using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

public class K2SClient : MonoBehaviour {

    OSCServer server;
    public int port = 9090;

    public int scaleFactor = 1;
    public Vector3 originOffset;

    public static bool primaryJointsSelected;
    public static bool secondaryJointsSelected;

    public static Dictionary<int, K2SBody> bodies;
    public static List<K2SBody> orderedBodies;

    public delegate void BodyEnteredHandler(K2SBody body);
    #region Delegates
    public static event BodyEnteredHandler bodyEntered;
    #endregion

    public delegate void BodyLeftHandler(int bodyID);
    #region Delegates
    public static event BodyLeftHandler bodyLeft;
    #endregion

    // Use this for initialization
    void Start () {
        server = new OSCServer(port);
        server.PacketReceivedEvent += packetReceived;
        server.Connect();

        bodies = new Dictionary<int, K2SBody>();
        orderedBodies = new List<K2SBody>();

        Debug.Log("Listening on port " + port);
	}
	
	// Update is called once per frame
	void Update () {
        server.Update();
	}

    void packetReceived(OSCPacket packet)
    {
        //Debug.Log("got packet");
        OSCMessage msg = (OSCMessage)packet;

        switch(msg.Address)
        {

            case "/k2s/body/entered":
                Debug.Log("body entered");
                addBody((int)msg.Data[0]);
                break;

            case "/k2s/body/left":
                Debug.Log("body left");
                int bodyID = (int)msg.Data[0];
                if(bodies.ContainsKey(bodyID))
                {
                    orderedBodies.Remove(bodies[bodyID]);
                    bodies.Remove(bodyID);
                    
                    orderedBodies.Sort(delegate (K2SBody x, K2SBody y)
                    {
                        return y.age.CompareTo(x.age);
                    });

                    if (bodyLeft != null) bodyLeft(bodyID);
                }

               
                break;

            case "/k2s/body/update":
                Debug.Log("body update");
                bodyID = (int)msg.Data[0];
                if (!bodies.ContainsKey(bodyID)) addBody(bodyID);

                bodies[bodyID].update((int)msg.Data[1],(int)msg.Data[2]);
                break;

              
            case "/k2s/joint":
                bodyID = (int)msg.Data[0];
                K2SJoint.JointType jointType = (K2SJoint.JointType)(int)msg.Data[1];
                K2SJoint joint = bodies[bodyID].joints[jointType];

                Vector3 position = new Vector3((float)msg.Data[2], (float)msg.Data[3], (float)msg.Data[4]);
                Quaternion orientation = Quaternion.Euler((float)msg.Data[5], (float)msg.Data[6], (float)msg.Data[7]);
                int trackingState = (int)msg.Data[8];

                joint.update((position-originOffset)*scaleFactor, orientation, trackingState);
                break;

            case "/k2s/primary":
                primaryJointsSelected = (int)msg.Data[0] == 1;
                break;

            case "/k2s/secondary":
                secondaryJointsSelected = (int)msg.Data[0] == 1;
                break;
                
        }
        
    }

    void OnDestroy()
    {
        server.Close();
    }

    void addBody(int bodyID)
    {
        bodies.Add(bodyID, new K2SBody(bodyID));
        orderedBodies.Add(bodies[bodyID]);
        if (bodyEntered != null) bodyEntered(bodies[bodyID]);
    }

    public static K2SBody getOldestBody()
    {
        if (orderedBodies.Count == 0) return null;
        return orderedBodies[0];
    }
}
