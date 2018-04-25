# ProtoPersister
Persistence framework build on top of protobuf-net 

## Basic usage

### Create your class
```csharp
    public class MyDataClass
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Anniversary { get; set; }
    }
```
### Setup ProtoPersister, attach to your data class and save
```csharp
    var persister = new Persister<MyDataClass>(
	                // the file extension does not matter, you can came up with your own one
                    new PersisterSettings("C:\\myDataFile.proto")
                    {
                        MaxHistorySteps = 100,
	                      // this will allow for undo/redo without notifying ProtoPersister explicitly about changes
                        TrackChanges = true,
                        AutoSave = new AutoSaveSettings()
                        {
                            AutoSaveFilePath = "C:\\myDataAutoSaveFile.proto",
                            Enabled = true,
                            SaveEvery = TimeSpan.FromMinutes(5)
                        }
                    }
                );

    var myDataClass = new MyDataClass() { Name = "Jack", Age = 20, Anniversary = DateTime.Today };
    persister.Attach(myDataClass);
    
    // save data
    persister.Save();
```
### Undo/redo
```csharp
    myDataClass.Age = 10;

    persister.Undo(); // -> now myDataClass = 20 
    persister.Redo() // -> now myDataClass = 10
```
### Using a manual tracking of changes (`TrackChanges = false`):
```csharp
    // change your object
    persister.CommitCurrentState("1"); // adds a new version of your class into the history stack
```
### Load your data from a file
```csharp
    var persister = Persister<MyDataClass>.Load(new PersisterSettings("C:\\myDataFile.proto"), "C:\\myDataFile.proto");
    var myDataClass = persister.TrackedObject;
```
