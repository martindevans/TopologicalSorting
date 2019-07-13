using System.Collections.Generic;

using Xunit;

using TopoSort;


namespace TopoSort.Testing {

    internal abstract class BaseClass {

        public override bool Equals(object o) {
            if (o is BaseClass other)
                return GetType().Name == other.GetType().Name;

            return false;
        }

        public override int GetHashCode() {
            return GetType().Name.GetHashCode();
        }

    }

    [DependsOn(typeof(ClassBar))]
    internal class ClassFoo : BaseClass { }

    internal class ClassBar : BaseClass { }

    public class DependsOnTests {

        [Fact]
        public void EqualityWorks() {
            Assert.Equal(new ClassFoo(), new ClassFoo());
        }

        [Fact]
        public void TestDependsOn() {

            IEnumerable<BaseClass> instances = new List<BaseClass> {
                new ClassBar(),
                new ClassFoo(),
                new ClassBar(),
                new ClassFoo(),
            };

            var sorted = instances.TopoSort();

            Assert.Equal(sorted, new List<BaseClass> {
                new ClassBar(),
                new ClassBar(),
                new ClassFoo(),
                new ClassFoo(),
            });

        }

    }

}