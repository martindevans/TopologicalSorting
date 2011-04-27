using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopologicalSorting
{
    /// <summary>
    /// Represents a sorting solution
    /// </summary>
    public class TopologicalSort
        :IEnumerable<ISet<OrderedProcess>>, IEnumerable<OrderedProcess>
    {
        private List<ISet<OrderedProcess>> collections = new List<ISet<OrderedProcess>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSort"/> class.
        /// </summary>
        public TopologicalSort()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSort"/> class.
        /// </summary>
        /// <param name="g">The graph to fill this sort with</param>
        public TopologicalSort(DependancyGraph g)
            :this()
        {
            g.CalculateSort(this);
        }

        internal void Append(ISet<OrderedProcess> collection)
        {
            collections.Add(collection);
        }

        #region IEnumerable
        /// <summary>
        /// Gets the enumerator which enumerates sets of processes, where a set can be executed in any order
        /// </summary>
        /// <returns></returns>
        IEnumerator<ISet<OrderedProcess>> IEnumerable<ISet<OrderedProcess>>.GetEnumerator()
        {
            return collections.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the processes
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return (this as IEnumerable<OrderedProcess>).GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator which enumerates through the processes in an order to be executed
        /// </summary>
        /// <returns></returns>
        IEnumerator<OrderedProcess> IEnumerable<OrderedProcess>.GetEnumerator()
        {
            IEnumerable<IEnumerable<OrderedProcess>> collections = (this as IEnumerable<IEnumerable<OrderedProcess>>);

            foreach (var collection in collections)
                foreach (var process in collection)
                    yield return process;
        }
        #endregion
    }
}
