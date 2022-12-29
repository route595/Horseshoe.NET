using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Horseshoe.NET.Primitives
{
    /// <summary>
    /// A data structure similar to <see cref="StringValues"/> but with significant limitations, see remarks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This <c>struct</c> is mainly used internally, however, feel free to use it.
    /// </para>
    /// <para>
    /// Note: There are significant limitations in <c>ObjectValues</c> when compared with <see cref="StringValues"/>
    /// that you should be aware of.  
    /// </para>
    /// <para>
    /// First, although <see cref="StringValues"/> extends the <c>IList&lt;string&gt;</c>
    /// interface several of its methods (i.e. the mutatable methods) are not available and appear to be hidden from code completion 
    /// (e.g. <c>Add()</c>, <c>Clear()</c>, etc.).  
    /// </para>
    /// <para>
    /// Second, a <c>string</c> can be implicitly cast to <see cref="StringValues"/>
    /// via implicit operator.  However, <c>object</c> cannot be used in an implicit cast so <c>ObjectValues</c> is not quite as robust.
    /// </para>
    /// </remarks>
    public readonly struct ObjectValues : IList<object>, IReadOnlyList<object>, IEquatable<ObjectValues>, IEquatable<object>, IEquatable<object[]>
    {
        private readonly List<object> _objects;

        /// <summary>
        /// Gets the <see cref="object"/> at the specified index.
        /// </summary>
        /// <param name="index">An index.</param>
        /// <returns>The <see cref="object"/> at the specified index.</returns>
        /// <exception cref="NotImplementedException">If the setter is accessed.</exception>
        public object this[int index] { get => (_objects ?? new List<object>())[index]; set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObjectValues"/>.
        /// </summary>
        public int Count => _objects?.Count ?? 0;

        /// <summary>
        /// Returns <c>true</c>.
        /// </summary>
        public bool IsReadOnly => true;

        ///// <summary>
        ///// Creates a new <c>ObjectValues</c>.  Disabling... passing in a collection of objects often lands here.
        ///// </summary>
        ///// <param name="obj">An <c>object</c>.</param>
        //public ObjectValues(object obj)
        //{
        //    _objects = obj == null 
        //        ? new List<object>()
        //        : new List<object> { obj };
        //}

        /// <summary>
        /// Creates a new <c>ObjectValues</c> from the supplied collection.
        /// </summary>
        /// <param name="coll">A collection of <c>object</c>s.</param>
        public ObjectValues(IEnumerable<object> coll)
        {
            _objects = coll == null
                ? new List<object>()
                : new List<object>(coll);
        }

        /// <summary>
        /// Disabled feature.
        /// </summary>
        /// <param name="item">An item.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disabled feature.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests if <c>item</c> is contained in this <see cref="ObjectValues"/>.
        /// </summary>
        /// <param name="item">An item.</param>
        /// <returns><c>true</c> or <c>false</c></returns>
        public bool Contains(object item)
        {
            return _objects.Contains(item);
        }

        /// <summary>
        /// Copies the entire contents of this <see cref="ObjectValues"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">A compatible one-dimensional array.</param>
        /// <param name="arrayIndex">The specified index of the target array.</param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            _objects.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator the iterates through the backing <c>List</c>.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurance within this <see cref="ObjectValues"/>.
        /// </summary>
        /// <param name="item">An item.</param>
        /// <returns>The zero-based index of the first occurance of <paramref name="item"/>.</returns>
        public int IndexOf(object item)
        {
            return _objects.IndexOf(item);
        }

        /// <summary>
        /// Disabled feature.
        /// </summary>
        /// <param name="index">An index.</param>
        /// <param name="item">An item.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Insert(int index, object item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disabled feature.
        /// </summary>
        /// <param name="item">An item.</param>
        /// <exception cref="NotImplementedException"></exception>
        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disabled feature.
        /// </summary>
        /// <param name="index">An index.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        #region equality stuff

        /// <summary>
        /// Determines whether two specified <see cref="ObjectValues"/> objects have the same values in the same order.
        /// </summary>
        /// <param name="left">The first <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The second <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool Equals(ObjectValues left, ObjectValues right)
        {
            int count = left.Count;

            if (count != right.Count)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="ObjectValues"/> object have the same values.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns><c>true</c> if the value of <paramref name="other"/> is the same as the value of this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(ObjectValues other) => Equals(this, other);

        /// <summary>
        /// Determines whether this instance and a specified object array have the same values.
        /// </summary>
        /// <param name="other">The object array to compare to this instance.</param>
        /// <returns><c>true</c> if the value of <paramref name="other"/> is the same as this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(object[] other) => Equals(this, new ObjectValues(other));

        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to <paramref name="obj"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return Equals(this, ObjectValues.Empty);
            }

            if (obj is object str)
            {
                return Equals(this, str);
            }

            if (obj is object[] array)
            {
                return Equals(this, array);
            }

            if (obj is ObjectValues objectValues)
            {
                return Equals(this, objectValues);
            }

            return false;
        }

        /// <summary>
        /// Gets a hashcode for this <see cref="ObjectValues"/>.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return -2046455862 + EqualityComparer<List<object>>.Default.GetHashCode(_objects);
        }

        #endregion

        #region static equality stuff

        /// <summary>
        /// Determines whether the specified <see cref="object"/> and <see cref="ObjectValues"/> objects have the same values.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c>. If <paramref name="left"/> is <c>null</c>, the method returns <c>false</c>.</returns>
        public static bool Equals(object left, ObjectValues right) => Equals(From(left), right);

        /// <summary>
        /// Determines whether the specified <see cref="ObjectValues"/> and <see cref="object"/> objects have the same values.
        /// </summary>
        /// <param name="left">The <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c>. If <paramref name="right"/> is <c>null</c>, the method returns <c>false</c>.</returns>
        public static bool Equals(ObjectValues left, object right) => Equals(left, From(right));

        /// <summary>
        /// Determines whether the specified object array and <see cref="ObjectValues"/> objects have the same values.
        /// </summary>
        /// <param name="left">The object array to compare.</param>
        /// <param name="right">The <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool Equals(object[] left, ObjectValues right) => Equals(new ObjectValues(left), right);

        /// <summary>
        /// Determines whether the specified <see cref="ObjectValues"/> and object array objects have the same values.
        /// </summary>
        /// <param name="left">The <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The object array to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool Equals(ObjectValues left, object[] right) => Equals(left, new ObjectValues(right));

        #endregion

        #region operators

        /// <summary>
        /// Determines whether two specified <see cref="ObjectValues"/> have the same values.
        /// </summary>
        /// <param name="left">The first <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The second <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(ObjectValues left, ObjectValues right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="ObjectValues"/> have different values.
        /// </summary>
        /// <param name="left">The first <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The second <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is different to the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(ObjectValues left, ObjectValues right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc cref="Equals(ObjectValues, object)" />
        public static bool operator ==(ObjectValues left, object right) => Equals(left, From(right));

        /// <summary>
        /// Determines whether the specified <see cref="ObjectValues"/> and <see cref="object"/> objects have different values.
        /// </summary>
        /// <param name="left">The <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is different to the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(ObjectValues left, object right) => !Equals(left, From(right));

        /// <inheritdoc cref="Equals(object, ObjectValues)" />
        public static bool operator ==(object left, ObjectValues right) => Equals(From(left), right);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> and <see cref="ObjectValues"/> objects have different values.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is different to the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(object left, ObjectValues right) => !Equals(From(left), right);

        /// <inheritdoc cref="Equals(ObjectValues, object[])" />
        public static bool operator ==(ObjectValues left, object[] right) => Equals(left, new ObjectValues(right));

        /// <summary>
        /// Determines whether the specified <see cref="ObjectValues"/> and object array have different values.
        /// </summary>
        /// <param name="left">The <see cref="ObjectValues"/> to compare.</param>
        /// <param name="right">The object array to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is different to the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(ObjectValues left, object[] right) => !Equals(left, new ObjectValues(right));

        /// <inheritdoc cref="Equals(object[], ObjectValues)" />
        public static bool operator ==(object[] left, ObjectValues right) => Equals(new ObjectValues(left), right);

        /// <summary>
        /// Determines whether the specified object array and <see cref="ObjectValues"/> have different values.
        /// </summary>
        /// <param name="left">The object array to compare.</param>
        /// <param name="right">The <see cref="ObjectValues"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="left"/> is different to the value of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(object[] left, ObjectValues right) => !Equals(new ObjectValues(left), right);

        #endregion

        /// <summary>
        /// Creates a new <c>ObjectValues</c> from a single or array of <c>object</c>s.
        /// </summary>
        /// <param name="objs">A single or array of <c>object</c>s.</param>
        /// <returns>An <c>ObjectValues</c>.</returns>
        public static ObjectValues From(params object[] objs) => new ObjectValues(objs);

        /// <summary>
        /// Creates a new <c>ObjectValues</c> from a single or array of <c>strings</c>s.
        /// </summary>
        /// <param name="stringValues">A single or array of <c>strings</c>s.</param>
        /// <returns>An <c>ObjectValues</c>.</returns>
        public static ObjectValues FromStringValues(StringValues stringValues) => new ObjectValues(stringValues);

        /// <summary>
        /// An <see cref="ObjectValues"/> with an empty backing list.
        /// </summary>
        public static ObjectValues Empty { get; } = From();
    }
}
