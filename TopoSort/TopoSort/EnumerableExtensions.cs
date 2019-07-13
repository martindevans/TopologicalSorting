using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace TopoSort {

    /// <summary>
    /// Extensions to IEnumerable
    /// </summary>
    public static class EnumerableExtensions {

        /// <summary>
        /// Determines whether the specified enumerable is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">The e.</param>
        /// <returns>
        /// 	<c>true</c> if the specified e is empty; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsEmpty<T>(this IEnumerable<T> e) {
            return !e.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Indicates that all the members of the enumerable must happen after the single predecessor
        /// </summary>
        /// <param name="followers">The followers.</param>
        /// <param name="predecessor">The predecessor.</param>
        /// <returns>the predecessor</returns>
        public static OrderedProcess<T> After<T>(this IEnumerable<OrderedProcess<T>> followers,
                                                 OrderedProcess<T> predecessor) {
            predecessor.Before(followers);

            return predecessor;
        }

        /// <summary>
        /// Indicates that all members of the enumerable must happen after all the predecessors
        /// </summary>
        /// <param name="followers">The followers.</param>
        /// <param name="predecessors">The predecessors.</param>
        /// <returns>the predecessors</returns>
        public static IEnumerable<OrderedProcess<T>> After<T>(this IEnumerable<OrderedProcess<T>> followers,
                                                              params OrderedProcess<T>[] predecessors) {
            return After(followers, predecessors as IEnumerable<OrderedProcess<T>>);
        }

        /// <summary>
        /// Indicates that all members of the enumerable must happen after all the predecessors
        /// </summary>
        /// <param name="followers">The followers.</param>
        /// <param name="predecessors">The predecessors.</param>
        /// <returns>the predecessors</returns>
        public static IEnumerable<OrderedProcess<T>> After<T>(this IEnumerable<OrderedProcess<T>> followers,
                                                              IEnumerable<OrderedProcess<T>> predecessors) {
            foreach (var predecessor in predecessors) {
                predecessor.Before(followers);
            }

            return predecessors;
        }

        /// <summary>
        /// Indicates that all members of the enumerable must happen before the single follower
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        /// <param name="follower">The follower.</param>
        /// <returns>the followers</returns>
        public static OrderedProcess<T> Before<T>(this IEnumerable<OrderedProcess<T>> predecessors,
                                                  OrderedProcess<T> follower) {
            follower.After(predecessors);

            return follower;
        }

        /// <summary>
        /// Indicates that all members of the enumerable must happen before all the followers
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        /// <param name="followers">The followers.</param>
        /// <returns>the followers</returns>
        public static IEnumerable<OrderedProcess<T>> Before<T>(this IEnumerable<OrderedProcess<T>> predecessors,
                                                               params OrderedProcess<T>[] followers) {
            return Before(predecessors, followers as IEnumerable<OrderedProcess<T>>);
        }

        /// <summary>
        /// Indicates that all members of the enumerable must happen before all the followers
        /// </summary>
        /// <param name="predecessors">The predecessors.</param>
        /// <param name="followers">The followers.</param>
        /// <returns>the followers</returns>
        public static IEnumerable<OrderedProcess<T>> Before<T>(this IEnumerable<OrderedProcess<T>> predecessors,
                                                               IEnumerable<OrderedProcess<T>> followers) {
            foreach (var follower in followers) {
                follower.After(predecessors);
            }

            return followers;
        }

        private static IEnumerable<DependsOnAttribute> GetAttributes(Type type) {
            return type
                   .GetCustomAttributes(typeof(DependsOnAttribute))
                   .Select(a => (DependsOnAttribute) a);
        }

        private static HashSet<Type> GetDependencies(Type type) {
            var attrs = GetAttributes(type);
            return new HashSet<Type>(attrs.SelectMany(a => a.Dependencies));
        }

        private static void UpdateTypeMap(IDictionary<Type, HashSet<Type>> typeMap,
                                          Type type,
                                          HashSet<Type> dependencies) {
            if (typeMap.ContainsKey(type)) {
                var set = typeMap[type];
                set.IntersectWith(dependencies);
            } else {
                typeMap[type] = dependencies;
            }
        }

        public static void UpdateProcessMap<TType>(Dictionary<Type, HashSet<OrderedProcess<TType>>> processMap,
                                                   Type type,
                                                   OrderedProcess<TType> process) {
            if (processMap.ContainsKey(type)) {
                var set = processMap[type];
                set.Add(process);
            } else {
                processMap[type] = new HashSet<OrderedProcess<TType>> {process};
            }
        }

        public static void InitializeMaps<TType>(DependencyGraph<TType> graph,
                                                 IEnumerable<TType> items,
                                                 Dictionary<Type, HashSet<Type>> typeMap,
                                                 Dictionary<Type, HashSet<OrderedProcess<TType>>> processMap) {
            foreach (var item in items) {
                var process = new OrderedProcess<TType>(graph, item);

                var type = item.GetType();
                var deps = GetDependencies(type);

                UpdateTypeMap(typeMap, type, deps);
                UpdateProcessMap(processMap, type, process);
            }
        }

        public static List<TType> GraphAsList<TType>(TopologicalSort<TType> solution) {
            var results = new List<TType>();

            foreach (OrderedProcess<TType> item in solution) {
                results.Add(item.Value);
            }

            return results;
        }

        public static IEnumerable<TType> Sort<TType>(this IEnumerable<TType> items) {
            var graph = new DependencyGraph<TType>();
            var typeMap = new Dictionary<Type, HashSet<Type>>();
            var processMap = new Dictionary<Type, HashSet<OrderedProcess<TType>>>();

            InitializeMaps(graph, items, typeMap, processMap);

            foreach (var typeEntry in typeMap) {
                var type = typeEntry.Key;
                var dependencyTypes = typeEntry.Value;
                var dependencyObjects = dependencyTypes.SelectMany(dt => processMap[dt]);

                foreach (var instance in processMap[typeEntry.Key]) {
                    instance.After(dependencyObjects);
                }

            }

            var result = graph.CalculateSort();
            return GraphAsList(result);
        }

    }

}