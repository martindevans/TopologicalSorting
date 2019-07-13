using System.Collections.Generic;
using System.Linq;


namespace TopoSort {

    /// <summary>
    /// A process that requires execution, a process depends upon other processes being executed first, and the resources it uses not being consumed at the same time
    /// </summary>
    public class OrderedProcess<T> {

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedProcess{T}"/> class.
        /// </summary>
        /// <param value="graph">The graph which this process is part of</param>
        /// <param value="value">The value of this process</param>
        public OrderedProcess(DependencyGraph<T> graph, T value) {
            Graph = graph;
            Value = value;

            Graph.Add(this);
        }

        #endregion

        #region resource constraints

        /// <summary>
        /// Indicates that this process requires the specified resource.
        /// </summary>
        /// <param value="resource">The resource.</param>
        /// <returns>returns this process</returns>
        public void Requires(Resource<T> resource) {
            DependencyGraph<T>.CheckGraph(resource, this);

            if (_resources.Add(resource)) {
                resource.UsedBy(this);
            }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return "Process { " + Value + " }";
        }

        #region fields

        /// <summary>
        /// The value of this process
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// The graph this process is part of
        /// </summary>
        public readonly DependencyGraph<T> Graph;

        private readonly HashSet<OrderedProcess<T>> _predecessors = new HashSet<OrderedProcess<T>>();

        /// <summary>
        /// Gets the predecessors of this process
        /// </summary>
        /// <value>The predecessors.</value>
        public IEnumerable<OrderedProcess<T>> Predecessors => _predecessors;

        private readonly HashSet<OrderedProcess<T>> _followers = new HashSet<OrderedProcess<T>>();

        /// <summary>
        /// Gets the followers of this process
        /// </summary>
        public IEnumerable<OrderedProcess<T>> Followers => _followers;

        private readonly HashSet<Resource<T>> _resources = new HashSet<Resource<T>>();

        /// <summary>
        /// Gets the resources this process depends upon
        /// </summary>
        /// <value>The resources.</value>
        public IEnumerable<Resource<T>> Resources => _resources;

        internal ISet<Resource<T>> ResourcesSet => _resources;

        #endregion

        #region ordering constraints

        /// <summary>
        /// Indicates that this process should execute before another
        /// </summary>
        /// <param value="follower">The ancestor.</param>
        /// <returns>returns this process</returns>
        public OrderedProcess<T> Before(OrderedProcess<T> follower) {
            DependencyGraph<T>.CheckGraph(this, follower);

            if (_followers.Add(follower)) {
                follower.After(this);
            }

            return follower;
        }

        /// <summary>
        /// Indicates that this process must happen before all the followers
        /// </summary>
        /// <param value="followers">The followers.</param>
        /// <returns>the followers</returns>
        public IEnumerable<OrderedProcess<T>> Before(params OrderedProcess<T>[] followers) {
            return Before(followers as IEnumerable<OrderedProcess<T>>);
        }

        /// <summary>
        /// Indicates that this process must happen before all the followers
        /// </summary>
        /// <param value="followers">The followers.</param>
        /// <returns>the followers</returns>
        public IEnumerable<OrderedProcess<T>> Before(IEnumerable<OrderedProcess<T>> followers) {
            var orderedProcesses = followers as OrderedProcess<T>[] ?? followers.ToArray();

            foreach (var ancestor in orderedProcesses) {
                Before(ancestor);
            }

            return orderedProcesses;
        }

        /// <summary>
        /// Indicates that this process should execute after another
        /// </summary>
        /// <param value="predecessor">The predecessor.</param>
        /// <returns>returns this process</returns>
        public OrderedProcess<T> After(OrderedProcess<T> predecessor) {
            DependencyGraph<T>.CheckGraph(this, predecessor);

            if (_predecessors.Add(predecessor)) {
                predecessor.Before(this);
            }

            return predecessor;
        }

        /// <summary>
        /// Indicates that this process must happen after all the predecessors
        /// </summary>
        /// <param value="predecessors">The predecessors.</param>
        /// <returns>the predecessors</returns>
        public IEnumerable<OrderedProcess<T>> After(params OrderedProcess<T>[] predecessors) {
            return After(predecessors as IEnumerable<OrderedProcess<T>>);
        }

        /// <summary>
        /// Indicates that this process must happen after all the predecessors
        /// </summary>
        /// <param value="predecessors">The predecessors.</param>
        /// <returns>the predecessors</returns>
        public IEnumerable<OrderedProcess<T>> After(IEnumerable<OrderedProcess<T>> predecessors) {
            var orderedProcesses = predecessors as OrderedProcess<T>[] ?? predecessors.ToArray();

            foreach (var predecessor in orderedProcesses) {
                After(predecessor);
            }

            return orderedProcesses;
        }

        #endregion

    }

    public class OrderedProcessComprarer<T> : IEqualityComparer<OrderedProcess<T>> {

        public OrderedProcessComprarer(IEqualityComparer<T> comparer) {
            Comparer = comparer;
        }

        public IEqualityComparer<T> Comparer { get; }

        public bool Equals(OrderedProcess<T> x, OrderedProcess<T> y) {
            if (x == null && y == null) {
                return true;
            }

            if (x == null || y == null) {
                return false;
            }

            return Comparer.Equals(x.Value, y.Value);
        }

        public int GetHashCode(OrderedProcess<T> obj) {
            return obj.Value.GetHashCode();
        }

    }

}