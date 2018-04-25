using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;

namespace ProtoPersister
{
    public class Persister<T> : IDisposable
    {
        private bool _propertyChangedEventsAttached;
        private PersisterSettings Settings { get; set; }
        
        private INotifyPropertyChanged _notifyPropertyChangedTrackedObject;

        private ObjectToFileSerializer _serializer;
        private IUndoRedoHandler<T> _undoRedoHandler;

        private readonly object _lock = new object();
        private Timer _autosaveTimer;
        
        private string _currentHistoryId;

        public Persister(PersisterSettings settings)
        {
            Settings = settings;
            ValidateSettings();
            
            _undoRedoHandler = new UndoRedoHandler<T>(Settings.MaxHistorySteps);
            _undoRedoHandler.CanRedoChanged += UndoRedoHandler_CanRedoChanged;
            _undoRedoHandler.CanUndoChanged += UndoRedoHandler_CanUndoChanged;

            _serializer = new ObjectToFileSerializer();
            InitializeAutoSave();
        }
        
        private Persister(PersisterSettings settings, string pathToFile, Type objectType) : this(settings)
        {
            TrackedObject = (T)_serializer.Deserialize(pathToFile, objectType);
        }

        public T TrackedObject { get; private set; }

        /// <summary>
        /// Attach object to be tracked for changes or save it later
        /// </summary>
        /// <param name="trackedObject"></param>
        public void Attach(T trackedObject)
        {
            if (Settings.TrackChanges && trackedObject is INotifyPropertyChanged)
            {
                _notifyPropertyChangedTrackedObject = trackedObject as INotifyPropertyChanged;

                _notifyPropertyChangedTrackedObject.PropertyChanged += TrackedObjectPropertyChanged;
          
                foreach (var item in trackedObject.GetType().GetProperties())
                {
                    if(typeof(INotifyPropertyChanged).IsAssignableFrom(item.PropertyType))
                    {
                        var propertyValue = item.GetValue(trackedObject);
                        if(propertyValue != null)
                        {
                            (propertyValue as INotifyPropertyChanged).PropertyChanged += TrackedObjectPropertyChanged;
                        }
                    }
                }

                _propertyChangedEventsAttached = true;
            }
            
            TrackedObject = trackedObject;
        }
        
        public bool Save()
        {
            _serializer.Serialize(TrackedObject, Settings.FilePath);   
            return true;
        }

        /// <summary>
        /// Loads data from provided path into a new persister class with loaded object from given path
        /// </summary>
        /// <param name="settings">Persister settings</param>
        /// <param name="pathToFile">Path to file to be used to load data</param>
        /// <returns>New Persister class with loaded object</returns>
        /// <exception cref="PersisterException">Throws on incorrect parameters</exception>
        public static Persister<T> Load(PersisterSettings settings, string pathToFile)
        {
            if(settings == null)
            {
                throw new PersisterException("settings parameter cannot be null!");
            }
            if(string.IsNullOrEmpty(pathToFile))
            {
                throw new PersisterException("path to file parameter cannot be null or empty!"); 
            }
            
            return new Persister<T>(settings, pathToFile, typeof(T));
        }

        /// <summary>
        /// Restore the state of a tracked object - one step in a history back
        /// </summary>
        /// <returns>Returns history id</returns>
        public string Undo()
        {
            lock (_lock)
            {
                return DoHistoryOperation(_undoRedoHandler.Undo);
            }
        }

        /// <summary>
        ///Restore the state of a tracked object - one step in a history forward 
        /// </summary>
        /// <returns>Returns history id</returns>
        public string Redo()
        {
            lock (_lock)
            {
                return DoHistoryOperation(_undoRedoHandler.Redo);
            }
        }
        
        public void CommitCurrentState(string historyId)
        {
            lock (_lock)
            {
                _undoRedoHandler.Push(_serializer.DeepClone<T>(TrackedObject), historyId);
            }
        }
        
        public bool CanUndo => _undoRedoHandler.CanUndo();

        public bool CanRedo => _undoRedoHandler.CanRedo();

        public event EventHandler CanUndoChanged;

        public event EventHandler CanRedoChanged;
        
        public void Dispose()
        {
            DetachEvents();
            _autosaveTimer?.Dispose();
        }

        ~Persister()
        {
            DetachEvents();
        }
        
        private void ValidateSettings()
        {
            if(Settings == null)
            {
                throw new PersisterException("Settings cannot be null.");
            }

            if(string.IsNullOrEmpty(Settings.FilePath))
            {
                throw new PersisterException("File path cannot be null or empty.");
            }

            if (Settings.AutoSave != null &&
                Settings.AutoSave.Enabled == true &&
               (string.IsNullOrEmpty(Settings.AutoSave.AutoSaveFilePath) ||
                Settings.AutoSave.SaveEvery == null ||
                Settings.AutoSave.SaveEvery.Milliseconds == 0))
            {
                throw new PersisterException("AutoSave is enabled but not configured properly, check its properties.");
            }
        }
        
        private void UndoRedoHandler_CanUndoChanged(object sender, EventArgs e)
        {
            CanUndoChanged?.Invoke(null, EventArgs.Empty);
        }

        private void UndoRedoHandler_CanRedoChanged(object sender, EventArgs e)
        {
            CanRedoChanged?.Invoke(null, EventArgs.Empty);
        }

        private void InitializeAutoSave()
        {
            if (Settings.AutoSave == null)
            {
                // nothing to do
                return;
            }
            
            _autosaveTimer = new Timer(AutoSaveTimerCallback, null, Settings.AutoSave.SaveEvery.Milliseconds, Timeout.Infinite);
        }

        private void AutoSaveTimerCallback(Object state)
        {
            _serializer.Serialize(TrackedObject, Settings.AutoSave.AutoSaveFilePath);
            //schedule new auto save event
            _autosaveTimer.Change(Settings.AutoSave.SaveEvery.Milliseconds, Timeout.Infinite);
        }

        private void TrackedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _undoRedoHandler.Push(_serializer.DeepClone<T>(TrackedObject), Guid.NewGuid().ToString());
        }
        
        private string DoHistoryOperation(Func<T, string, Tuple<T, string>> historyAction)
        {
            var result = historyAction.Invoke(_serializer.DeepClone<T>(TrackedObject), _currentHistoryId);
            TrackedObject.PopulateWithDataFrom(result.Item1);
            _currentHistoryId = result.Item2;
            return result.Item2;
        }
        
        private void DetachEvents()
        {
            if (_propertyChangedEventsAttached)
            {
                _notifyPropertyChangedTrackedObject.PropertyChanged -= TrackedObjectPropertyChanged;
                _propertyChangedEventsAttached = false;
            }

            _undoRedoHandler.CanRedoChanged -= UndoRedoHandler_CanRedoChanged;
            _undoRedoHandler.CanUndoChanged -= UndoRedoHandler_CanUndoChanged;
        }
    }
}
