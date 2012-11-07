using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TopologicalSorting;

namespace Topological_Sorting_Test
{
    [TestClass]
    public class Resources
    {
        /// <summary>
        /// tests if basic resource resolution works
        /// </summary>
        [TestMethod]
        public void BasicResourceResolution()
        {
            DependencyGraph g = new DependencyGraph();

            Resource res = new Resource(g, "resource");

            OrderedProcess a = new OrderedProcess(g, "A");
            OrderedProcess b = new OrderedProcess(g, "B");
            OrderedProcess c = new OrderedProcess(g, "C");

            a.Before(b);
            a.Before(c);

            b.Requires(res);
            c.Requires(res);

            IEnumerable<IEnumerable<OrderedProcess>> s = g.CalculateSort();

            Assert.AreEqual(3, s.Count());

            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.AreEqual(a, s.Skip(0).First().First());

            Assert.AreEqual(1, s.Skip(1).First().Count());
            Assert.IsTrue(s.Skip(1).First().First() == b || s.Skip(1).First().First() == c);

            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.IsTrue(s.Skip(2).First().First() == b || s.Skip(2).First().First() == c);

            Assert.AreNotEqual(s.Skip(1).First().First(), s.Skip(2).First().First());
        }

        /// <summary>
        /// Test if resource resolution works on a complex branching graph
        /// </summary>
        [TestMethod]
        public void BranchingResourceResolution()
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

            Resource resource = new Resource(g, "Resource");
            resource.UsedBy(c1, c3);

            IEnumerable<IEnumerable<OrderedProcess>> s = g.CalculateSort();

            //check that A comes first
            Assert.AreEqual(1, s.Skip(0).First().Count());
            Assert.AreEqual(a, s.Skip(0).First().First());

            //check that D comes last
            Assert.AreEqual(1, s.Skip(4).First().Count());
            Assert.AreEqual(d, s.Skip(4).First().First());

            //check that no set contains both c1 and c3
            Assert.AreEqual(0, s.Where(set => set.Contains(c1) && set.Contains(c3)).Count());
        }
    }
}
