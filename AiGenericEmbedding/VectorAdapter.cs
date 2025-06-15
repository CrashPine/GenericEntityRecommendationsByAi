using Microsoft.Extensions.VectorData;

namespace AiGenericEmbedding;

public class VectorAdapter<TKey, TEntity> : IVectorEntity<TKey>
{
    private readonly TEntity _entity = default!;
    private readonly Func<TEntity, TKey> _keySelector = null!;
    private readonly Func<TEntity, string>[] _textSelectors = null!;

    public VectorAdapter() { } 
    
    public VectorAdapter(
        TEntity entity,
        Func<TEntity, TKey> keySelector,
        params Func<TEntity, string>[] textSelectors)
    {
        _entity = entity;
        _keySelector = keySelector;
        _textSelectors = textSelectors;
    }

    [VectorStoreKey]
    public TKey Id => _keySelector(_entity);

    [VectorStoreData]
    public string CombinedData => string.Join(" ", _textSelectors.Select(sel => sel(_entity)));
    
   
    [VectorStoreVector(
        Dimensions: 4096,
        DistanceFunction = DistanceFunction.CosineSimilarity,
        IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> Vector { get; set; }

    public TEntity Entity => _entity;
}