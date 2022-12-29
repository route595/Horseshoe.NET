﻿using System;

namespace Horseshoe.NET.Compare
{
    /// <summary>
    /// Defines all properties and methods common to Horseshoe.NET comparators.
    /// </summary>
    /// <typeparam name="T">A comparable type.</typeparam>
    public interface IComparator<T> where T : IComparable<T>
    {
        /// <summary>
        /// The compare mode, e.g. Equals, Contains, Between, etc.
        /// </summary>
        CompareMode Mode { get; }


        /// <summary>
        /// Indicates whether the input item is a criteria match.
        /// </summary>
        /// <param name="input"></param>
        /// <returns><c>true</c> or <c>false</c></returns>
        bool IsMatch(T input);
    }
}
