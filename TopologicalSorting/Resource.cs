using System.Collections.Generic;

namespace TopologicalSorting
{
    /// <summary>
    /// A class of resource which may be used by a single concurrent process
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The name of this resource
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The graph this class is part of
        /// </summary>
        public readonly DependencyGraph Graph;

        private readonly HashSet<OrderedProcess> _users = new HashSet<OrderedProcess>();
        /// <summary>
        /// Gets a set of processes which use this resource
        /// </summary>
        /// <value>The users.</value>
        public IEnumerable<OrderedProcess> Users
        {
            get
            {
                return _users;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="graph">The graph which this ResourceClass is part of</param>
        /// <param name="name">The name of this resource</param>
        public Resource(DependencyGraph graph, string name)
        {
            Graph = graph;
            Name = name;

            Graph.Add(this);
        }

        #region constraints
        /// <summary>
        /// Indicates that this resource is used by the given process
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public void UsedBy(OrderedProcess process)
        {
            DependencyGraph.CheckGraph(this, process);

            if (_users.Add(process))
                process.Requires(this);
        }

        /// <summary>
        /// Indicates that this resource is used by the given processes
        /// </summary>
        /// <param name="processes">The processes.</param>
        public void UsedBy(params OrderedProcess[] processes)
        {
            UsedBy(processes as IEnumerable<OrderedProcess>);
        }

        /// <summary>
        /// Indicates that this resource is used by the given processes
        /// </summary>
        /// <param name="processes">The processes.</param>
        public void UsedBy(IEnumerable<OrderedProcess> processes)
        {
            foreach (var process in processes)
                UsedBy(process);
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
            return "Resource { " + Name + " }";
        }
    }
}
