using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class PropertyEx_Tests
    {
        [TestMethod]
        public void Name_WhenSelectorExpressionArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            // act
            try
            {
                Expression<Func<string>> selectorExpression = null;
                Shiva.PropertyEx.Name(selectorExpression);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "selectorExpression");
            }
        }

        int dummy() { return 0; }

        [TestMethod]
        public void Name_WhenSelectorExpressionArgumentIsNotMemberExpression_ThrowArgumentException()
        {
            // arrange
            // act
            try
            {
                Shiva.PropertyEx.Name<int>(() => dummy());
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "selectorExpression");
            }
        }

        public int MyProperty { get; set; }

        [TestMethod]
        public void Name_WhenArgumentsAreOk_ShouldReturnCorrectly()
        {
            // arrange
            // act
            // assert
            Assert.AreEqual(Shiva.PropertyEx.Name(() => MyProperty), "MyProperty");
        }
    }
}
