//originally made by MediaMax, ported by DJ::Ötzi

using UnityEngine;

[RequireComponent(typeof(Wheel))]
public class UnityCar3DTireSystem : MonoBehaviour
{
    [Space(15)]
    public bool use3DTire = true;
    [Range(2, 360)]
    public int raysNumber = 36;
    [Range(0f, 360f)]
    public float raysMaxAngle = 180f;
    [HideInInspector]
    public float wheelWidth = .25f;
    [HideInInspector]
    public Transform wheelModel;

    private Wheel _wheelCollider;
    private Drivetrain carController;
    private float orgRadius;

    void Awake()
    {
        _wheelCollider = GetComponent<Wheel>();
        carController = GetComponentInParent<Drivetrain>();
        orgRadius = _wheelCollider.radius;
        wheelWidth = _wheelCollider.width;
        wheelModel = _wheelCollider.model.transform;
    }
    
    void Update()
    {
        if (use3DTire)
        {
            if (!wheelModel)
                return;

            float radiusOffset = 0f;

            for (int i = 0; i <= raysNumber; i++)
            {
                Vector3 rayDirection = Quaternion.AngleAxis(_wheelCollider.deltaSteering*_wheelCollider.maxSteeringAngle, transform.up) * Quaternion.AngleAxis(i * (raysMaxAngle / raysNumber) + ((180f - raysMaxAngle) / 2), transform.right) * transform.forward;

                if (Physics.Raycast(wheelModel.position, rayDirection, out RaycastHit hit, _wheelCollider.radius))
                {
                    if (!hit.transform.IsChildOf(carController.transform) && !hit.collider.isTrigger)
                    {
                        Debug.DrawLine(wheelModel.position, hit.point, Color.red);

                        radiusOffset = Mathf.Max(radiusOffset, _wheelCollider.radius - hit.distance);
                    }
                }

                Debug.DrawRay(wheelModel.position, rayDirection * orgRadius, Color.green);
                
                if (Physics.Raycast(wheelModel.position + wheelModel.right * wheelWidth * .5f, rayDirection, out RaycastHit rightHit, _wheelCollider.radius))
                {
                    if (!rightHit.transform.IsChildOf(carController.transform) && !rightHit.collider.isTrigger)
                    {
                        Debug.DrawLine(wheelModel.position + wheelModel.right * wheelWidth * .5f, rightHit.point, Color.red);

                        radiusOffset = Mathf.Max(radiusOffset, _wheelCollider.radius - rightHit.distance);
                    }
                }

                Debug.DrawRay(wheelModel.position + wheelModel.right * wheelWidth * .5f, rayDirection * orgRadius, Color.green);
                
                if (Physics.Raycast(wheelModel.position - wheelModel.right * wheelWidth * .5f, rayDirection, out RaycastHit leftHit, _wheelCollider.radius))
                {
                    if (!leftHit.transform.IsChildOf(carController.transform) && !leftHit.collider.isTrigger)
                    {
                        Debug.DrawLine(wheelModel.position - wheelModel.right * wheelWidth * .5f, leftHit.point, Color.red);

                        radiusOffset = Mathf.Max(radiusOffset, _wheelCollider.radius - leftHit.distance);
                    }
                }
                
                Debug.DrawRay(wheelModel.position - wheelModel.right * wheelWidth * .5f, rayDirection * orgRadius, Color.green);
            }

            _wheelCollider.radius = Mathf.LerpUnclamped(_wheelCollider.radius, orgRadius + radiusOffset, Time.deltaTime * 10f);
        }
        else
        {
            _wheelCollider.radius = Mathf.LerpUnclamped(_wheelCollider.radius, orgRadius, Time.deltaTime * 10f);
        }
    }
}
