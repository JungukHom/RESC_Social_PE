using UnityEngine;

namespace SimplestarGame.Water
{
    [RequireComponent(typeof(MeshRenderer))]
    public class WaterRefractionChanger : MonoBehaviour
    {
        [SerializeField] Transform mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            this.waterMaterial = this.GetComponent<Renderer>().material;
            mainCamera = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            // If in the water, flip water surface normal vector and change IOR.
            this.waterMaterial.SetFloat("_AboveTheWater", (this.transform.position.y < this.mainCamera.position.y) ? 1 : -1);
        }

        /// <summary>
        /// Water Shader Material
        /// </summary>
        Material waterMaterial;
    }
}
