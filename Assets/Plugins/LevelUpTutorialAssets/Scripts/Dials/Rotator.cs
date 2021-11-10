using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using NaughtyAttributes;

namespace LevelUP.Dial
{
    public class Rotator : MonoBehaviour
    {
        public Transform linkedDialVisual;
        [Space(5)]

        [Header("Rotation Axis")]
        [Dropdown("axisValues")]
        public string axis;
        private string[] axisValues = new string[] { "X", "Y", "Z" };

        [HorizontalLine]

        [SerializeField] private int snapRotationAmout = 25;
        [SerializeField] private float angleTolerance;

        public float startAngle;
        [Space(10)]
        public bool clampRotation;
        public float angleMinMax;

        private XRBaseInteractor interactor;
        //private float startAngle;
        private bool requiresStartAngle = true;
        private bool shouldGetHandRotation = false;
        private Rigidbody myRB;

        private void Start()
        {
            myRB = GetComponent<Rigidbody>();

            switch (axis)
            {
                //case "X":
                //    myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                //    break;
                //case "Y":
                //    myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                //    break;
                //case "Z":
                //    myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
                //    break;
                //default:
                //    myRB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                //    break;
            }

            //if(axis == "X")
            //{

            //}
        }



        public void GrabbedBy()
        {
            interactor = GetComponent<XRGrabInteractable>().selectingInteractor;
            //interactor.GetComponent<XRDirectInteractor>().hideControllerOnSelect = true;

            shouldGetHandRotation = true;
            startAngle = 0f;

            //HandModelVisibility(true);
        }

        private void HandModelVisibility(bool visibilityState)
        {
            //if(interactor.gameObject.GetComponent<XRController>().controllerNode == XRNode.RightHand)
            //{
            //    RighthandModel.SetActive(visibilityState);
            //}
            //else
            //{
            //    LefthandModel.SetActive(visibilityState);
            //}
        }

        public void GrabEnd()
        {
            shouldGetHandRotation = false;
            requiresStartAngle = true;
            //HandModelVisibility(false);
        }

        void Update()
        {
            if (shouldGetHandRotation)
            {
                var rotationAngle = GetInteractorRotation(); //gets the current controller angle
                GetRotationDistance(rotationAngle);
            }
        }

        public float GetInteractorRotation()
        {
            var handRotation = interactor.GetComponent<Transform>().eulerAngles;
            return handRotation.z;
        }

        private void GetRotationDistance(float currentAngle)
        {
            if (!requiresStartAngle)
            {
                var angleDifference = Mathf.Abs(startAngle - currentAngle);

                if (angleDifference > angleTolerance)
                {
                    if (angleDifference > 270f) //checking to see if the user has gone from 0-360 - a very tiny movement but will trigger the angletolerance
                    {
                        float angleCheck;

                        if (startAngle < currentAngle) //going anticlockwise
                        {
                            angleCheck = CheckAngle(currentAngle, startAngle);

                            if (angleCheck < angleTolerance)
                            {
                                return;
                            }
                            else
                            {
                                RotateDialAntiClockwise();
                                startAngle = currentAngle;
                            }
                        }
                        else if (startAngle > currentAngle) //going clockwise;
                        {
                            angleCheck = CheckAngle(currentAngle, startAngle);

                            if (angleCheck < angleTolerance)
                            {
                                return;
                            }
                            else
                            {
                                RotateDialClockwise();
                                startAngle = currentAngle;
                            }
                        }
                    }
                    else
                    {
                        if (startAngle < currentAngle)//clockwise
                        {
                            RotateDialClockwise();
                            startAngle = currentAngle;
                        }
                        else if (startAngle > currentAngle)
                        {
                            RotateDialAntiClockwise();
                            startAngle = currentAngle;
                        }
                    }
                }
            }
            else
            {
                requiresStartAngle = false;
                startAngle = currentAngle;
            }
        }

        private float CheckAngle(float currentAngle, float startAngle)
        {
            var checkAngleTravelled = (360f - currentAngle) + startAngle;
            return (checkAngleTravelled);
        }

        private void RotateDialClockwise()
        {
            linkedDialVisual.localEulerAngles = new Vector3(linkedDialVisual.localEulerAngles.x, linkedDialVisual.localEulerAngles.y - snapRotationAmout, linkedDialVisual.localEulerAngles.z);
            GetComponent<IDial>().DialChanged(linkedDialVisual.localEulerAngles.y);
        }

        private void RotateDialAntiClockwise()
        {
            linkedDialVisual.localEulerAngles = new Vector3(linkedDialVisual.localEulerAngles.x, linkedDialVisual.localEulerAngles.y + snapRotationAmout, linkedDialVisual.localEulerAngles.z);
            GetComponent<IDial>().DialChanged(linkedDialVisual.localEulerAngles.y);
        }

        //void OnDrawGizmosSelected()
        //{
        //    float totalFOV = 70.0f;
        //    float rayRange = 10.0f;
        //    float halfFOV = totalFOV / 2.0f;
        //    Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        //    Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        //    Vector3 leftRayDirection = leftRayRotation * transform.forward;
        //    Vector3 rightRayDirection = rightRayRotation * transform.forward;
        //    Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        //    Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
        //}
    }

    
}
