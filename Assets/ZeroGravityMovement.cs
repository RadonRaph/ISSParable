using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ZeroGravityMovement : MonoBehaviour
{
    public LayerMask blockingLayer;

    public LayerMask gripLayer;
    public LayerMask gripLayerRight;
    public LayerMask gripLayerLeft;

    public float maxGrabDistance = 5;


    public Transform RightHandRigController;

    public Transform Rig;

    Transform _rightHandGhost;
    public Vector3 RightHandGhost
    {
        get
        {
            if (_rightHandGhost)
                return _rightHandGhost.position;
            else
                return Vector3.zero;
        }

        set
        {
            RaycastHit hit;
            Vector3 target = _rightHandGhost.position + value;

            Debug.DrawLine(RightHandRigController.position, target, Color.green);

            if (Vector3.Distance(RightHandRigController.position, target) > maxGrabDistance)
            {
                Debug.DrawLine(RightHandRigController.position, target, Color.red);
                target = _rightHandGhost.position;

            }


            Physics.Linecast(RightHandRigController.position, target, out hit, blockingLayer);


            Collider[] walls =  Physics.OverlapSphere(target, value.magnitude, blockingLayer);
            if (hit.transform || walls.Length > 0)
            {
                Debug.DrawLine(RightHandRigController.position, target, Color.red);
                Inertie -= 0.5f*Inertie;

                if (hit.transform)
                    Debug.DrawLine(RightHandRigController.position, hit.point, Color.green);
            }
            else
            {
                Collider[] grips = Physics.OverlapSphere(target, 0.5f, gripLayerRight);
                if (grips.Length > 0)
                {
                    target = target.Lerp(grips[0].transform.position, 0.5f);
                }



                _rightHandGhost.position = target;
            }


            
        }
    }


    public Transform LeftHandRigController;

    Transform _leftHandGhost;
    public Vector3 LeftHandGhost
    {
        get
        {
            if (_leftHandGhost)
                return _leftHandGhost.position;
            else
                return Vector3.zero;
        }

        set
        {
            RaycastHit hit;
            Vector3 target = _leftHandGhost.position + value;

            Debug.DrawLine(LeftHandRigController.position, target, Color.green);

            if (Vector3.Distance(LeftHandRigController.position, target) > maxGrabDistance)
            {
                Debug.DrawLine(LeftHandRigController.position, target, Color.red);
                target = _leftHandGhost.position;

            }


            Physics.Linecast(LeftHandRigController.position, target, out hit, blockingLayer);


            Collider[] walls = Physics.OverlapSphere(target, value.magnitude, blockingLayer);
            if (hit.transform || walls.Length > 0)
            {
                Debug.DrawLine(LeftHandRigController.position, target, Color.red);
                Inertie -= 0.5f * Inertie;

                if (hit.transform)
                    Debug.DrawLine(LeftHandRigController.position, hit.point, Color.green);
            }
            else
            {
                Collider[] grips = Physics.OverlapSphere(target, 0.5f, gripLayerLeft);
                if (grips.Length > 0)
                {
                    target = target.Lerp(grips[0].transform.position, 0.5f);
                }



                _leftHandGhost.position = target;
            }



        }
    }


    public Vector3 AllCorpsePositon
    {
        get
        {
            return Rig.position;
        }
        set
        {
            RightHandRigController.position += value;
            LeftHandRigController.position += value;
            RightFootRig.position += value;
            LeftFootRig.position += value;

            InitPos();
        }
    }




    public Transform RightFootRig;
    public Transform LeftFootRig;

    Vector3 RightFootOffset;
    Vector3 LeftFootOffset;


    public void OnEnable()
    {
        _rightHandGhost = new GameObject("Ghost_RightHand").transform;


        _leftHandGhost = new GameObject("Ghost_RightHand").transform;


        RightFootOffset = RightFootRig.position - Rig.position;
        LeftFootOffset = LeftFootRig.position - Rig.position;

        InitPos();
    }

    void InitPos()
    {
        _rightHandGhost.parent = transform;
        _rightHandGhost.position = RightHandRigController.position;
        _rightHandGhost.rotation = RightHandRigController.parent.rotation;

        _leftHandGhost.parent = transform;
        _leftHandGhost.position = LeftHandRigController.position;
        _leftHandGhost.rotation = LeftHandRigController.parent.rotation;


    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(RightHandGhost, 0.1f);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(LeftHandGhost, 0.1f);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }





    Vector3 GetInputsAxis()
    {
        float x =0;
        float y = 0;
        float z = 0;

        if (Input.GetKey(KeyCode.Z))
        {
            x += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            x -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            z += 1;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            z -= 1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            y += 1;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            y -= 1;
        }

        return new Vector3(z, y, x);

    }

    // Update is called once per frame

    public float speed = 5;

    [SerializeField]
    Vector3 Inertie;


    Transform RightHandGrip;
    Vector3 tmp = Vector3.zero;

    Transform LeftHandGrip;

    //0 rien, 1 grippé, 2 other grippé
    int RightGripStatut = 0;
    int LeftGripStatut = 0;


    bool FreeFall;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            FreeFall = true;

        if (Input.GetKeyUp(KeyCode.LeftShift))
            FreeFall = false;
    }

    void FixedUpdate()
    {

        if (RightGripStatut + LeftGripStatut > 0)
        {
            if (!FreeFall)
                Inertie += (speed * GetInputsAxis() * Time.deltaTime) - 0.2f * Inertie;
            else
                Inertie += (speed * GetInputsAxis() * Time.deltaTime);
            Inertie = Vector3.ClampMagnitude(Inertie, 1);
            if (Inertie.magnitude < 0.01f)
                Inertie = Vector3.zero;
        }
        else
        {
            AllCorpsePositon = Inertie;
        }





        if (!FreeFall)
        {



            if (RightGripStatut == 0 || RightGripStatut == 2)
            {
                RightHandGhost = Inertie - 0.1f * (RightHandGhost - RightHandRigController.position);


            }


            if (LeftGripStatut == 0 || LeftGripStatut == 2)
            {
                LeftHandGhost = Inertie - 0.1f * (LeftHandGhost - LeftHandRigController.position);

            }



            Collider[] Rgrips = Physics.OverlapSphere(RightHandGhost, 0.2f, gripLayerRight);
            if (Rgrips.Length > 0)
            {
                if (RightHandGrip)
                    RightHandGrip.gameObject.layer = 9;

                RightHandGrip = Rgrips[0].transform;
                RightHandRigController.DOMove(RightHandGrip.position, 0.3f).OnComplete(() =>
                {
                    if (LeftHandGrip.gameObject.layer == 11)
                        Rgrips[0].gameObject.layer = 12;
                    else
                        Rgrips[0].gameObject.layer = 10;



                    RightGripStatut = 1;
                    if (LeftGripStatut == 1)
                        LeftGripStatut = 2;
                });
            }



            Collider[] Lgrips = Physics.OverlapSphere(LeftHandGhost, 0.2f, gripLayerLeft);
            if (Lgrips.Length > 0)
            {
                if (LeftHandGrip)
                    LeftHandGrip.gameObject.layer = 9;

                LeftHandGrip = Lgrips[0].transform;
                LeftHandRigController.DOMove(LeftHandGrip.position, 0.3f).OnComplete(() =>
                {

                    if (LeftHandGrip.gameObject.layer == 10)
                        Lgrips[0].gameObject.layer = 12;
                    else
                        Lgrips[0].gameObject.layer = 11;

                    LeftGripStatut = 1;
                    if (RightGripStatut == 1)
                        RightGripStatut = 2;
                });



            }


        }
        else
        {
            RightGripStatut = 0;
            LeftGripStatut = 0;

            
        }


        

            



        RightFootRig.position = Rig.position + RightFootOffset;
        LeftFootRig.position = Rig.position + LeftFootOffset;

        /*
        if (LeftHandGrip)
        {
            
            if (Vector3.Distance(LeftHandRigController.position, LeftHandGrip.position) > 0.1f)
                LeftHandRigController.position = Vector3.SmoothDamp(LeftHandRigController.position, LeftHandGrip.position, ref tmp, 0.3f);
        }
            */
    }
}
