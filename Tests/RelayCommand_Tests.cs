using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tests
{
    [TestClass]
    public class RelayCommand_Tests
    {
        [TestMethod]
        public void CtorOne_WhenExecuteArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            // act
            try
            {
                var c = new Shiva.RelayCommand(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "execute");
            }
        }

        [TestMethod]
        public void CtorTwo_WhenExecuteArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            // act
            try
            {
                var c = new Shiva.RelayCommand(null, o => true);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "execute");
            }
        }

        [TestMethod]
        public void CanExecute_Always_ShouldFunctionCorrectly()
        {
            // arrange
            Action<object> act = o => { };
            Predicate<object> can = o => { return (bool)o; };
            var c1 = new Shiva.RelayCommand(act);
            var c2 = new Shiva.RelayCommand(act, can);

            // act
            // assert
            Assert.AreEqual(c1.CanExecute(8), true);
            Assert.AreEqual(c1.CanExecute(false), true);
            Assert.AreEqual(c1.CanExecute(true), true);
            Assert.AreEqual(c2.CanExecute(false), false);
            Assert.AreEqual(c2.CanExecute(true), true);
        }

        [TestMethod]
        public void Execute_Always_ShouldFunctionCorrectly()
        {
            // arrange
            bool x = false;
            var c = new Shiva.RelayCommand(o => { x = true; });

            // act
            c.Execute(null);

            // assert
            Assert.IsTrue(x);
        }

        [TestMethod]
        public void CanExecuteChanged_Always_ShouldRedirectToCommandManagerRequerySuggested()
        {
            // arrange
            bool triggered;
            EventHandler handler = new EventHandler((o, args) => { triggered = true; });
            var c = new Shiva.RelayCommand(o => { });

            // act
            triggered = false;
            c.CanExecuteChanged += handler;
            CommandManager.InvalidateRequerySuggested();
            // Ensure the invalidate is processed
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
            GC.KeepAlive(handler);
            // assert
            Assert.IsTrue(triggered);

            // act
            triggered = false;
            c.CanExecuteChanged -= handler;
            CommandManager.InvalidateRequerySuggested();
            // Ensure the invalidate is processed
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
            GC.KeepAlive(handler);
            // assert
            Assert.IsTrue(!triggered);
        }
    }
}
