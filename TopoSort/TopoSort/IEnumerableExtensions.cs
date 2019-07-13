using System.Collections.Generic;


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

    }

}