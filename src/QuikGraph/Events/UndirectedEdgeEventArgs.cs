#if SUPPORTS_SERIALIZATION
using System;
#endif

namespace QuikGraph
{
    /// <summary>
    /// Base class for arguments of an event related to an undirected edge.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public class UndirectedEdgeEventArgs<TVertex, TEdge> : EdgeEventArgs<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndirectedEdgeEventArgs{TVertex, TEdge}"/> class.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="reversed">Indicates if the edge should be reversed or not.</param>
        public UndirectedEdgeEventArgs(TEdge edge, bool reversed)
            : base(edge)
        {
            Reversed = reversed;
        }

        /// <summary>
        /// Indicates if the edge vertices are reversed or not.
        /// </summary>
        public bool Reversed { get; }

        /// <summary>
        /// Edge source.
        /// </summary>
        public TVertex Source => Reversed ? Edge.Target : Edge.Source;

        /// <summary>
        /// Edge target.
        /// </summary>
        public TVertex Target => Reversed ? Edge.Source : Edge.Target;
    }
}
