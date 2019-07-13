This is a fork of Martin Devans' (https://github.com/martindevans/TopologicalSorting)[TopologicalSorting] library.

Topological Sorting
===================

Topological sorting is the process or ordering a directed graph, such that all vertices with a link to another vertex are considered before the destination vertex. Topological sorting is often used for dependency resolution, when a series of tasks need to be executed, and a certain task cannot be executed until another one it depends upon has already executed.

TopologicalSorting
------------------

This is a class library to provide topological sorting to .net programs using a fluent interface or class attributes to express relationships between processes. The system also provides a system for expressing if some tasks use a resource which cannot be concurrently accessed and thus two tasks which use that resource must not be run in parallel.

Fluent Interface
----------------

Tasks are represented by OrderedProcess objects. OrderedProcess objects have names to uniquely identifty them, or you can inherit off OrderedProcess directly in your code where you implement your application specific tasks. Order of tasks is simple (find more examples in the Tests project):

> DependencyGraph g = new DependencyGraph();
>
> OrderedProcess a = new OrderedProcess(g, "Task A");
>
> OrderedProcess b = new OrderedProcess(g, "Task B");
>
> OrderedProcess c = new OrderedProcess(g, "Task C");
> 
> a.Before(b);
>
> b.Before(c);

The ordering relationships can be chained, so the previous ordering could alternatively be expressed like so:

> a.Before(b).Before(c);

Of course, the expression of ordering can also be reversed:

> c.After(b).After(a);

Multiple process relationships can be expressed together:

> a.Before(b, c, d);

Even more can be chained on to the end of this:

> a.Before(b, c, d).After(e);

This states that a must run before b, c or d, and that e must run before b, c and d.

Output
------

A sort operation is calculated by calling CalculateSort on a dependency graph after all the process relationships have been set up. The output is a TopologicalSort object (or an InvalidOperationException if an impossible set of constraints is in place). A topological sort object is enumerable in two different ways. enumerating it as an IEnumerable<OrderedProcess> will return all the processes in a valid order to execute them. Alternatively enumerating as an IEnumerable<ISet<OrderedProcess>> will enumerate sets of processes which may be executed in parallel.

Example
-------

> a.Before(b, c).Before(d);

In this example, "a" must execute before "b", "c" and "d", and "d" must execute before "b" and "c", but there are no constraints between "b" and "c". Therefore either of these results would be valid:

> a -> d -> b -> c

> a -> d -> c -> b

However, if enumerated as an IEnumerable<ISet<OrderedProcess>> we would instead get the one single valid result:

> a -> d -> {b, c}
