using System.Collections.Generic;

namespace Proto.Tests
{
    public static class TestsHelper
    {
        public static Persister<TrackingObject> GetPersisterWithSimpleObject(int age, string name)
        {
            TrackingObject trackingObject = new TrackingObject()
            {
                Age = age,
                Name = name
            };

            PersisterSettings settings = new PersisterSettings(@"D:\test.txt");
            Persister<TrackingObject> persister = new Persister<TrackingObject>(settings);
            persister.Attach(trackingObject);

            return persister;
        }

        public static Persister<ComplexTrackingObject> GetPersisterWithArray(int numberOfItems, string defaultName, int defaultAge)
        {
            ComplexTrackingObject trackingObject = new ComplexTrackingObject()
            {
               TrackingArray= new TrackingObject[numberOfItems]
            };

            for (int i = 0; i < numberOfItems; i++)
            {
                trackingObject.TrackingArray[i] = new TrackingObject()
                {
                    Age = defaultAge,
                    Name = defaultName
                };
            }

            PersisterSettings settings = new PersisterSettings(@"D:\test.txt");
            Persister<ComplexTrackingObject> persister = new Persister<ComplexTrackingObject>(settings);
            persister.Attach(trackingObject);

            return persister;
        }

        public static Persister<ComplexTrackingObject> GetPersisterWithList(int numberOfItems, string defaultName, int defaultAge)
        {
            ComplexTrackingObject trackingObject = new ComplexTrackingObject()
            {
                TrackingList = new List<TrackingObject>()
            };

            for (int i = 0; i < numberOfItems; i++)
            {

                trackingObject.TrackingList.Add(new TrackingObject()
                {
                    Age = defaultAge,
                    Name = defaultName
                });
            }

            PersisterSettings settings = new PersisterSettings(@"D:\test.txt");
            Persister<ComplexTrackingObject> persister = new Persister<ComplexTrackingObject>(settings);
            persister.Attach(trackingObject);

            return persister;
        }
    }
}
