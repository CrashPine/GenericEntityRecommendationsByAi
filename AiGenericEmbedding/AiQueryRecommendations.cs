using Microsoft.Extensions.VectorData;

namespace AiGenericEmbedding;

public class AiQueryRecommendations<TKey, TEntity> : AiBase<TKey, TEntity>
    where TEntity : class, IVectorEntity<TKey>, new() where TKey : notnull
{
    public AiQueryRecommendations(List<TEntity> items, Uri uri, string model)
        : base(items, uri, model) { }

    public async Task<List<TEntity>> GetByQueryAsync(string query, uint topN)
    {
        await GenerateEmbeddingsAsync();

        var embedding = (await Generator.GenerateAsync(new[] { query }))[0].Vector;

        var results = Store.SearchAsync(
            embedding,
            top: (int)topN,
            options: new VectorSearchOptions<TEntity>
            {
                VectorProperty = x => x.Vector
            }
        );

        var list = new List<TEntity>();
        await foreach (var r in results)
            list.Add(r.Record);
        return list;
    }
}