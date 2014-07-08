module GPUComputationExpression

open CudaDataStructure
open CUDARandom
open System.Runtime.InteropServices

type CUDARandomComputationExpression() = 
    let mutable n = 0
    let mutable r = Unchecked.defaultof<_>
    let mutable g = Unchecked.defaultof<RandGenerator>

    member this.Yield() = 
        r <- CUDARandom()
    
    member this.Init(ty) =
        let status, g0 = r.CreateGenerator(ty)
        g <- g0
    member this.Bind(r:CUDARandom) = 
        let status, v = r.GenerateUniform(g, n)
        if status = curandStatus.CURAND_SUCCESS then
            v
        else
            failwith "generate random failed"
    member this.Return(v:CUDAPointer) = 
        let array : float32 array = Array.zeroCreate n
        let nativePtr = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0)
        let p = System.IntPtr(nativePtr.ToPointer())
        CUDARuntime.CUDARuntime64.cudaMemcpy(
            p, 
            v.Pointer, 
            SizeT(n*Marshal.SizeOf(sizeof<float32>)),
            CUDAMemcpyKind.cudaMemcpyDeviceToHost)
        r.DestroyGenerator(g)
        array
