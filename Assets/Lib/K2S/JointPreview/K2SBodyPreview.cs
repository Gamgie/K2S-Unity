﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class K2SBodyPreview : MonoBehaviour
{

    public int targetOrderedID;
    private K2SBody _body;

    public K2SJointPreview jointPrefab;
    List<K2SJointPreview> joints;

    TextMesh tm;

    // Use this for initialization
    void Start()
    {
        //Debug.Log(jointCount + " joints");
        joints = new List<K2SJointPreview>();

        for (int i = 0; i < K2SBody.NUM_JOINTS; i++)
        {
            K2SJointPreview j = GameObject.Instantiate(jointPrefab).GetComponent<K2SJointPreview>();
            joints.Add(j);
            j.transform.parent = transform;
            j.jointType = (K2SJoint.JointType)i;
        }

        tm = GetComponentInChildren<TextMesh>();

        K2SClient.bodyEntered += bodyEntered;
        K2SClient.bodyLeft += bodyLeft;
    }

    void bodyEntered(K2SBody body)
    {
        if (this.body == null) this.body = body;
    }

    void bodyLeft(int bodyID)
    {
        if (body.bodyId == bodyID)
        {
            body = K2SClient.getOldestBody();
        }
    }

    void bodyUpdate()
    {
        tm.transform.localPosition = body.getPosition();
        tm.text = "ID :" + body.bodyId + "\nAge :" + body.age;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        body = null;
    }

    public K2SBody body
    {
        get { return _body; }
        set
        {
            if (body == value) return;
            if (body != null)
            {
                body.bodyUpdate += bodyUpdate;
            }

            _body = value;

            if (value != null)
            {
                body.bodyUpdate += bodyUpdate;
                foreach (K2SJointPreview jp in joints)
                {
                    jp.joint = body.joints[jp.jointType];

                }


            }
            else
            {
                foreach (K2SJointPreview jp in joints)
                {
                    jp.joint = null;

                }

                GetComponentInChildren<TextMesh>().text = "No Body Assigned";
            }

        }
    }
}
