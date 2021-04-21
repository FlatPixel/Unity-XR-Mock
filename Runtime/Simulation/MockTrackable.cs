using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockTrackable : MonoBehaviour
{
    protected Pose pose { get { return new Pose(transform.position, transform.rotation); } }
}
