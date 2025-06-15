using Microsoft.Extensions.VectorData;

namespace AiGenericEmbedding;

public class AiRecommendations<TKey, TEntity> : AiBase<TKey, TEntity>
    where TEntity : class, IVectorEntity<TKey>, new() where TKey : notnull
{
    public AiRecommendations(List<TEntity> items, Uri uri, string model)
        : base(items, uri, model) { }

    public async Task<List<TEntity>> GetSimilarItemsAsync(TEntity target, uint topN)
    {
        await GenerateEmbeddingsAsync();

        dynamic dyn = target;
        string combinedText = dyn.CombinedData;

        var genResult = await Generator.GenerateAsync(new[] { combinedText });
        var targetVector = genResult[0].Vector;

        var results = Store.SearchAsync(
            targetVector,
            top: (int)topN,
            options: new VectorSearchOptions<TEntity>
            {
                VectorProperty = x => x.Vector
            }
        );

        var list = new List<TEntity>();
        await foreach (var r in results)
        {
            if (!r.Record.Id.Equals(target.Id))
            {
                list.Add(r.Record);
            }
        }

        return list;
    }
}