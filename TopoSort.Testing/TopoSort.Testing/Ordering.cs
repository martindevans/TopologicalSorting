using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;


namespace TopoSort.Testing {

    public class OrderingTests {

        /// <summary>
        /// Test that a graph with a split in the middle will order properly
        /// </summary>
        [Fact]
        public void BasicBranching() {
            var g = new DependencyGraph<string>();

            var a = new OrderedProcess<string>(g, "A");
            var b1 = new OrderedProcess<string>(g, "B1");
            var b2 = new OrderedProcess<string>(g, "B2");
            var c = new OrderedProcess<string>(g, "C");

            a.Before(b1, b2).Before(c);

            IEnumerable<IEnumerable<OrderedProcess<string>>> s = g.CalculateSort();

            Assert.Single(s.Skip(0).First());
            Assert.Equal(a, s.Skip(0).First().First());

            Assert.Equal(2, s.Skip(1).First().Count());
            Assert.Contains(b1, s.Skip(1).First());
            Assert.Contains(b2, s.Skip(1).First());

            Assert.Single(s.Skip(2).First());
            Assert.Equal(c, s.Skip(2).First().First());
        }

        /// <summary>
        /// Test that a simple A->B->C graph works with every possible restriction on ordering
        /// </summary>
        [Fact]
        public void BasicOrderAfter() {
            var g = new DependencyGraph<string>();

            var a = new OrderedProcess<string>(g, "A");
            var b = new OrderedProcess<string>(g, "B");
            var c = new OrderedProcess<string>(g, "C");

            a.Before(b).Before(c);

            c.After(b).After(a);

            IEnumerable<IEnumerable<OrderedProcess<string>>> s = g.CalculateSort();

            Assert.Single(s.Skip(0).First());
            Assert.Equal(a, s.Skip(0).First().First());

            Assert.Single(s.Skip(1).First());
            Assert.Equal(b, s.Skip(1).First().First());

            Assert.Single(s.Skip(2).First());
            Assert.Equal(c, s.Skip(2).First().First());
        }

        /// <summary>
        /// Test that a simple A->B->C graph works with minimum restrictions on ordering
        /// </summary>
        [Fact]
        public void BasicOrderBefore() {
            var g = new DependencyGraph<string>();

            var a = new OrderedProcess<string>(g, "A");
            var b = new OrderedProcess<string>(g, "B");
            var c = new OrderedProcess<string>(g, "C");

            a.Before(b).Before(c);

            IEnumerable<IEnumerable<OrderedProcess<string>>> s = g.CalculateSort();

            Assert.Single(s.Skip(0).First());
            Assert.Equal(a, s.Skip(0).First().First());

            Assert.Single(s.Skip(1).First());
            Assert.Equal(b, s.Skip(1).First().First());

            Assert.Single(s.Skip(2).First());
            Assert.Equal(c, s.Skip(2).First().First());
        }

        /// <summary>
        /// Tests that a complex branching system with an impossible constraint is detected
        /// </summary>
        [Fact]
        public void BranchingUnorderable() {
            var g = new DependencyGraph<string>();

            var a = new OrderedProcess<string>(g, "A");
            var b1 = new OrderedProcess<string>(g, "B1");
            var b2 = new OrderedProcess<string>(g, "B2");
            var c1 = new OrderedProcess<string>(g, "C1");
            var c2 = new OrderedProcess<string>(g, "C2");
            var c3 = new OrderedProcess<string>(g, "C3");
            var c4 = new OrderedProcess<string>(g, "C4");
            var d = new OrderedProcess<string>(g, "D");

            a.Before(b1, b2).Before(c1, c2, c3, c4).Before(d).Before(b1);

            Assert.Throws<InvalidOperationException>(() => g.CalculateSort());
        }

        /// <summary>
        /// Test a complex branching scheme
        /// </summary>
        [Fact]
        public void ComplexBranching() {
            var g = new DependencyGraph<string>();

            var a = new OrderedProcess<string>(g, "A");
            var b1 = new OrderedProcess<string>(g, "B1");
            var b2 = new OrderedProcess<string>(g, "B2");
            var c1 = new OrderedProcess<string>(g, "C1");
            var c2 = new OrderedProcess<string>(g, "C2");
            var c3 = new OrderedProcess<string>(g, "C3");
            var c4 = new OrderedProcess<string>(g, "C4");
            var d = new OrderedProcess<string>(g, "D");

            a.Before(b1, b2).Before(c1, c2, c3, c4).Before(d);

            IEnumerable<IEnumerable<OrderedProcess<string>>> s = g.CalculateSort();

            Assert.Single(s.Skip(0).First());
            Assert.Equal(a, s.Skip(0).First().First());

            Assert.Equal(2, s.Skip(1).First().Count());
            Assert.Contains(b1, s.Skip(1).First());
            Assert.Contains(b2, s.Skip(1).First());

            Assert.Equal(4, s.Skip(2).First().Count());
            Assert.Contains(c1, s.Skip(2).First());
            Assert.Contains(c2, s.Skip(2).First());
            Assert.Contains(c3, s.Skip(2).First());
            Assert.Contains(c4, s.Skip(2).First());

            Assert.Single(s.Skip(3).First());
            Assert.Equal(d, s.Skip(3).First().First());
        }

        /// <summary>
        /// Tests that an impossible ordering constraint is detected
        /// </summary>
        [Fact]
        public void Unorderable() {
            var g = new DependencyGraph<string>();

            var a = new OrderedProcess<string>(g, "A");
            var b = new OrderedProcess<string>(g, "B");

            a.Before(b);
            b.Before(a);

            Assert.Throws<InvalidOperationException>(() => g.CalculateSort());
        }

    }

}