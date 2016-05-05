using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopologicalSorting;

namespace Topological_Sorting_Test
{
    [TestClass]
    public class Ordering
    {
        /// <summary>
        /// Test that a simple A->B->C graph works with every possible restriction on ordering
        /// </summary>
        [TestMethod]
        public void BasicOrderAfter()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b = new OrderedProcess(g, "B");
            OrderedProcess c = new OrderedProcess(g, "C");

            a.Before(b).Before(c);

            c.After(b).After(a);

            IEnumerable<IEnumerable<OrderedProcess>> s = g.CalculateSort();

            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.AreEqual(a, s.Skip(0).First().First());

            Assert.AreEqual(1, s.Skip(1).First().Count());
            Assert.AreEqual(b, s.Skip(1).First().First());

            Assert.AreEqual(1, s.Skip(2).First().Count());
            Assert.AreEqual(c, s.Skip(2).First().First());
        }

        /// <summary>
        /// Test that a simple A->B->C graph works with minimum restrictions on ordering
        /// </summary>
        [TestMethod]
        public void BasicOrderBefore()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b = new OrderedProcess(g, "B");
            OrderedProcess c = new OrderedProcess(g, "C");

            a.Before(b).Before(c);

            IEnumerable<IEnumerable<OrderedProcess>> s = g.CalculateSort();

            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.AreEqual(a, s.Skip(0).First().First());

            Assert.AreEqual(1, s.Skip(1).First().Count());
            Assert.AreEqual(b, s.Skip(1).First().First());

            Assert.AreEqual(1, s.Skip(2).First().Count());
            Assert.AreEqual(c, s.Skip(2).First().First());
        }

        /// <summary>
        /// Tests that an impossible ordering constraint is detected
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Unorderable()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b = new OrderedProcess(g, "B");

            a.Before(b);
            b.Before(a);

            g.CalculateSort();
        }

        /// <summary>
        /// Test that a graph with a split in the middle will order properly
        /// </summary>
        [TestMethod]
        public void BasicBranching()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b1 = new OrderedProcess(g, "B1");
            OrderedProcess b2 = new OrderedProcess(g, "B2");
            OrderedProcess c = new OrderedProcess(g, "C");

            a.Before(b1, b2).Before(c);

            IEnumerable<IEnumerable<OrderedProcess>> s = g.CalculateSort();

            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.AreEqual(a, s.Skip(0).First().First());

            Assert.AreEqual(2, s.Skip(1).First().Count());
            Assert.IsTrue(s.Skip(1).First().Contains(b1));
            Assert.IsTrue(s.Skip(1).First().Contains(b2));

            Assert.AreEqual(1, s.Skip(2).First().Count());
            Assert.AreEqual(c, s.Skip(2).First().First());
        }

        /// <summary>
        /// Test a complex branching scheme
        /// </summary>
        [TestMethod]
        public void ComplexBranching()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b1 = new OrderedProcess(g, "B1");
            OrderedProcess b2 = new OrderedProcess(g, "B2");
            OrderedProcess c1 = new OrderedProcess(g, "C1");
            OrderedProcess c2 = new OrderedProcess(g, "C2");
            OrderedProcess c3 = new OrderedProcess(g, "C3");
            OrderedProcess c4 = new OrderedProcess(g, "C4");
            OrderedProcess d = new OrderedProcess(g, "D");

            a.Before(b1, b2).Before(c1, c2, c3, c4).Before(d);

            IEnumerable<IEnumerable<OrderedProcess>> s = g.CalculateSort();

            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.AreEqual(a, s.Skip(0).First().First());

            Assert.AreEqual(2, s.Skip(1).First().Count());
            Assert.IsTrue(s.Skip(1).First().Contains(b1));
            Assert.IsTrue(s.Skip(1).First().Contains(b2));

            Assert.AreEqual(4, s.Skip(2).First().Count());
            Assert.IsTrue(s.Skip(2).First().Contains(c1));
            Assert.IsTrue(s.Skip(2).First().Contains(c2));
            Assert.IsTrue(s.Skip(2).First().Contains(c3));
            Assert.IsTrue(s.Skip(2).First().Contains(c4));

            Assert.AreEqual(1, s.Skip(3).First().Count());
            Assert.AreEqual(d, s.Skip(3).First().First());
        }

        /// <summary>
        /// Tests that a complex branching system with an impossible constraint is detected
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BranchingUnorderable()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b1 = new OrderedProcess(g, "B1");
            OrderedProcess b2 = new OrderedProcess(g, "B2");
            OrderedProcess c1 = new OrderedProcess(g, "C1");
            OrderedProcess c2 = new OrderedProcess(g, "C2");
            OrderedProcess c3 = new OrderedProcess(g, "C3");
            OrderedProcess c4 = new OrderedProcess(g, "C4");
            OrderedProcess d = new OrderedProcess(g, "D");

            a.Before(b1, b2).Before(c1, c2, c3, c4).Before(d).Before(b1);

            g.CalculateSort();
        }
    }
}
