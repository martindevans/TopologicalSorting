using System.Collections.Generic;

namespace TopologicalSorting
{
    /// <summary>
    /// A process that requires execution, a process depends upon other processes being executed first, and the resources it uses not being consumed at the same time
    /// </summary>
    public class OrderedProcess
    {
        #region fields
        /// <summary>
        /// The name of this process
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The graph this process is part of
        /// </summary>
        public readonly DependencyGraph Graph;

        private readonly HashSet<OrderedProcess> _predecessors = new HashSet<OrderedProcess>();
        /// <summary>
        /// Gets the predecessors of this process
        /// </summary>
        /// <value>The predecessors.</value>
        public IEnumerable<OrderedProcess> Predecessors
        {
            get
            {
                return _predecessors;
            }
        }

        private readonly HashSet<OrderedProcess> _followers = new HashSet<OrderedProcess>();
        /// <summary>
        /// Gets the followers of this process
        /// </summary>
        public IEnumerable<OrderedProcess> Followers
        {
            get
            {
                return _followers;
            }
        }

        private readonly HashSet<Resource> _resources = new HashSet<Resource>();
        /// <summary>
        /// Gets the resources this process depends upon
        /// </summary>
        /// <value>The resources.</value>
        public IEnumerable<Resource> Resources
        {
            get
            {
                return _resources;
            }
        }
        internal ISet<Resource> ResourcesSet
        {
            get
            {
                return _resources;
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedProcess"/> class.
        /// </summary>
        /// <param name="graph">The graph which this process is part of</param>
        /// <param name="name">The name of this process</param>
        public OrderedProcess(DependencyGraph graph, string name)
        {
            Graph = graph;
            Name = name;

            Graph.Add(this);
        }
        #endregion

        #region ordering constraints
        /// <summary>
        /// Indicates that this process should execute before another
        /// </summary>
        /// <param name="follower">The ancestor.</param>
        /// <returns>returns this process</returns>
        public OrderedProcess Before(OrderedProcess follower)
        {
            DependencyGraph.CheckGraph(this, follower);

            if (_followers.Add(follower))
                follower.After(this);

            return follower;
        }

        /// <summary>
        /// Indicates that this process must happen before all the followers
        /// </summary>
        /// <param name="followers">The followers.</param>
        /// <returns>the followers</returns>
        public IEnumerable<OrderedProcess> Before(params OrderedProcess[] followers)
        {
            return Before(followers as IEnumerable<OrderedProcess>);
        }

        /// <summary>
        /// Indicates that this process must happen before all the followers
        /// </summary>
        /// <param name="followers">The followers.</param>
        /// <returns>the followers</returns>
        public IEnumerable<OrderedProcess> Before(IEnumerable<OrderedProcess> followers)
        {
            foreach (var ancestor in followers)
                Before(ancestor);

            return followers;
        }

        /// <summary>
        /// Indicates that this process should execute after another
        /// </summary>
        /// <param name="predecessor">The predecessor.</param>
        /// <returns>returns this process</returns>
        public OrderedProcess After(OrderedProcess predecessor)
        {
            DependencyGraph.CheckGraph(this, predecessor);

            if (_predecessors.Add(predecessor))
                predecessor.Before(this);

            return predecessor;
        }

        /// <summary>
        /// Indicates that this process must happen after all the predecessors
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        /// <returns>the predecessors</returns>
        public IEnumerable<OrderedProcess> After(params OrderedProcess[] predecessors)
        {
            return After(predecessors as IEnumerable<OrderedProcess>);
        }

        /// <summary>
        /// Indicates that this process must happen after all the predecessors
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        /// <returns>the predecessors</returns>
        public IEnumerable<OrderedProcess> After(IEnumerable<OrderedProcess> predecessors)
        {
            foreach (var predecessor in predecessors)
                After(predecessor);

            return predecessors;
        }
        #endregion

        #region resource constraints
        /// <summary>
        /// Indicates that this process requires the specified resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>returns this process</returns>
        public void Requires(Resource resource)
        {
            DependencyGraph.CheckGraph(resource, this);

            if (_resources.Add(resource))
                resource.UsedBy(this);
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Process { " + Name + " }";
        }
    }
}
