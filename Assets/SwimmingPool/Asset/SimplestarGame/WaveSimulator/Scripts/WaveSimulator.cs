using UnityEngine;

namespace SimplestarGame.Wave
{
    /// <summary>
    /// Wave Simulator
    /// </summary>
    /// <remarks>
    /// original idea is from this qiita topic https://qiita.com/aa_debdeb/items/1d69d49333630b06f6ce
    /// </remarks>
    public class WaveSimulator : MonoBehaviour
    {
        [SerializeField] ComputeShader computeShader;
        
        [SerializeField] [Range(0.0f, 3.0f)] float waveSpeed = 3.0f;

        /// <summary>
        /// Wave Interactive Function
        /// </summary>
        /// <param name="point">AddWavePoint</param>
        /// <param name="velocity">PointVelocity</param>
        internal void AddWave(Vector3 point, Vector3 velocity)
        {
            var localPoint = this.transform.worldToLocalMatrix.MultiplyPoint(point) * 2.0f; // x:-1.0 ~ 1.0, y:-1.0 ~ 1.0
            var waveHeight = this.transform.worldToLocalMatrix.MultiplyVector(velocity);
            this.computeShader.SetFloats("addPoint", localPoint.x, localPoint.y, Mathf.Clamp(waveHeight.z, -2, 2));
            this.computeShader.Dispatch(this.kernelAddWave, Mathf.CeilToInt(this.currWaveTexture.width / this.threadSizeAddWave.x), Mathf.CeilToInt(this.currWaveTexture.height / this.threadSizeAddWave.y), 1);
        }

        void Start()
        {
            // Get ComputeShader Kernel Index;
            this.kernelInitialize = this.computeShader.FindKernel("Initialize");
            this.kernelAddWave = this.computeShader.FindKernel("AddWave");
            this.kernelUpdate = this.computeShader.FindKernel("Update");
            this.kernelReplace = this.computeShader.FindKernel("Replace");

            // Create Float Texture x 3
            const int resolution = 512;
            this.lastWaveTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat);
            this.lastWaveTexture.wrapMode = TextureWrapMode.Clamp;
            this.lastWaveTexture.enableRandomWrite = true;
            this.lastWaveTexture.Create();

            this.currWaveTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat);
            this.currWaveTexture.wrapMode = TextureWrapMode.Clamp;
            this.currWaveTexture.enableRandomWrite = true;
            this.currWaveTexture.Create();

            this.nextWaveTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat);
            this.nextWaveTexture.wrapMode = TextureWrapMode.Clamp;
            this.nextWaveTexture.enableRandomWrite = true;
            this.nextWaveTexture.Create();

            // Get Thread Size
            uint threadSizeX, threadSizeY, threadSizeZ;
            this.computeShader.GetKernelThreadGroupSizes(this.kernelInitialize, out threadSizeX, out threadSizeY, out threadSizeZ);
            this.threadSizeInitialize = new Vector3Int((int)threadSizeX, (int)threadSizeY, (int)threadSizeZ);
            this.computeShader.GetKernelThreadGroupSizes(this.kernelAddWave, out threadSizeX, out threadSizeY, out threadSizeZ);
            this.threadSizeAddWave = new Vector3Int((int)threadSizeX, (int)threadSizeY, (int)threadSizeZ);
            this.computeShader.GetKernelThreadGroupSizes(this.kernelUpdate, out threadSizeX, out threadSizeY, out threadSizeZ);
            this.threadSizeUpdate = new Vector3Int((int)threadSizeX, (int)threadSizeY, (int)threadSizeZ);
            this.computeShader.GetKernelThreadGroupSizes(this.kernelReplace, out threadSizeX, out threadSizeY, out threadSizeZ);
            this.threadSizeReplace = new Vector3Int((int)threadSizeX, (int)threadSizeY, (int)threadSizeZ);

            // Initialize Input Textures
            this.computeShader.SetTexture(this.kernelInitialize, "lastWaveTexture", this.lastWaveTexture);
            this.computeShader.SetTexture(this.kernelInitialize, "currWaveTexture", this.currWaveTexture);
            this.computeShader.Dispatch(this.kernelInitialize, Mathf.CeilToInt(this.currWaveTexture.width / this.threadSizeInitialize.x), Mathf.CeilToInt(this.currWaveTexture.height / this.threadSizeInitialize.y), 1);

            // Set to Update Kernel
            this.computeShader.SetTexture(this.kernelUpdate, "lastWaveTexture", this.lastWaveTexture);
            this.computeShader.SetTexture(this.kernelUpdate, "currWaveTexture", this.currWaveTexture);
            this.computeShader.SetTexture(this.kernelUpdate, "nextWaveTexture", this.nextWaveTexture);

            // Set to AddWave Kernel
            this.computeShader.SetTexture(this.kernelAddWave, "currWaveTexture", this.currWaveTexture);

            // Set to Replace Kernel
            this.computeShader.SetTexture(this.kernelReplace, "lastWaveTexture", this.lastWaveTexture);
            this.computeShader.SetTexture(this.kernelReplace, "currWaveTexture", this.currWaveTexture);
            this.computeShader.SetTexture(this.kernelReplace, "nextWaveTexture", this.nextWaveTexture);

            // Set Default Wave Coef.
            this.computeShader.SetFloat("deltaSize", deltaSize);
            this.computeShader.SetFloat("waveCoef", waveCoef);

            // Set Result Texture to Material Param
            this.GetComponent<Renderer>().material.SetTexture("_WaveHeightMap", this.nextWaveTexture);
        }

        void FixedUpdate()
        {
            this.computeShader.SetFloat("deltaTime", Time.deltaTime * this.waveSpeed);
            // Compute Next Wave Heights
            this.computeShader.Dispatch(this.kernelUpdate, Mathf.CeilToInt(this.currWaveTexture.width / this.threadSizeUpdate.x), Mathf.CeilToInt(this.currWaveTexture.height / this.threadSizeUpdate.y), 1);
            this.computeShader.Dispatch(this.kernelReplace, Mathf.CeilToInt(this.currWaveTexture.width / this.threadSizeReplace.x), Mathf.CeilToInt(this.currWaveTexture.height / this.threadSizeReplace.y), 1);
        }

        const float deltaSize = 0.1f;
        const float waveCoef = 1.0f;

        /// <summary>
        /// Height Textures
        /// </summary>
        RenderTexture lastWaveTexture;
        RenderTexture currWaveTexture;
        /// <summary>
        /// Wave Height Map (float 32 R).
        /// </summary>
        public RenderTexture nextWaveTexture;

        /// <summary>
        /// ComputeShader Kernels
        /// </summary>
        int kernelInitialize;
        int kernelAddWave;
        int kernelUpdate;
        int kernelReplace;

        /// <summary>
        /// ThreadSize
        /// </summary>
        Vector3Int threadSizeInitialize;
        Vector3Int threadSizeAddWave;
        Vector3Int threadSizeUpdate;
        Vector3Int threadSizeReplace;
    }
}