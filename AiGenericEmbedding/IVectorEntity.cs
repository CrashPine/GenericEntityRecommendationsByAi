
using Microsoft.Extensions.VectorData;

namespace AiGenericEmbedding;

public interface IVectorEntity<TKey>
{
    [VectorStoreKey]
    TKey Id { get; }

    [VectorStoreVector(
        Dimensions: 4096,
        DistanceFunction = DistanceFunction.CosineSimilarity,
        IndexKind = IndexKind.Hnsw)]
    ReadOnlyMemory<float> Vector { get; set; }
}