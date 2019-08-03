using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph.Algorithms.Services;

namespace QuikGraph.Algorithms.MaximumFlow
{
    /// <summary>
    /// Base class for all graph augmentor algorithms.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public abstract class GraphAugmentorAlgorithmBase<TVertex, TEdge, TGraph> : AlgorithmBase<TGraph>, IDisposable
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphAugmentorAlgorithmBase{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexFactory">Vertex factory method.</param>
        /// <param name="edgeFactory">Edge factory method.</param>
        protected GraphAugmentorAlgorithmBase(
            [CanBeNull] IAlgorithmComponent host,
            [NotNull] TGraph visitedGraph,
            [NotNull] VertexFactory<TVertex> vertexFactory,
            [NotNull] EdgeFactory<TVertex, TEdge> edgeFactory)
            : base(host, visitedGraph)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);
#endif

            VertexFactory = vertexFactory;
            EdgeFactory = edgeFactory;
        }

        /// <summary>
        /// Vertex factory method.
        /// </summary>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [NotNull]
        public VertexFactory<TVertex> VertexFactory { get; }

        /// <summary>
        /// Edge factory method.
        /// </summary>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        [NotNull]
        public EdgeFactory<TVertex, TEdge> EdgeFactory { get; }

        /// <summary>
        /// Gets the flow source vertex.
        /// </summary>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        public TVertex SuperSource { get; private set; }

        /// <summary>
        /// Gets the flow sink vertex.
        /// </summary>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        public TVertex SuperSink { get; private set; }

        /// <summary>
        /// Indicates if the graph has been augmented or not.
        /// </summary>
#if SUPPORTS_CONTRACTS
        [System.Diagnostics.Contracts.Pure]
#endif
        public bool Augmented { get; private set; }

        /// <summary>
        /// Gets the collections of edges added to augment the graph.
        /// </summary>
        [NotNull, ItemNotNull]
        public ICollection<TEdge> AugmentedEdges { get; } = new List<TEdge>();

        /// <summary>
        /// Fired when the super source vertex is added.
        /// </summary>
        public event VertexAction<TVertex> SuperSourceAdded;

        private void OnSuperSourceAdded([NotNull] TVertex vertex)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(vertex != null);
#endif

            SuperSourceAdded?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when the super sink vertex is added.
        /// </summary>
        public event VertexAction<TVertex> SuperSinkAdded;

        private void OnSuperSinkAdded([NotNull] TVertex vertex)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(vertex != null);
#endif

            SuperSinkAdded?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when an edge is added.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> EdgeAdded;

        private void OnEdgeAdded([NotNull] TEdge edge)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(edge != null);
#endif

            EdgeAdded?.Invoke(edge);
        }

        #region Algorithm<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (Augmented)
                throw new InvalidOperationException("Graph already augmented.");

            SuperSource = VertexFactory();
            VisitedGraph.AddVertex(SuperSource);
            OnSuperSourceAdded(SuperSource);

            SuperSink = VertexFactory();
            VisitedGraph.AddVertex(SuperSink);
            OnSuperSinkAdded(SuperSink);

            AugmentGraph();
            Augmented = true;
        }

        #endregion

        /// <summary>
        /// Rollbacks the graph augmentation.
        /// </summary>
        public virtual void Rollback()
        {
            if (!Augmented)
                return;

            Augmented = false;
            VisitedGraph.RemoveVertex(SuperSource);
            VisitedGraph.RemoveVertex(SuperSink);
            SuperSource = default(TVertex);
            SuperSink = default(TVertex);
            AugmentedEdges.Clear();
        }

        /// <summary>
        /// Augments the graph.
        /// </summary>
        protected abstract void AugmentGraph();

        /// <summary>
        /// Creates and adds an augmented edge between <paramref name="source"/> and <paramref name="target"/>.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        protected void AddAugmentedEdge([NotNull] TVertex source, [NotNull] TVertex target)
        {
#if SUPPORTS_CONTRACTS
            Contract.Requires(source != null);
            Contract.Requires(target != null);
#endif

            TEdge edge = EdgeFactory(source, target);
            AugmentedEdges.Add(edge);
            VisitedGraph.AddEdge(edge);
            OnEdgeAdded(edge);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called when the object is disposed or finalized.
        /// </summary>
        /// <param name="disposing">True if called when disposing, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Rollback();
            }
        }

        #endregion
    }
}
