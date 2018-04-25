using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace ProtoPersister.Tests
{
    /// <summary>
    /// Contains all basic build in types such as reference types, value types, enums, struct, arrays, 
    /// </summary>
    public class ComplexTrackingObject : INotifyPropertyChanged
    {
        #region Arrays
        public ObservableCollection<TrackingObject> TrackingObjects { get; set; }
        public List<TrackingObject> TrackingList { get; set; }
        public TrackingObject[] TrackingArray { get; set; }
        #endregion

        #region Build in types
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Notify("Name");
            }
        }

        private char _nameChar;
        public char NameChar
        {
            get { return _nameChar; }
            set
            {
                _nameChar = value;
                Notify("NameChar");
            }
        }

        private decimal _cash;
        public decimal Cash
        {
            get { return _cash; }
            set
            {
                _cash = value;
                Notify("Cash");
            }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                Notify("Age");
            }
        }

        private uint _ageUInt;
        public uint AgeUInt
        {
            get { return _ageUInt; }
            set
            {
                _ageUInt = value;
                Notify("AgeUInt");
            }
        }

        private float _ageFloat;
        public float AgeFloat
        {
            get { return _ageFloat; }
            set
            {
                _ageFloat = value;
                Notify("AgeFloat");
            }
        }

        private double _ageDouble;
        public double AgeDouble
        {
            get { return _ageDouble; }
            set
            {
                _ageDouble = value;
                Notify("AgeDouble");
            }
        }

        private long _ageLong;
        public long AgeLong
        {
            get { return _ageLong; }
            set
            {
                _ageLong = value;
                Notify("AgeLong");
            }
        }

        private ulong _ageULong;
        public ulong AgeULong
        {
            get { return _ageULong; }
            set
            {
                _ageULong = value;
                Notify("AgeULong");
            }
        }

        private byte _ageByte;
        public byte AgeByte
        {
            get { return _ageByte; }
            set
            {
                _ageByte = value;
                Notify("AgeByte");
            }
        }

        private sbyte _ageSByte;
        public sbyte AgeSByte
        {
            get { return _ageSByte; }
            set
            {
                _ageSByte = value;
                Notify("AgeSByte");
            }
        }

        private short _ageShort;
        public short AgeShort
        {
            get { return _ageShort; }
            set
            {
                _ageShort = value;
                Notify("AgeShort");
            }
        }

        private ushort _ageUShort;
        public ushort AgeUShort
        {
            get { return _ageUShort; }
            set
            {
                _ageUShort = value;
                Notify("AgeUShort");
            }
        }

        private bool _true;
        public bool True
        {
            get { return _true; }
            set
            {
                _true = value;
                Notify("True");
            }
        }
        #endregion

        #region Enum
        public enum MyEnum { test1, test2, test3}

        private MyEnum _testEnum;
        public MyEnum TestEnum
        {
            get { return _testEnum; }
            set { _testEnum = value; Notify("TestEnum"); }
        }
        #endregion
        
        #region Struct

        public struct MyStructure
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }

        public MyStructure Structure { get; set; }
        #endregion

        #region Other types
        private DateTime _dateTime;
        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                Notify("DateTime");
            }
        }

        private TimeSpan _timeSpan;
        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                _timeSpan = value;
                Notify("TimeSpan");
            }
        }

        private Guid _id;
        public Guid ID
        {
            get { return _id; }
            set
            {
                _id = value;
                Notify("ID");
            }
        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
