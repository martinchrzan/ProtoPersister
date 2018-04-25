using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ProtoPersister
{
    internal static class ObjectDataPopulator
    {
        public static void PopulateWithDataFrom<T>(this T originalData, T populateWith)
        {
            if (populateWith == null)
            {
                return;
            }

            var currentType = typeof(T);
            if(currentType == typeof(object))
            {
                // when called without specifying generic type
                currentType = originalData.GetType();
            }

            foreach (var prop in currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.PropertyType.IsSimpleType())
                {
                    var newValue = prop.GetValue(populateWith);
                    var oldValue = prop.GetValue(originalData);
                    if ((oldValue != null && newValue != null) &&
                        ((oldValue == null && newValue != null) ||
                        (newValue == null && oldValue != null) ||
                        !newValue.Equals(oldValue)))
                    {
                        // this will raise property changed, we want to change it only when properties are different
                        prop.SetValue(originalData, newValue);
                    }
                }
                else
                {
                    ProcessArray(prop, originalData, populateWith);
                }
            }
        }

        private static void ProcessArray<T>(PropertyInfo prop, T originalData, T populateWith)
        {
            var genericEnumerables = ReflectionHelper.GetGenericIEnumerables(prop.PropertyType);
            if (genericEnumerables.Any())
            {
                if (genericEnumerables.First().IsSimpleType())
                {
                    // just replace all array items
                    ReplaceArray(prop, originalData, populateWith);
                }
                else
                {
                    var newValue = prop.GetValue(populateWith);
                    var oldValue = prop.GetValue(originalData);
                    if (oldValue == null && newValue == null)
                    {
                        return;
                    }

                    if (oldValue == null && newValue != null)
                    {
                        prop.SetValue(originalData, newValue);
                        return;
                    }

                    var oldEnumerable = (IEnumerable)oldValue;
                    var newEnumerable = (IEnumerable)newValue;
                    var newEnumerableEnumerator = newEnumerable.GetEnumerator();

                    var newEnumerableEnumeratorFinished = false;
                    //get to the first item
                    newEnumerableEnumerator.MoveNext();

                    var index = 0;

                    foreach (var oldItem in oldEnumerable)
                    {
                        oldItem.PopulateWithDataFrom(newEnumerableEnumerator.Current);
                        if (!newEnumerableEnumerator.MoveNext())
                        {
                            newEnumerableEnumeratorFinished = true;
                            break;
                        }
                        index++;
                    }

                    if (oldEnumerable is IList)
                    {
                        var listEnumerable = oldEnumerable as IList;

                        if (newEnumerableEnumeratorFinished)
                        {
                            // remove extra items
                            for (int i = index + 1; i < listEnumerable.Count; i++)
                            {
                                if (!listEnumerable.IsFixedSize)
                                {
                                    listEnumerable.RemoveAt(i);
                                }
                                else
                                {
                                    listEnumerable[i] = null;
                                }
                            }
                        }
                        else
                        {
                            if (listEnumerable.IsFixedSize)
                            {
                                listEnumerable[index] = newEnumerableEnumerator.Current;
                                index++;
                            }
                            else
                            {
                                listEnumerable.Add(newEnumerableEnumerator.Current);
                            }

                            while (newEnumerableEnumerator.MoveNext())
                            {
                                if (listEnumerable.IsFixedSize)
                                {
                                    listEnumerable[index] = newEnumerableEnumerator.Current;
                                    index++;
                                }
                                else
                                {
                                    listEnumerable.Add(newEnumerableEnumerator.Current);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ReplaceArray(PropertyInfo property, object originalObject, object newObject)
        {
            var originalArray = property.GetValue(originalObject);
            var newArray = (IEnumerable<Type>)property.GetValue(newObject);

            if (originalArray is ObservableCollection<Type>)
            {
                var observableArray = (originalArray as ObservableCollection<Type>);
                observableArray.Clear();
                foreach(var item in newArray)
                {
                    observableArray.Add(item);
                }
            }
            else
            {
                property.SetValue(originalObject, property.GetValue(newObject));
            }
        }
    }
}
