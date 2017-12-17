using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proto.Tests
{
    [TestClass]
    public class UndoRedoTests
    {
        [TestMethod]
        public void CanReturnToPreviousStateWithUndo()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            // commit current state before change
            persister.CommitCurrentState("1");

            persister.TrackedObject.Age = 16;
            persister.TrackedObject.Name = "Jack";

            var historyId = persister.Undo();

            Assert.AreEqual("1", historyId);
            Assert.AreEqual(15, persister.TrackedObject.Age);
            Assert.AreEqual("John", persister.TrackedObject.Name);
        }

        [TestMethod]
        public void CanRedoToTheLatestStateAfterUndo()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            // commit current state before change
            persister.CommitCurrentState("1");

            persister.TrackedObject.Age = 16;
            persister.TrackedObject.Name = "Jack";

            var historyId = persister.Undo();
            historyId = persister.Redo();

            Assert.IsTrue(string.IsNullOrEmpty(historyId));
            Assert.AreEqual(16, persister.TrackedObject.Age);
            Assert.AreEqual("Jack", persister.TrackedObject.Name);
        }

        [TestMethod]
        public void CanUndoIsTrueAfterCommitData()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            Assert.AreEqual(persister.CanUndo, false);

            persister.TrackedObject.Age = 5;
            persister.CommitCurrentState("1");
            Assert.AreEqual(true, persister.CanUndo);
        }

        [TestMethod]
        public void CanRedoIsTrueAfterUndoData()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            Assert.AreEqual(persister.CanRedo, false);

            persister.TrackedObject.Age = 5;
            persister.CommitCurrentState("1");
            persister.Undo();

            Assert.AreEqual(true, persister.CanRedo);
        }

        [TestMethod]
        public void RedoHistoryIsDestroyedWhenCommitingInHistory()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            persister.TrackedObject.Age = 1;
            persister.CommitCurrentState("1");
            persister.Undo();

            Assert.AreEqual(persister.CanRedo, true);

            persister.TrackedObject.Age = 2;
            persister.CommitCurrentState("2");

            Assert.AreEqual(false, persister.CanRedo);
        }

        [TestMethod]
        public void CanUndoChangedEventIsFiredWhenDataAreCommited()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            var numberOfEventFires = 0;
            persister.CanUndoChanged += (e,o) =>
            {
                numberOfEventFires++;
            };

            persister.CommitCurrentState("1");

            Assert.AreEqual(1, numberOfEventFires);
        }

        [TestMethod]
        public void CanUndoChangedEventIsFiredTwiceWhenUndoIsAvailableAndLaterUnavailable()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            var numberOfEventFires = 0;
            persister.CanUndoChanged += (e, o) =>
            {
                numberOfEventFires++;
            };

            persister.CommitCurrentState("1");
            persister.Undo();

            Assert.AreEqual(2, numberOfEventFires);
        }

        [TestMethod]
        public void CanRedoEventIsFiredWhenRedoBecameAvailable()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            var numberOfEventFires = 0;
            persister.CanRedoChanged += (e, o) =>
            {
                numberOfEventFires++;
            };

            persister.CommitCurrentState("1");
            persister.Undo();

            Assert.AreEqual(1, numberOfEventFires);
        }

        [TestMethod]
        public void CanRedoEventIsFiredTwiceWhenRedoBecameAvailableAndUnavailable()
        {
            var persister = TestsHelper.GetPersisterWithSimpleObject(15, "John");

            var numberOfEventFires = 0;
            persister.CanRedoChanged += (e, o) =>
            {
                numberOfEventFires++;
            };

            persister.CommitCurrentState("1");
            persister.Undo();
            persister.Redo();

            Assert.AreEqual(2, numberOfEventFires);
        }

        [TestMethod]
        public void UndoWorksWithArray()
        {
            var persister = TestsHelper.GetPersisterWithArray(3, "Jon", 10);
            persister.CommitCurrentState("1");

            persister.TrackedObject.TrackingArray[1].Name = "A";

            persister.Undo();

            Assert.AreEqual("Jon", persister.TrackedObject.TrackingArray[1].Name);
        }

        [TestMethod]
        public void UndoWorksWithIncreasingArray()
        {
            var originalNumberOfItems = 3;
            var persister = TestsHelper.GetPersisterWithList(originalNumberOfItems, "Jon", 10);
            persister.CommitCurrentState("1");

            persister.TrackedObject.TrackingList.Add(new TrackingObject());

            persister.Undo();

            Assert.AreEqual(originalNumberOfItems, persister.TrackedObject.TrackingList.Count);
        }

        [TestMethod]
        public void UndoWorksWithSmallerArray()
        {
            var originalNumberOfItems = 3;
            var persister = TestsHelper.GetPersisterWithList(originalNumberOfItems, "Jon", 10);
            persister.CommitCurrentState("1");

            persister.TrackedObject.TrackingList.RemoveAt(0);

            persister.Undo();

            Assert.AreEqual(originalNumberOfItems, persister.TrackedObject.TrackingList.Count);
        }
    }
}
