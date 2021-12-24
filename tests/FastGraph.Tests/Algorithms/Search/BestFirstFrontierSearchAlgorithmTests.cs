#nullable enable

using NUnit.Framework;
using FastGraph.Algorithms;
using FastGraph.Algorithms.Observers;
using FastGraph.Algorithms.Search;
using FastGraph.Algorithms.ShortestPath;
using static FastGraph.Tests.Algorithms.AlgorithmTestHelpers;

namespace FastGraph.Tests.Algorithms.Search
{
    /// <summary>
    /// Tests for <see cref="BestFirstFrontierSearchAlgorithm{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal sealed class BestFirstFrontierSearchAlgorithmTests : SearchAlgorithmTestsBase
    {
        #region Test helpers

        private static void RunAndCheckSearch<TVertex, TEdge>(
            IBidirectionalGraph<TVertex, TEdge> graph)
            where TVertex : notnull
            where TEdge : IEdge<TVertex>
        {
            if (graph.VertexCount == 0)
                return;

            IDistanceRelaxer distanceRelaxer = DistanceRelaxers.ShortestDistance;

            var search = new BestFirstFrontierSearchAlgorithm<TVertex, TEdge>(
                graph,
                _ => 1.0,
                distanceRelaxer);
            bool targetReached = false;
            search.TargetReached += (_, _) => targetReached = true;

            TVertex root = graph.Vertices.First();
            TVertex target = graph.Vertices.Last();

            var recorder = new VertexPredecessorRecorderObserver<TVertex, TEdge>();
            using (recorder.Attach(search))
                search.Compute(root, target);

            if (recorder.VerticesPredecessors.ContainsKey(target))
            {
                recorder.TryGetPath(target, out IEnumerable<TEdge>? path).Should().BeTrue();

                if (Equals(root, path!.First().Source))
                    targetReached.Should().BeTrue();
                else
                    targetReached.Should().BeFalse();
            }
        }

        private static void CompareSearches<TVertex, TEdge>(
            IBidirectionalGraph<TVertex, TEdge> graph,
            TVertex root,
            TVertex target)
            where TVertex : notnull
            where TEdge : IEdge<TVertex>
        {
            double EdgeWeights(TEdge edge) => 1.0;

            IDistanceRelaxer distanceRelaxer = DistanceRelaxers.ShortestDistance;

            var search = new BestFirstFrontierSearchAlgorithm<TVertex, TEdge>(
                graph,
                EdgeWeights,
                distanceRelaxer);
            var recorder = new VertexDistanceRecorderObserver<TVertex, TEdge>(EdgeWeights);
            using (recorder.Attach(search))
                search.Compute(root, target);

            var dijkstra = new DijkstraShortestPathAlgorithm<TVertex, TEdge>(graph, EdgeWeights, distanceRelaxer);
            var dijkstraRecorder = new VertexDistanceRecorderObserver<TVertex, TEdge>(EdgeWeights);
            using (dijkstraRecorder.Attach(dijkstra))
                dijkstra.Compute(root);

            IDictionary<TVertex, double> bffsVerticesDistances = recorder.Distances;
            IDictionary<TVertex, double> dijkstraVerticesDistances = dijkstraRecorder.Distances;
            if (dijkstraVerticesDistances.TryGetValue(target, out double cost))
            {
                bffsVerticesDistances.ContainsKey(target).Should().BeTrue();
                bffsVerticesDistances[target].Should().Be(dijkstraVerticesDistances[target]);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.ShortestDistance);
            AssertAlgorithmState(algorithm, graph);

            algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(default, graph, _ => 1.0, DistanceRelaxers.ShortestDistance);
            AssertAlgorithmState(algorithm, graph);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            var graph = new BidirectionalGraph<int, Edge<int>>();

#pragma warning disable CS8625
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, _ => 1.0, DistanceRelaxers.ShortestDistance)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                graph, default, DistanceRelaxers.ShortestDistance)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                graph, _ => 1.0, default)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, default, DistanceRelaxers.ShortestDistance)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                graph, default, default)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, default, default)).Should().Throw<ArgumentNullException>();

            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, default, _ => 1.0, DistanceRelaxers.ShortestDistance)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, graph, default, DistanceRelaxers.ShortestDistance)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, graph, _ => 1.0, default)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, default, default, DistanceRelaxers.ShortestDistance)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, graph, default, default)).Should().Throw<ArgumentNullException>();
            Invoking(() => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                default, default, default, default)).Should().Throw<ArgumentNullException>();
#pragma warning restore CS8625
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        #region Rooted algorithm

        [Test]
        public void TryGetRootVertex()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            TryGetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            SetRootVertex_Test(algorithm);
        }

        [Test]
        public void SetRootVertex_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<TestVertex, Edge<TestVertex>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            SetRootVertex_Throws_Test(algorithm);
        }

        [Test]
        public void ClearRootVertex()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            ClearRootVertex_Test(algorithm);
        }

        [Test]
        public void ComputeWithoutRoot_Throws()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            ComputeWithoutRoot_Throws_Test(
                graph,
                () => new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance));
        }

        #endregion

        #region Search algorithm

        [Test]
        public void TryGetTargetVertex()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            TryGetTargetVertex_Test(algorithm);
        }

        [Test]
        public void SetTargetVertex()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            SetTargetVertex_Test(algorithm);
        }

        [Test]
        public void SetTargetVertex_Throws()
        {
            var graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<TestVertex, Edge<TestVertex>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            SetTargetVertex_Throws_Test(algorithm);
        }

        [Test]
        public void ClearTargetVertex()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            ClearTargetVertex_Test(algorithm);
        }

        [Test]
        public void ComputeWithRootAndTarget()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            graph.AddVertexRange(new[] { 0, 1 });
            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            ComputeWithRootAndTarget_Test(algorithm);
        }

        [Test]
        public void ComputeWithRootAndTarget_Throws()
        {
            var graph1 = new BidirectionalGraph<int, Edge<int>>();
            var algorithm1 = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(graph1, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            ComputeWithRootAndTarget_Throws_Test(graph1, algorithm1);

            var graph2 = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();
            var algorithm2 = new BestFirstFrontierSearchAlgorithm<TestVertex, Edge<TestVertex>>(graph2, _ => 1.0, DistanceRelaxers.EdgeShortestDistance);
            ComputeWithRootAndTarget_Throws_Test(algorithm2);
        }

        #endregion

        [Test]
        public void SameStartAndEnd()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            graph.AddVerticesAndEdge(new Edge<int>(1, 3));
            graph.AddVerticesAndEdge(new Edge<int>(1, 2));
            graph.AddVerticesAndEdge(new Edge<int>(2, 5));
            graph.AddVerticesAndEdge(new Edge<int>(2, 4));
            graph.AddVerticesAndEdge(new Edge<int>(5, 6));
            graph.AddVerticesAndEdge(new Edge<int>(5, 7));

            var algorithm = new BestFirstFrontierSearchAlgorithm<int, Edge<int>>(
                graph, _ => 1.0, DistanceRelaxers.ShortestDistance);
            bool targetReached = false;
            algorithm.TargetReached += (_, _) => targetReached = true;

            algorithm.Compute(1, 1);
            targetReached.Should().BeTrue();
        }

        [Test]
        public void SimpleGraph()
        {
            var graph = new BidirectionalGraph<char, SEquatableEdge<char>>();
            graph.AddVerticesAndEdge(new SEquatableEdge<char>('A', 'C'));
            graph.AddVerticesAndEdge(new SEquatableEdge<char>('A', 'B'));
            graph.AddVerticesAndEdge(new SEquatableEdge<char>('B', 'E'));
            graph.AddVerticesAndEdge(new SEquatableEdge<char>('B', 'D'));
            graph.AddVerticesAndEdge(new SEquatableEdge<char>('E', 'F'));
            graph.AddVerticesAndEdge(new SEquatableEdge<char>('E', 'G'));

            RunAndCheckSearch(graph);
        }

        [TestCaseSource(nameof(BidirectionalGraphs_SlowTests))]
        [Category(TestCategories.LongRunning)]
        public void BestFirstFrontierSearch(TestGraphInstance<BidirectionalGraph<string, Edge<string>>, string> testGraph)
        {
            RunAndCheckSearch(testGraph.Instance);
        }

        [TestCaseSource(nameof(BidirectionalGraphs_SlowTests_RootedVertices))]
        [Category(TestCategories.LongRunning)]
        public void BestFirstFrontierComparedToDijkstraSearch(TestGraphInstanceWithSelectedVertexPair<BidirectionalGraph<string, Edge<string>>, string> testGraph)
        {
            var graph = testGraph.Instance;
            var root = testGraph.SelectedVertex0;
            var vertex = testGraph.SelectedVertex1;
            CompareSearches(graph, root, vertex);
        }

        private static readonly IEnumerable<TestCaseData> BidirectionalGraphs_SlowTests =
            TestGraphFactory
                .SampleBidirectionalGraphs()
                .Select(t => new TestCaseData(t) { TestName = t.DescribeForTestCase() })
                .Memoize();

        private static readonly IEnumerable<TestCaseData> BidirectionalGraphs_SlowTests_RootedVertices =
            TestGraphFactory
                .SampleBidirectionalGraphs()
                .Where(t => t.VertexCount > 1)
                .Select(t => t.Select().First())
                .SelectMany(t => t.Select().Where(t => t.SelectedVertex0 != t.SelectedVertex1))
                .Select(t => new TestCaseData(t) { TestName = t.DescribeForTestCase() })
                .Memoize();
    }
}
