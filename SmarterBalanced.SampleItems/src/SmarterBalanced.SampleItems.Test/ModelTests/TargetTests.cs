using SmarterBalanced.SampleItems.Dal.Providers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmarterBalanced.SampleItems.Test.ModelTests
{
    public class TargetTests
    {
        Target target;

        public TargetTests()
        {
            target = Target.Create(
                desc: "SHORT NAME: Long description",
                id: "1-2",
                idLabel: "1",
                subject: "ELA",
                claim: "1");
        }
        [Fact]
        public void TestTargetCreate()
        {
            Assert.Equal(target.Descripton, "Long description");
            Assert.Equal(target.Name, "1. Short Name");
            Assert.Equal(target.Id, "1-2");
            Assert.Equal(target.IdLabel, "1");
            Assert.Equal(target.ClaimId, "1");
            Assert.Equal(target.Subject, "ELA");
            Assert.Equal(target.NameHash, target.GetHashCode());
        }

        [Fact]
        public void TestTargetWithDescription()
        {
            int oldHash = target.NameHash;
            var newTarget = target.WithDescription("ANOTHER DESC: Blah blah blah!");

            Assert.Equal(newTarget.Descripton, "Blah blah blah!");
            Assert.Equal(newTarget.Name, "1. Another Desc");
            Assert.Equal(newTarget.Id, "1-2");
            Assert.Equal(newTarget.IdLabel, "1");
            Assert.Equal(newTarget.ClaimId, "1");
            Assert.Equal(newTarget.Subject, "ELA");
            Assert.Equal(newTarget.NameHash, newTarget.GetHashCode());
            Assert.NotEqual(newTarget.NameHash, target.NameHash);
        }
    }
}
