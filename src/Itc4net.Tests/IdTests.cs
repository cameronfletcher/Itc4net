using System;
using System.Diagnostics.CodeAnalysis;
using Shouldly;
using NSubstitute;
using TUnit.Core;

namespace Itc4net.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IdTests
    {
        [Test]
        public void ToStringShouldCorrectlyRepresentLeafWith0()
        {
            new Id.Leaf(0).ToString().ShouldBe("0");
        }

        [Test]
        public void ToStringShouldCorrectlyRepresentLeafWith1()
        {
            new Id.Leaf(1).ToString().ShouldBe("1");
        }

        [Test]
        public void ToStringShouldCorrectlyRepresentNode()
        {
            var id = new Id.Node(1, new Id.Node(0, 1));

            id.ToString().ShouldBe("(1,(0,1))");
        }

        [Test]
        public void LeafCtorShouldAccept0()
        {
            new Id.Leaf(0).Value.ShouldBe(0);
        }

        [Test]
        public void LeafCtorShouldAccept1()
        {
            new Id.Leaf(1).Value.ShouldBe(1);
        }

        [Test]
        public void LeafCtorShouldThrowWhenLessThan0()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new Id.Leaf(-1);
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void LeafCtorShouldThrowWhenGreaterThan1()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new Id.Leaf(2);
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void NodeCtorShouldThrowWhenLeftIdNull()
        {
            Action act = () => new Id.Node(null, 0);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void NodeCtorShouldThrowWhenRightIdNull()
        {
            Action act = () => new Id.Node(0, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void MatchShouldInvokeLeafFuncWhenIdRepresentsLeaf()
        {
            // Arrange
            Id id = new Id.Leaf(0);

            Func<int, object> leafFn = Substitute.For<Func<int, object>>();
            Func<Id, Id, object> nodeFn = Substitute.For<Func<Id, Id, object>>();

            // Act
            id.Match(leafFn, nodeFn);

            // Assert
            leafFn.Received().Invoke(0);
            nodeFn.DidNotReceiveWithAnyArgs();
        }

        [Test]
        public void MatchShouldInvokeNodeFuncWhenIdRepresentsNode()
        {
            // Arrange
            Id id = new Id.Node(0, 1);

            Func<int, object> leafFn = Substitute.For<Func<int, object>>();
            Func<Id, Id, object> nodeFn = Substitute.For<Func<Id, Id, object>>();

            // Act
            id.Match(leafFn, nodeFn);

            // Assert
            leafFn.DidNotReceiveWithAnyArgs();
            nodeFn.Received().Invoke(0, 1);
        }

        [Test]
        public void MatchShouldInvokeLeafActionWhenIdRepresentsLeaf()
        {
            // Arrange
            Id id = new Id.Leaf(0);

            Action<int> leafAction = Substitute.For<Action<int>>();
            Action<Id, Id> nodeAction = Substitute.For<Action<Id, Id>>();

            // Act
            id.Match(leafAction, nodeAction);

            // Assert
            leafAction.Received().Invoke(0);
            nodeAction.DidNotReceiveWithAnyArgs();
        }

        [Test]
        public void MatchShouldInvokeNodeActionWhenIdRepresentsNode()
        {
            // Arrange
            Id id = new Id.Node(0, 1);

            Action<int> leafAction = Substitute.For<Action<int>>();
            Action<Id, Id> nodeAction = Substitute.For<Action<Id, Id>>();

            // Act
            id.Match(leafAction, nodeAction);

            // Assert
            leafAction.DidNotReceiveWithAnyArgs();
            nodeAction.Received().Invoke(0, 1);
        }

        [Test]
        public void NormalizeShouldReturniWhenLeafIsi() // norm(i) = i
        {
            // Arrange
            Id id = new Id.Leaf(1);

            // Act
            Id normalized = id.Normalize();

            // Assert
            normalized.ShouldBe((Id)1);
        }

        [Test]
        public void NormalizeShouldReturn0WhenNodeIs0and0() // norm((0,0)) = 0
        {
            // Arrange
            Id id = new Id.Node(0, 0);

            // Act
            Id normalized = id.Normalize();

            // Assert
            normalized.ShouldBe((Id)0);
        }

        [Test]
        public void NormalizeShouldReturn1WhenNodeIs1and1() // norm((1,1)) = 1
        {
            // Arrange
            Id id = new Id.Node(1, 1);

            // Act
            Id normalized = id.Normalize();

            // Assert
            normalized.ShouldBe((Id)1);
        }

        [Test]
        public void NormalizeShouldReturnUnchangedWhenNodeCannotBeSimplifiedLike0and1()
        {
            // Arrange
            Id id = new Id.Node(0, 1);

            // Act
            Id normalized = id.Normalize();

            // Assert
            normalized.ShouldBe(id);
        }

        [Test]
        public void NormalizeShouldReturnUnchangedWhenNodeCannotBeSimplifiedLike1and0()
        {
            // Arrange
            Id id = new Id.Node(1, 0);

            // Act
            Id normalized = id.Normalize();

            // Assert
            normalized.ShouldBe(id);
        }

        [Test]
        public void EqualsShouldReturnTrueWhenComparingLeafsWithSameValues()
        {
            // Arrange
            Id i1 = new Id.Leaf(0);
            Id i2 = new Id.Leaf(0);

            // Act & Assert
            i1.Equals(i2).ShouldBeTrue();
        }

        [Test]
        public void EqualsShouldReturnFalseWhenComparingLeafsWithDifferentValues()
        {
            // Arrange
            Id i1 = new Id.Leaf(0);
            Id i2 = new Id.Leaf(1);

            // Act & Assert
            i1.Equals(i2).ShouldBeFalse();
        }

        [Test]
        public void EqualsShouldReturnTrueWhenComparingNodesWithSameValues()
        {
            // Arrange
            Id i1 = new Id.Node(0, 1);
            Id i2 = new Id.Node(0, 1);

            // Act & Assert
            i1.Equals(i2).ShouldBeTrue();
        }

        [Test]
        public void EqualsShouldReturnFalseWhenComparingNodesWithDifferentValues()
        {
            // Arrange
            Id i1 = new Id.Node(0, 0);
            Id i2 = new Id.Node(0, 1);

            // Act & Assert
            i1.Equals(i2).ShouldBeFalse();
        }

        [Test]
        public void EqualsShouldReturnTrueWhenComparingComplexNodesWithSameValues()
        {
            // Arrange
            Id i1 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))
            Id i2 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))

            // Act & Assert
            i1.Equals(i2).ShouldBeTrue();
        }

        [Test]
        public void EqualsShouldReturnFalseWhenComparingComplexNodesWithDifferentValues()
        {
            // Arrange
            Id i1 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))
            Id i2 = new Id.Node(new Id.Node(0, new Id.Node(1, 1)), new Id.Node(1, 0)); // ((0,(1,1)),(1,0))

            // Act & Assert
            i1.Equals(i2).ShouldBeFalse();
        }

        [Test]
        public void GetHashCodeShouldMatchWhenLeafsHaveSameValues()
        {
            // Arrange
            // Arrange
            Id i1 = new Id.Leaf(0);
            Id i2 = new Id.Leaf(0);

            // Act
            int hash1 = i1.GetHashCode();
            int hash2 = i2.GetHashCode();

            // Assert
            hash1.ShouldBe(hash2);
        }

        [Test]
        public void GetHashCodeShouldNotMatchWhenLeafsHaveDifferentValues()
        {
            // Arrange
            // Arrange
            Id i1 = new Id.Leaf(0);
            Id i2 = new Id.Leaf(1);

            // Act
            int hash1 = i1.GetHashCode();
            int hash2 = i2.GetHashCode();

            // Assert
            hash1.ShouldNotBe(hash2);
        }

        [Test]
        public void GetHashCodeShouldMatchWhenNodesHaveSameValues()
        {
            // Arrange
            Id i1 = new Id.Node(0, 1);
            Id i2 = new Id.Node(0, 1);

            // Act
            int hash1 = i1.GetHashCode();
            int hash2 = i2.GetHashCode();

            // Assert
            hash1.ShouldBe(hash2);
        }

        [Test]
        public void GetHashCodeShouldNotMatchWhenNodesHaveDifferentValues()
        {
            // Arrange
            Id i1 = new Id.Node(0, 0);
            Id i2 = new Id.Node(0, 1);

            // Act
            int hash1 = i1.GetHashCode();
            int hash2 = i2.GetHashCode();

            // Assert
            hash1.ShouldNotBe(hash2);
        }

        [Test]
        public void GetHashCodeShouldMatchWhenComplexNodesHaveSameValues()
        {
            // Arrange
            Id i1 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))
            Id i2 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))

            // Act
            int hash1 = i1.GetHashCode();
            int hash2 = i2.GetHashCode();

            // Assert
            hash1.ShouldBe(hash2);
        }

        [Test]
        public void GetHashCodeShouldNotMatchWhenComplexNodesHaveDifferentValues()
        {
            // Arrange
            Id i1 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))
            Id i2 = new Id.Node(new Id.Node(0, new Id.Node(1, 1)), new Id.Node(1, 0)); // ((0,(1,1)),(1,0))

            // Act
            int hash1 = i1.GetHashCode();
            int hash2 = i2.GetHashCode();

            // Assert
            hash1.ShouldNotBe(hash2);
        }

        [Test]
        public void SplitShouldReturnNode00WhenLeaf0() // split(0) = (0,0)
        {
            // split(0) = (0,0)
            Id zero = new Id.Leaf(0);
            zero.Split().ShouldBe(new Id.Node(0,0));
        }

        [Test]
        public void SplitShouldReturnNode1001WhenLeaf1() // split(1) = ((1,0),(0,1))
        {
            // split(1) = ((1,0),(0,1))
            Id one = new Id.Leaf(1);
            one.Split().ShouldBe(new Id.Node(new Id.Node(1, 0), new Id.Node(0, 1)));
        }

        [Test]
        public void SplitShouldReturnNode0i0iWhenLeftLeaf0() // split((0,i)) = ((0,i1),(0,i2)) where (i1,i2) = split(i)
        {
            // split((0,1)) = ((0,(1,0)),(0,(0,1)))
            Id id1 = new Id.Node(0, 1);
            id1.Split().ShouldBe(
                new Id.Node(
                    new Id.Node(0, new Id.Node(1, 0)),
                    new Id.Node(0, new Id.Node(0, 1))
                )
            );

            // split((0,(0,1))) = ((0,(0,(1,0))),(0,(0,(0,1))))
            Id id2 = new Id.Node(0, new Id.Node(0, 1));
            id2.Split().ShouldBe(
                new Id.Node(
                    new Id.Node(0, new Id.Node(0, new Id.Node(1, 0))),
                    new Id.Node(0, new Id.Node(0, new Id.Node(0, 1)))
                ));
        }

        [Test]
        public void SplitShouldReturnNodei0i0WhenRightLeaf0() // split((i,0)) = ((i1,0),(i2,0)) where (i1,i2) = split(i)
        {
            // split((1,0)) = (((1,0),0),((0,1),0))
            Id id1 = new Id.Node(1, 0);
            id1.Split().ShouldBe(
                new Id.Node(
                    new Id.Node(new Id.Node(1, 0), 0),
                    new Id.Node(new Id.Node(0, 1), 0)
                )
            );

            // split(((1,0),0)) = ((((1,0),0),0),(((0,1),0),0))
            Id id2 = new Id.Node(new Id.Node(1, 0), 0);
            id2.Split().ShouldBe(
                new Id.Node(
                    new Id.Node(new Id.Node(new Id.Node(1, 0), 0), 0),
                    new Id.Node(new Id.Node(new Id.Node(0, 1), 0), 0)
                )
            );
        }

        [Test]
        public void SplitShouldReturnNodei00iWhenLeftAndRightNodes() // split((i1,i2) = ((i1,0),(0,i2))
        {
            // split(((0,1),(1,0))) = (((0,1),0),((0,(1,0)))
            Id id = new Id.Node(new Id.Node(0,1), new Id.Node(1,0));
            id.Split().ShouldBe(
                new Id.Node(
                    new Id.Node(new Id.Node(0, 1), 0),
                    new Id.Node(0, new Id.Node(1, 0))
                )
            );
        }

        [Test]
        public void SumShouldReturniWhenLeftLeaf0() // sum(0,i) = i
        {
            Id sum1 = Id.Sum(0, 0);
            sum1.ShouldBe(new Id.Leaf(0));

            Id sum2 = Id.Sum(0, 1);
            sum2.ShouldBe(new Id.Leaf(1));

            Id sum3 = Id.Sum(0, new Id.Node(1, 0));
            sum3.ShouldBe(new Id.Node(1, 0));

            Id sum4 = Id.Sum(0, new Id.Node(0, 1));
            sum4.ShouldBe(new Id.Node(0, 1));
        }

        [Test]
        public void SumShouldReturniWhenRightLeaf0() // sum(i,0) = i
        {
            Id sum1 = Id.Sum(0, 0);
            sum1.ShouldBe(new Id.Leaf(0));

            Id sum2 = Id.Sum(1, 0);
            sum2.ShouldBe(new Id.Leaf(1));

            Id sum3 = Id.Sum(new Id.Node(1, 0), 0);
            sum3.ShouldBe(new Id.Node(1, 0));

            Id sum4 = Id.Sum(new Id.Node(0, 1), 0);
            sum4.ShouldBe(new Id.Node(0, 1));
        }

        [Test]
        public void SumShouldReturnSuml1l2Sumr1r2WhenLeftAndRightNodes() // sum((l1,r1),(l2,r2)) = norm(sum(l1,l2),sum(r1,r2))
        {
            Id sum1 = Id.Sum(new Id.Node(0, 1), new Id.Node(0, 1));
            sum1.ShouldBe(new Id.Node(0, 1));

            Id sum2 = Id.Sum(new Id.Node(0, 1), new Id.Node(1, 0));
            sum2.ShouldBe(new Id.Leaf(1)); // Id.Node(1,1) normalizes to 1

            Id sum3 = Id.Sum(new Id.Node(1, 0), new Id.Node(1, 0));
            sum3.ShouldBe(new Id.Node(1, 0));

            Id sum4 = Id.Sum(new Id.Node(1, 0), new Id.Node(0, 1));
            sum4.ShouldBe(new Id.Leaf(1)); // Id.Node(1,1) normalizes to 1
        }

        [Test]
        public void SumShouldReturnNode10WhenLeftNode100AndRightNode010()
        {
            // Arrange
            Id i1 = new Id.Node(new Id.Node(1, 0), 0);
            Id i2 = new Id.Node(new Id.Node(0, 1), 0);

            // Act
            Id sum = i1.Sum(i2);

            // Assert
            sum.ShouldBe(new Id.Node(1, 0));
        }

        [Test]
        public void ImplicitConversionOperatorShouldReturnLeafWhenInteger()
        {
            Id i1 = new Id.Leaf(1);
            Id i2 = 1;

            i1.Equals(i2).ShouldBeTrue();
        }

        [Test]
        public void ImplicitConversionOperatorShouldReturnNodeWhenTuple()
        {
            // Arrange
            Id i1 = new Id.Node(new Id.Node(0, new Id.Node(1, 0)), new Id.Node(1, 0)); // ((0,(1,0)),(1,0))
            Id i2 = ((0,(1,0)),(1,0)); // combo of C#7 tuples and implicit conversion operator is wow!

            // Act & Assert
            i1.Equals(i2).ShouldBeTrue();
        }
    }
}
