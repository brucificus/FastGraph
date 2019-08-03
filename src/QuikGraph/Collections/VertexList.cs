#if SUPPORTS_SERIALIZATION || SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
using JetBrains.Annotations;

namespace QuikGraph.Collections
{
    /// <summary>
    /// Stores a list of vertices.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class VertexList<TVertex> : List<TVertex>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexList{TVertex}"/> class.
        /// </summary>
        public VertexList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexList{TVertex}"/> class.
        /// </summary>
        /// <param name="capacity">List capacity.</param>
        public VertexList(int capacity)
            : base(capacity)
        {
        }

        /// <inheritdoc />
        public VertexList([NotNull] VertexList<TVertex> other)
            : base(other)
        {
        }

        /// <summary>
        /// Clones this vertex list.
        /// </summary>
        /// <returns>Cloned list.</returns>
        [NotNull]
        public VertexList<TVertex> Clone()
        {
            return new VertexList<TVertex>(this);
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif
    }
}
