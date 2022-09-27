using System.Diagnostics;

namespace SpeechSearchSystem.Domain;

/// <summary>
/// Will throw exceptions when conditions are not satisfied.
/// </summary>
[DebuggerStepThrough]
public class Ensure
{
    /// <summary>
    /// Ensures that the given expression is true
    /// </summary>
    /// <typeparam name="TException">Type of exception to throw</typeparam>
    /// <param name="condition">Condition to test/ensure</param>
    /// <param name="message">Message for the exception</param>
    /// <exception>Thrown when
    ///     <cref>TException</cref>
    ///     <paramref name="condition"/> is false</exception>
    /// <remarks><see cref="TException"/> must have a constructor that takes a single string</remarks>
    public static void That<TException>(bool condition, string message = "") where TException : Exception
    {
        if (!condition)
        {
            throw ((TException)Activator.CreateInstance(typeof(TException), message)!);
        }
    }
}