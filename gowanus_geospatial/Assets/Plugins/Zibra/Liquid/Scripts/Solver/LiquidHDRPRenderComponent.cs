#if UNITY_PIPELINE_HDRP

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using com.zibra.liquid.Solver;

namespace com.zibra.liquid
{
    internal class LiquidHDRPRenderComponent : CustomPassVolume
    {
        internal class FluidHDRPRender : CustomPass
        {
            public ZibraLiquid liquid;
            RTHandle Depth;

            protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
            {
                Depth = RTHandles.Alloc(Vector2.one, TextureXR.slices, dimension: TextureXR.dimension,
                                        colorFormat: GraphicsFormat.R32_SFloat,
                                        // We don't need alpha for this effect
                                        useDynamicScale: true, name: "Depth buffer");
            }

            protected override void Execute(CustomPassContext ctx)
            {
                if (liquid && liquid.IsRenderingEnabled())
                {
                    RTHandle cameraColor = ctx.cameraColorBuffer;
                    RTHandle cameraDepth = ctx.cameraDepthBuffer;

                    HDCamera hdCamera = ctx.hdCamera;
                    CommandBuffer cmd = ctx.cmd;

                    if ((hdCamera.camera.cullingMask & (1 << liquid.gameObject.layer)) ==
                        0) // fluid gameobject layer is not in the culling mask of the camera
                        return;

                    float scale = (float)(hdCamera.actualWidth) / hdCamera.camera.pixelWidth;

                    liquid.RenderCallBack(hdCamera.camera, scale);

                    var depth = Shader.PropertyToID("_CameraDepthTexture");
                    cmd.GetTemporaryRT(depth, hdCamera.actualWidth, hdCamera.actualHeight, 32, FilterMode.Point,
                                       RenderTextureFormat.RFloat);

                    // copy screen to background
                    if (liquid.IsBackgroundCopyNeeded(hdCamera.camera))
                    {
                        Vector2 colorBlitScale = new Vector2(scale / cameraColor.rt.width * hdCamera.actualWidth,
                                                             scale / cameraColor.rt.height * hdCamera.actualHeight);
                        cmd.Blit(cameraColor, liquid.CameraResourcesMap[hdCamera.camera].Background, colorBlitScale,
                                 Vector2.zero, 0, 0);
                    }
                    // blit depth to temp RT
                    HDUtils.BlitCameraTexture(cmd, cameraDepth, Depth);
                    Vector2 depthBlitScale = new Vector2(scale / cameraDepth.rt.width * hdCamera.actualWidth,
                                                         scale / cameraDepth.rt.height * hdCamera.actualHeight);
                    cmd.Blit(Depth, depth, depthBlitScale, Vector2.zero, 1, 0);

                    Rect viewport = new Rect(0, 0, hdCamera.actualWidth, hdCamera.actualHeight);

                    if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan)
                    {
                        cmd.SetRenderTarget(liquid.Color0, liquid.Depth);
                        cmd.ClearRenderTarget(true, true, Color.clear);
                    }

                    // bind temp depth RT
                    cmd.SetGlobalTexture("_CameraDepthTexture", depth);
                    liquid.RenderLiquidNative(cmd, hdCamera.camera, viewport);

                    liquid.RenderFluid(cmd, hdCamera.camera, cameraColor, cameraDepth, viewport);
                    cmd.ReleaseTemporaryRT(depth);
                }
            }

            protected override void Cleanup()
            {
                RTHandles.Release(Depth);
            }
        }

        public FluidHDRPRender fluidPass;
    }
}

#endif // UNITY_PIPELINE_HDRP