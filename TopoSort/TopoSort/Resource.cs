using System.Collections.Generic;


namespace TopoSort {

    /// <summary>
    /// A class of resource which may be used by a single concurrent process
    /// </summary>
    public class Resource<T> {

        private readonly HashSet<OrderedProcess<T>> _users = new HashSet<OrderedProcess<T>>();

        /// <summary>
        /// The graph this class is part of
        /// </summary>
        public readonly DependencyGraph<T> Graph;

        /// <summary>
        /// The name of this resource
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource{T}"/> class.
        /// </summary>
        /// <param name="name">The graph which this ResourceClass is part of</param>
        /// <param name="name">The name of this resource</param>
        public Resource(DependencyGraph<T> graph, string name) {
            Graph = graph;
            Name = name;

            Graph.Add(this);
        }

        /// <summary>
        /// Gets a set of processes which use this resource
        /// </summary>
        /// <value>The users.</value>
        public IEnumerable<OrderedProcess<T>> Users => _users;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return "Resource { " + Name + " }";
        }

        #region constraints

        /// <summary>
        /// Indicates that this resource is used by the given process
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public void UsedBy(OrderedProcess<T> process) {
            DependencyGraph<T>.CheckGraph(this, process);

            if (_users.Add(process)) {
                process.Requires(this);
            }
        }

        /// <summary>
        /// Indicates that this resource is used by the given processes
        /// </summary>
        /// <param name="processes">The processes.</param>
        public void UsedBy(params OrderedProcess<T>[] processes) {
            UsedBy(processes as IEnumerable<OrderedProcess<T>>);
        }

        /// <summary>
        /// Indicates that this resource is used by the given processes
        /// </summary>
        /// <param name="processes">The processes.</param>
        public void UsedBy(IEnumerable<OrderedProcess<T>> processes) {
            foreach (var process in processes) {
                UsedBy(process);
            }
        }

        #endregion

    }

}