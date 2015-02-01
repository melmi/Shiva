using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ValidationRule_Tests
    {
        [TestMethod]
        public void Ctor_WhenRuleArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            // act
            try
            {
                var vr = new Shiva.ValidationRule<string>(null, "something");
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "rule");
            }
        }

        [TestMethod]
        public void Ctor_WhenMessageArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            // act
            try
            {
                var vr = new Shiva.ValidationRule<string>(x => true, null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "message");
            }
        }

        [TestMethod]
        public void Ctor_WhenArgumentsAreOk_ShouldCreateCorrectly()
        {
            // arrange
            // act
            Func<string, bool> rule = s => true;
            var vr = new Shiva.ValidationRule<string>(rule, "something");

            // assert
            Assert.AreEqual(vr.Message, "something");
            Assert.AreEqual(vr.Rule, rule);
        }

        [TestMethod]
        public void Validate_WhenArgumentCouldNotBeCasted_ShouldThrowArgumentException()
        {
            // arrange
            Func<string, bool> rule = s => s == "good";
            var vr = new Shiva.ValidationRule<string>(rule, "something");

            // act
            try
            {
                vr.Validate(4);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "value");
            }
        }

        [TestMethod]
        public void Validate_WhenArgumentsAreOk_ShouldActCorrectly()
        {
            // arrange
            Func<string, bool> rule = s => s == "good";
            var vr = new Shiva.ValidationRule<string>(rule, "something");

            // act
            // assert
            Assert.IsTrue(vr.Validate("good"));
            Assert.IsTrue(!vr.Validate("bad"));
            Assert.IsTrue(!vr.Validate(null));
        }
    }
}
