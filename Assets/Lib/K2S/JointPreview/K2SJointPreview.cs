using UnityEngine;
using System.Collections;

public class K2SJointPreview : MonoBehaviour
{

    private K2SJoint _joint;
    public K2SJoint.JointType jointType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (joint == null) return;
        transform.localPosition = joint.position;
        transform.localRotation = joint.orientation;
    }

    public K2SJoint joint
    {
        get { return _joint; }
        set
        {
            _joint = value;
            GetComponent<Renderer>().material.color = (joint != null) ? Color.white : Color.grey;
        }
    }
}
