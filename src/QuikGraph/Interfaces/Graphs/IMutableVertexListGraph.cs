#if SUPPORTS_CONTRACTS
using System.Diagnostics.Contracts;
using QuikGraph.Contracts;
#endif

namespace QuikGraph
{
    /// <summary>
    /// A mutable vertex list graph with vertices of type <typeparamref name="TVertex"/>
    /// and edges of type <typeparamref name="TEdge"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
#if SUPPORTS_CONTRACTS
    [ContractClass(typeof(MutableVertexListGraphContract<,>))]
#endif
    public interface IMutableVertexListGraph<TVertex, TEdge>
        : IMutableIncidenceGraph<TVertex, TEdge>
        , IMutableVertexSet<TVertex>
         where TEdge : IEdge<TVertex>
    {
    }
}
