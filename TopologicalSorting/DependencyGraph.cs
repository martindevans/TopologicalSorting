using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace TopologicalSorting
{
    /// <summary>
    /// A graph of processes and resources from which a topological sort can be extracted
    /// </summary>
    public class DependencyGraph<T>
    {
        #region fields
        readonly HashSet<OrderedProcess<T>> _processes = new HashSet<OrderedProcess<T>>();
        /// <summary>
        /// Gets the processes which are part of this dependency graph
        /// </summary>
        /// <value>The processes.</value>
        public IEnumerable<OrderedProcess<T>> Processes
        {
            get
            {
                return _processes;
            }
        }

        readonly HashSet<Resource<T>> _resources = new HashSet<Resource<T>>();
        /// <summary>
        /// Gets the resources which are part of this dependency graph
        /// </summary>
        /// <value>The resources.</value>
        public IEnumerable<Resource<T>> Resources
        {
            get
            {
                return _resources;
            }
        }
        #endregion

        /// <summary>
        /// Gets the process count.
        /// </summary>
        /// <value>The process count.</value>
        public int ProcessCount
        {
            get
            {
                return _processes.Count;
            }
        }

        #region sorting
        /// <summary>
        /// Calculates the sort which results from this dependency network
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if no sort exists for the given set of constraints</exception>
        public TopologicalSort<T> CalculateSort(IEqualityComparer<T> comparer = null)
        {
            return CalculateSort(new TopologicalSort<T>(), comparer);
        }

        /// <summary>
        /// Append the result of this dependency graph to the end of the given sorting solution
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public TopologicalSort<T> CalculateSort(TopologicalSort<T> instance, IEqualityComparer<T> comparer = null)
        {
	        OrderedProcessComprarer<T> orderedProcessComparer = new OrderedProcessComprarer<T>(comparer ?? EqualityComparer<T>.Default);

            HashSet<OrderedProcess<T>> unused = new HashSet<OrderedProcess<T>>(_processes);

            do
            {
                HashSet<OrderedProcess<T>> set = new HashSet<OrderedProcess<T>>(
                    unused.Where(p => !unused.Overlaps(p.Predecessors)), //select processes which have no predecessors in the unused set, which means that all their predecessors must either be used, or not exist, either way is fine
					orderedProcessComparer
				);

                if (set.Count == 0)
                    throw new InvalidOperationException("Cannot order this set of processes");

                unused.ExceptWith(set);

                foreach (var subset in SolveResourceDependencies(set))
                    instance.Append(subset);
            }
            while (unused.Count > 0);

            return instance;
        }

        /// <summary>
        /// Given a set of processes which are not interdependent, split up into multiple sets which do not use the same resource concurrently
        /// </summary>
        /// <param name="processes">The processes.</param>
        /// <returns></returns>
        private IEnumerable<ISet<OrderedProcess<T>>> SolveResourceDependencies(ISet<OrderedProcess<T>> processes)
        {
            // if there are no resources in this graph, or none of the processes in this set have any
            // resources, we can simply return the set of processes
            if (_resources.Count == 0 || !processes.SelectMany(p => p.ResourcesSet).Any())
                yield return processes;
            else
            {
                HashSet<HashSet<OrderedProcess<T>>> result = new HashSet<HashSet<OrderedProcess<T>>>();

                foreach (var process in processes)
                {
                    var process1 = process;

                    //all sets this process may be added to
                    IEnumerable<HashSet<OrderedProcess<T>>> agreeableSets = result                     //from the set of result sets
                        .Where(set => set                                                           //select a candidate set to add to
                            .Where(p => p.ResourcesSet.Overlaps(process1.Resources))                //select processes whose resource usage overlaps this one
                            .IsEmpty());                                                            //if there are none which overlap, then this is a valid set

                    //the single best set to add to
                    HashSet<OrderedProcess<T>> agreeableSet;

                    var enumerable = agreeableSets as HashSet<OrderedProcess<T>>[] ?? agreeableSets.ToArray();
                    if (enumerable.IsEmpty())
                    {
                        //no sets can hold this process, create a new one
                        agreeableSet = new HashSet<OrderedProcess<T>>();
                        result.Add(agreeableSet);
                    }
                    else
                        agreeableSet = enumerable.Aggregate((a, b) => a.Count < b.Count ? a : b);    //pick the smallest set

                    //finally, add this process to the selected set
                    agreeableSet.Add(process);
                }

                foreach (var set in result)
                    yield return set;
            }
        }
        #endregion

        internal bool Add(OrderedProcess<T> orderedProcess)
        {
            return _processes.Add(orderedProcess);
        }

        internal bool Add(Resource<T> resourceClass)
        {
            return _resources.Add(resourceClass);
        }

        internal static void CheckGraph(OrderedProcess<T> a, OrderedProcess<T> b)
        {
            if (a.Graph != b.Graph)
                throw new ArgumentException(string.Format("process {0} is not associated with the same graph as process {1}", a, b));
        }

        internal static void CheckGraph(Resource<T> a, OrderedProcess<T> b)
        {
            if (a.Graph != b.Graph)
                throw new ArgumentException(string.Format("Resource {0} is not associated with the same graph as process {1}", a, b));
        }
    }
}
