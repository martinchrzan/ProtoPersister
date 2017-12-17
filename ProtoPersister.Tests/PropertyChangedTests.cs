using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proto.Tests
{
    [TestClass]
    public class PropertyChangedTests
    {
        [TestMethod]
        public void PropertyChangedIsRisenOnUndoOperation()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");
            persister.CommitCurrentState("1");

            persister.TrackedObject.Age = 5;
            int propertyChangedCount = 0;
            persister.TrackedObject.PropertyChanged += (e, v) =>
            {
                propertyChangedCount++;
            };

            persister.Undo();

            Assert.AreEqual(1, propertyChangedCount);
        }

        [TestMethod]
        public void PropertyChangedIsRisenOnUndoOperationAsManyTimeAsNumberOfChanges()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");
            persister.CommitCurrentState("1");

            persister.TrackedObject.Age = 5;
            persister.TrackedObject.Name = "Jack";

            int propertyChangedCount = 0;
            persister.TrackedObject.PropertyChanged += (e, v) =>
            {
                propertyChangedCount++;
            };

            persister.Undo();

            Assert.AreEqual(2, propertyChangedCount);
        }

        public void PropertyChangedIsRisenOnChangedArray()
        {
            var persister = TestsHelper.GetPersisterWithArray(3, "Jon", 10);
            persister.CommitCurrentState("1");
            
            persister.TrackedObject.TrackingArray[1].Name = "A";



        }
    }
}
