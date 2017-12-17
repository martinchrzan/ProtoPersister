using System;

namespace Proto
{
    public class PersisterSettings
    {
        public PersisterSettings(string filePath)
        {
            FilePath = filePath;
            MaxHistorySteps = 100;
        }

        /// <summary>
        /// Enable to support undo/redo functionality
        /// </summary>
        /// <returns></returns>
        public bool TrackChanges { get; set; }

        /// <summary>
        /// Maximal number of undo actions, default is 100.
        /// </summary>
        public int MaxHistorySteps { get; set; }

        /// <summary>
        /// Full path to a file of a serialized object
        /// </summary>
        public string FilePath { get; private set; }

        public AutoSaveSettings AutoSave { get; set; }
    }

    public class AutoSaveSettings
    {
        public string AutoSaveFilePath { get; set; }

        public bool Enabled { get; set; }

        public TimeSpan SaveEvery { get; set; }
    }
}