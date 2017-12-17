using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace Proto.Tests
{
    [TestClass]
    public class PersisterTests
    {
        [TestMethod]
        public void SerializeAndDeserializeSimpleObject()
        {
            var trackedObject = new TrackingObject()
            {
                Name = "Test1",
                Age = 20
            };

            var settings = new PersisterSettings(@"D:\test.txt");
            var persister = new Persister<TrackingObject>(settings);
            
            persister.Attach(trackedObject);

            persister.Save();

            var loaded = Persister<TrackingObject>.Load(settings, @"D:\test.txt");

            Assert.AreEqual(loaded.TrackedObject.Age, trackedObject.Age);
            Assert.AreEqual(loaded.TrackedObject.Name, trackedObject.Name);
        }

        [TestMethod]
        public void SerializeAndDeserializeComplexObject()
        {
            var trackedObject = new TrackingObject()
            {
                Name = "Test1",
                Age = 20
            };

            var trackedObject2 = new TrackingObject()
            {
                Name = "Test2",
                Age = 10
            };

            var trackedObject3 = new TrackingObject()
            {
                Name = "Test3",
                Age = 5
            };

            var complexTrackedObject = new ComplexTrackingObject()
            {
                TrackingObjects = new ObservableCollection<TrackingObject>() { trackedObject },
                TrackingList = new System.Collections.Generic.List<TrackingObject>() { trackedObject, trackedObject2 },
                TrackingArray = new TrackingObject[2] { trackedObject2, trackedObject3 },

                Name = "Test1",
                NameChar = 'A',
                Cash = 12.3m,
                Age = 12,
                AgeUInt = 13,
                AgeFloat = 12.1f,
                AgeDouble = 12.3,
                AgeLong = -1234466456,
                AgeULong = 12315155435,
                AgeByte = 25,
                AgeSByte = 15,
                AgeShort = 16,
                AgeUShort = 17,
                True = true,

                DateTime = new DateTime(123456),
                TimeSpan = new TimeSpan(444),
                ID = new Guid("5C60F693-BEF5-E011-A485-80EE7300C695"),

                TestEnum = ComplexTrackingObject.MyEnum.test2,

                Structure = new ComplexTrackingObject.MyStructure() { Age = 55, Name = "haha" }
            };

            var settings = new PersisterSettings(@"D:\test.txt");
            var persister = new Persister<ComplexTrackingObject>(settings);

            persister.Attach(complexTrackedObject);
            
            persister.Save();

            var loaded = Persister<ComplexTrackingObject>.Load(settings, @"D:\test.txt");

            Assert.AreEqual(loaded.TrackedObject.TrackingObjects.Count, 1);
            Assert.AreEqual(loaded.TrackedObject.TrackingObjects[0].Name, trackedObject.Name);
            Assert.AreEqual(loaded.TrackedObject.TrackingObjects[0].Age, trackedObject.Age);

            Assert.AreEqual(loaded.TrackedObject.TrackingList.Count, 2);
            Assert.AreEqual(loaded.TrackedObject.TrackingList[0].Name, trackedObject.Name);
            Assert.AreEqual(loaded.TrackedObject.TrackingList[0].Age, trackedObject.Age);
            Assert.AreEqual(loaded.TrackedObject.TrackingList[1].Name, trackedObject2.Name);
            Assert.AreEqual(loaded.TrackedObject.TrackingList[1].Age, trackedObject2.Age);
            
            Assert.AreEqual(loaded.TrackedObject.TrackingArray.Length, 2);
            Assert.AreEqual(loaded.TrackedObject.TrackingArray[0].Name, trackedObject2.Name);
            Assert.AreEqual(loaded.TrackedObject.TrackingArray[0].Age, trackedObject2.Age);
            Assert.AreEqual(loaded.TrackedObject.TrackingArray[1].Name, trackedObject3.Name);
            Assert.AreEqual(loaded.TrackedObject.TrackingArray[1].Age, trackedObject3.Age);
            
            Assert.AreEqual(loaded.TrackedObject.Name, "Test1");
            Assert.AreEqual(loaded.TrackedObject.NameChar, 'A');
            Assert.AreEqual(loaded.TrackedObject.Cash, 12.3m);
            Assert.AreEqual(loaded.TrackedObject.Age, 12);
            Assert.AreEqual(loaded.TrackedObject.AgeUInt, (uint)13);
            Assert.AreEqual(loaded.TrackedObject.AgeFloat, 12.1f);
            Assert.AreEqual(loaded.TrackedObject.AgeDouble, 12.3);
            Assert.AreEqual(loaded.TrackedObject.AgeLong, -1234466456);
            Assert.AreEqual(loaded.TrackedObject.AgeULong, (ulong)12315155435);
            Assert.AreEqual(loaded.TrackedObject.AgeByte, (byte)25);
            Assert.AreEqual(loaded.TrackedObject.AgeSByte, (sbyte)15);
            Assert.AreEqual(loaded.TrackedObject.AgeShort, 16);
            Assert.AreEqual(loaded.TrackedObject.AgeUShort, (ushort)17);
            Assert.AreEqual(loaded.TrackedObject.True, true);

            Assert.AreEqual(loaded.TrackedObject.DateTime, new DateTime(123456));
            Assert.AreEqual(loaded.TrackedObject.TimeSpan, new TimeSpan(444));
            Assert.AreEqual(loaded.TrackedObject.ID, new Guid("5C60F693-BEF5-E011-A485-80EE7300C695"));


            Assert.AreEqual(loaded.TrackedObject.TestEnum, ComplexTrackingObject.MyEnum.test2);

            Assert.AreEqual(loaded.TrackedObject.Structure.Age, 55);
            Assert.AreEqual(loaded.TrackedObject.Structure.Name, "haha");

        }

        [TestMethod]
        public void SeralizeAndDeserializeOnlyPublicProperties()
        {

        }
    }
}
