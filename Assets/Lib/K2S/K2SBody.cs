using UnityEngine;
using System.Collections.Generic;


public class K2SBody {

    public static int NUM_JOINTS = 25;


    public delegate void BodyUpdateHandler();
    #region Delegates
    public event BodyUpdateHandler bodyUpdate;
    #endregion


    public int bodyId;
    public int orderedId;

    public int leftHandState;
    public int rightHandState;

    public float birthTime;
    public float age;

    public Dictionary<K2SJoint.JointType,K2SJoint> joints;

    public K2SBody(int bodyID)
    {

        this.bodyId = bodyID;
        joints = new Dictionary<K2SJoint.JointType, K2SJoint>();
        for (int i = 0; i < NUM_JOINTS; i++)
        {
            joints.Add((K2SJoint.JointType)i,new K2SJoint(this, (K2SJoint.JointType)i));
        }

        birthTime = Time.time;
        this.age = 0;
    }

    public Vector3 getPosition()
    {
        return joints[K2SJoint.JointType.JointType_SpineMid].position;
    }

    public void update(int leftHandState,int rightHandState)
    {
        this.age = Time.time - birthTime;
        this.leftHandState = leftHandState;
        this.rightHandState = rightHandState;
        if (bodyUpdate != null) bodyUpdate();
    }
}
