using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace TopoSort {

    /// <summary>
    /// Represents a sorting solution
    /// </summary>
    public class TopologicalSort<T>
        : IEnumerable<ISet<OrderedProcess<T>>>, IEnumerable<OrderedProcess<T>> {

        private readonly List<ISet<OrderedProcess<T>>> _collections = new List<ISet<OrderedProcess<T>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSort{T}"/> class.
        /// </summary>
        public TopologicalSort() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSort{T}"/> class.
        /// </summary>
        /// <param name="g">The graph to fill this sort with</param>
        public TopologicalSort(DependencyGraph<T> g)
            : this() {
            g.CalculateSort(this);
        }

        internal void Append(ISet<OrderedProcess<T>> collection) {
            _collections.Add(collection);
        }

        #region IEnumerable

        /// <summary>
        /// Gets the enumerator which enumerates sets of processes, where a set can be executed in any order
        /// </summary>
        /// <returns></returns>
        IEnumerator<ISet<OrderedProcess<T>>> IEnumerable<ISet<OrderedProcess<T>>>.GetEnumerator() {
            return _collections.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the processes
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator() {
            return (this as IEnumerable<OrderedProcess<T>>).GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator which enumerates through the processes in an order to be executed
        /// </summary>
        /// <returns></returns>
        IEnumerator<OrderedProcess<T>> IEnumerable<OrderedProcess<T>>.GetEnumerator() {
            IEnumerable<IEnumerable<OrderedProcess<T>>> collections = this;

            return collections.SelectMany(collection => collection).GetEnumerator();
        }

        #endregion

    }

}