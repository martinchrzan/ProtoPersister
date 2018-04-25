using System.ComponentModel;

namespace ProtoPersister.Tests
{
    public class TrackingObject : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Notify("Name");
            }
        }

        private int age;

        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                Notify("Age");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
    }
}
