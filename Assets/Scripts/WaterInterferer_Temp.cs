using SimplestarGame.WaterParticle;
using UnityEngine;
// using Valve.VR;

namespace SimplestarGame.Wave
{
    /// <summary>
    /// If collision object has WaveSimulator, Add wave at the ClosestPoint with its velocity.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class WaterInterferer_Temp : MonoBehaviour
    {
        [SerializeField] PeriodicEffector waterDrop;
        [SerializeField] PeriodicEffector waterSplash;

        Vector3 defaultPosition;

        //public void SetPosition(SteamVR_Behaviour_Pose pose, SteamVR_Input_Sources sources)
        //{
        //    this.transform.position = pose.transform.position;
        //    this.transform.rotation = Quaternion.identity;
        //}

        Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            //this.lastPoint = transform.position;

            defaultPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (transform.position.y <= -10)
            {
                transform.position = defaultPosition;
                rigidbody.velocity = Vector3.zero;
            }

            // For reading the delta time it is recommended to use Time.deltaTime instead.
            // Because it automatically returns the right delta time if you are inside a FixedUpdate function or Update function.
            //this.velocity = (this.transform.position - this.lastPoint) / Time.deltaTime;
            //this.lastPoint = transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            var waveComputer = other.gameObject.GetComponent<WaveSimulator>();
            if (null == waveComputer)
            {
                return;
            }
            waveComputer.AddWave(other.ClosestPoint(transform.position), rigidbody.velocity);

            if (null != this.waterSplash)
            {
                this.waterSplash.transform.position = new Vector3(transform.position.x, other.transform.position.y + 0.051f, transform.position.z);
                this.waterSplash.StartPowerEffect(0.2f, rigidbody.velocity);
            }

            Invoke("Temp", Random.Range(0.1f, 0.3f));
        }

        void Temp()
        {
            this.transform.position = defaultPosition;
            rigidbody.velocity = Vector3.zero;
        }

        void OnTriggerExit(Collider other)
        {
            var waveComputer = other.gameObject.GetComponent<WaveSimulator>();
            if (null == waveComputer)
            {
                return;
            }
            waveComputer.AddWave(other.ClosestPoint(transform.position), rigidbody.velocity);

            if (null != this.waterDrop)
            {
                this.waterDrop.StartEffect(0.2f);
            }

            Invoke("Temp", Random.Range(0.1f, 0.3f));
        }

        //Vector3 velocity;
        //Vector3 lastPoint;
    }
}
