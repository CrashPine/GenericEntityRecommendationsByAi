using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace AiGenericEmbedding;

public abstract class AiBase<TKey, TEntity>
    where TEntity : class, IVectorEntity<TKey>, new() where TKey : notnull
{
    protected readonly List<TEntity> ItemsList;
    protected readonly VectorStoreCollection<TKey, TEntity> Store;
    protected readonly IEmbeddingGenerator<string, Embedding<float>> Generator;

    protected AiBase(List<TEntity> itemsList, Uri uri, string model)
    {
        ItemsList = itemsList;
        Generator = new OllamaEmbeddingGenerator(uri, model);

        var options = new InMemoryVectorStoreOptions
        {
            EmbeddingGenerator = Generator
        };
        var store = new InMemoryVectorStore(options);
        Store = store.GetCollection<TKey, TEntity>(typeof(TEntity).Name.ToLowerInvariant());
    }

    public async Task GenerateEmbeddingsAsync()
    {
        await Store.EnsureCollectionExistsAsync();

        var texts = ItemsList
            .Select(item =>
            {
                dynamic dyn = item;
                return (string)dyn.CombinedData;
            })
            .ToArray();

        var results = await Generator.GenerateAsync(texts);

        for (int i = 0; i < ItemsList.Count; i++)
        {
            ItemsList[i].Vector = results[i].Vector;
        }

        await Store.UpsertAsync(ItemsList);
    }
}