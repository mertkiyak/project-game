using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderFeature : ScriptableRendererFeature
{
    class BlurPass : ScriptableRenderPass
    {
        private Material blurMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private string profilerTag;
        private int downScale;
        private float blurSize;

        public BlurPass(Material blurMat, int downScale, float blurSize, string tag)
        {
            this.blurMaterial = blurMat;
            this.downScale = downScale;
            this.blurSize = blurSize;
            profilerTag = tag;
            tempTexture.Init("_TempBlurTex");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.width /= downScale;
            descriptor.height /= downScale;
            descriptor.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, descriptor, FilterMode.Bilinear);

            blurMaterial.SetFloat("_BlurSize", blurSize);

            // Blur iþlemi
            cmd.Blit(source, tempTexture.Identifier(), blurMaterial);
            cmd.Blit(tempTexture.Identifier(), source); // Geri yaz

            // Shader'a global olarak blurTexture'ý gönder
            cmd.SetGlobalTexture("_BlurTex", tempTexture.Identifier());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    public Material blurMaterial;
    [Range(1, 4)] public int downScale = 2;
    [Range(0.1f, 5f)] public float blurSize = 1f;
    public string profilerTag = "URP Blur Pass";

    private BlurPass blurPass;

    public override void Create()
    {
        blurPass = new BlurPass(blurMaterial, downScale, blurSize, profilerTag)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        blurPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(blurPass);
    }
}
