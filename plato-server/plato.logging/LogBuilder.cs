using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ImagicleLibrary.Logging
{
    /// <summary>
    /// This class can be used to format in a consistent way your log lines
    /// </summary>
    public class LogBuilder
    {
        #region Private fields

        private readonly StringBuilder _buffer;
        private string _header;
        private string _callerMethod;
        private string _callerType;
        private readonly List<KeyValuePair<string, object>> _keyValuePairs;

        #endregion

        #region Initialization

        private LogBuilder()
        {
            _buffer = new StringBuilder();
            _keyValuePairs = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// Initializes a new log line, possibily with an header message
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static LogBuilder StartNew(
            string header = null, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            var className = callerFilePath.Split('\\')[callerFilePath.Split('\\').Length - 1].Split('.')[0];
            return new LogBuilder().Start(className,  header,  memberName);
        }

        #endregion

        #region KeyValue

        /// <summary>
        /// Appends to the current log line the provided key-value pair
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public LogBuilder KeyValue(string key, object value)
        {
            _keyValuePairs.Add(new KeyValuePair<string, object>(key, value));
            return this;
        }

        /// <summary>
        /// Appends to the current log line the provided key-value pair iff the value is not null
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        public LogBuilder OptionalKeyValue(string key, object value)
        {
            return OptionalKeyValue(key, value, val => val != null);
        }

        /// <summary>
        /// Appends to the current log line the provided key-value pair iff the condition is satisfied
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <param name="condition">The condition</param>
        /// <returns></returns>
        public LogBuilder OptionalKeyValue<T>(string key, T value, Func<T, bool> condition)
        {
            if (condition(value))
            {
                return KeyValue(key, value);
            }
            return this;
        }

        #endregion

        #region Collections

        /// <summary>
        /// Appends to the current log line the provided collection
        /// </summary>
        /// <param name="collectionName">The name of the given collection (it will be logged)</param>
        /// <param name="values">The collection</param>
        /// <typeparam name="T">The type of the elements of the collection</typeparam>
        public LogBuilder Collection<T>(string collectionName, IEnumerable<T> values)
        {
            return Collection(collectionName, values, arg => arg == null ? string.Empty : arg.ToString());
        }

        /// <summary>
        /// Appends to the current log line the provided collection if the collection is not null
        /// </summary>
        /// <param name="collectionName">The name of the given collection (it will be logged)</param>
        /// <param name="values">The collection</param>
        /// <typeparam name="T">The type of the elements of the collection</typeparam>
        public LogBuilder OptionalCollection<T>(string collectionName, IEnumerable<T> values)
        {
            return values != null ? Collection(collectionName, values) : this;
        }

        /// <summary>
        /// Appends to the current log line the provided collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="values"></param>
        /// <param name="toStringFunc">The function that will be used for representing each one of the given values</param>
        /// <typeparam name="T">The type of the elements of the collection</typeparam>
        /// <returns></returns>
        public LogBuilder Collection<T>(string collectionName, IEnumerable<T> values, Func<T, string> toStringFunc)
        {
            if (values == null)
            {
                return KeyValue(collectionName, null);
            }

            var valuesBuilder = StartNew();
            foreach (var value in values)
            {
                valuesBuilder.KeyValue(string.Empty, toStringFunc(value));
            }
            _keyValuePairs.Add(new KeyValuePair<string, object>(collectionName, valuesBuilder));
            return this;
        }

        /// <summary>
        /// Appends to the current log line the provided collection if the collection is not null
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="values"></param>
        /// <param name="toStringFunc">The function that will be used for representing each one of the given values</param>
        /// <typeparam name="T">The type of the elements of the collection</typeparam>
        /// <returns></returns>
        public LogBuilder OptionalCollection<T>(string collectionName, IEnumerable<T> values, Func<T, string> toStringFunc)
        {
            return values != null ? Collection(collectionName, values, toStringFunc) : this;
        }

        /// <summary>
        /// Appends to the current log line the given dictionary
        /// </summary>
        /// <typeparam name="T1">The type of the key</typeparam>
        /// <typeparam name="T2">The type of the value</typeparam>
        /// <param name="collectionName">The name of the given collection (it will be logged)</param>
        /// <param name="values">The collection</param>
        /// <param name="keyToStringFunc">The function that will be used to convert every key to a string</param>
        /// <param name="valueToStringFunc">The function that will be used to convert every value to a string</param>
        public LogBuilder Collection<T1, T2>(string collectionName, IDictionary<T1, T2> values, Func<T1, string> keyToStringFunc, Func<T2, string> valueToStringFunc)
        {
            return Collection(collectionName, values, pair => StartNew().KeyValue(keyToStringFunc(pair.Key), valueToStringFunc(pair.Value)));
        }

        /// <summary>
        /// Appends to the current log line the given dictionary if the collection is not null
        /// </summary>
        /// <typeparam name="T1">The type of the key</typeparam>
        /// <typeparam name="T2">The type of the value</typeparam>
        /// <param name="collectionName">The name of the given collection (it will be logged)</param>
        /// <param name="values">The collection</param>
        /// <param name="keyToStringFunc">The function that will be used to convert every key to a string</param>
        /// <param name="valueToStringFunc">The function that will be used to convert every value to a string</param>
        public LogBuilder OptionalCollection<T1, T2>(string collectionName, IDictionary<T1, T2> values, Func<T1, string> keyToStringFunc, Func<T2, string> valueToStringFunc)
        {
            return values != null ? Collection(collectionName, values, keyToStringFunc, valueToStringFunc) : this;
        }

        /// <summary>
        /// Appends to the current log line the given dictionary
        /// </summary>
        /// <typeparam name="T1">The type of the key</typeparam>
        /// <typeparam name="T2">The type of the value</typeparam>
        /// <param name="collectionName">The name of the given collection (it will be logged)</param>
        /// <param name="values">The collection</param>
        public LogBuilder Collection<T1, T2>(string collectionName, IDictionary<T1, T2> values)
        {
            return Collection(collectionName, values, key => key?.ToString(), value => value?.ToString());
        }

        /// <summary>
        /// Appends to the current log line the given dictionary if the collection is not null
        /// </summary>
        /// <typeparam name="T1">The type of the key</typeparam>
        /// <typeparam name="T2">The type of the value</typeparam>
        /// <param name="collectionName">The name of the given collection (it will be logged)</param>
        /// <param name="values">The collection</param>
        public LogBuilder OptionalCollection<T1, T2>(string collectionName, IDictionary<T1, T2> values)
        {
            return values != null ? Collection(collectionName, values) : this;
        }

        #endregion

        /// <summary>
        /// Appends to the current log line the provided exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="headerMessage"></param>
        /// <returns></returns>
        public LogBuilder Exception(Exception exception, string headerMessage = null)
        {
            if (exception == null)
            {
                return this;
            }
            return KeyValue(headerMessage, Format(exception));
        }

        /// <summary>
        /// Appends to the current log line the elapsed time of the given stopwatch (which won't be stopped)
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public LogBuilder ElapsedTime(Stopwatch stopwatch)
        {
            var elapsed = stopwatch.Elapsed;
            return elapsed < TimeSpan.FromSeconds(1) ? 
                KeyValue("Elapsed milliseconds", stopwatch.ElapsedMilliseconds) : 
                KeyValue("Elapsed time", stopwatch.Elapsed.ToString());
        }

        /// <summary>
        /// Overwrites current log's header message
        /// </summary>
        /// <param name="header">The new header</param>
        /// <returns></returns>
        public LogBuilder Header(
            string header, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            var className = callerFilePath.Split('\\')[callerFilePath.Split('\\').Length - 1].Split('.')[0];
            return Start(className, header, memberName);
        }

        /// <summary>
        /// Flushes the current log line and returns it
        /// </summary>
        /// <returns></returns>
        public string End()
        {
            AddHeader();
            AddKeyValuePairs();

            var result = _buffer.ToString();
            Clear();

            return result;
        }

        /// <summary>
        /// Automatically convert the current log builder to the string it is building
        /// </summary>
        /// <param name="logBuilder"></param>
        /// <returns></returns>
        public static implicit operator string(LogBuilder logBuilder)
        {
            return logBuilder.End();
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            return End();
        }

        #region Utility Methods

        private LogBuilder Start(string callingtype, string header = null,  string callerMethod = null)
        {
            _callerType = callingtype;
            _header = header;
            _callerMethod = callerMethod;
            return this;
        }

        private void AddHeader()
        {
            if (!string.IsNullOrEmpty(_header))
            {
                if(!string.IsNullOrEmpty(_callerType))
                {
                    _buffer.Append("[Class: " + _callerType + "]");
                }

                if (!string.IsNullOrEmpty(_callerMethod))
                {
                    _buffer.Append("[Method: " + _callerMethod + "]");
                }

                _buffer.Append(_header);
                if (_keyValuePairs.Any())
                {
                    _buffer.Append(": ");
                }
            }
        }

        private void AddKeyValuePairs()
        {
            _buffer.Append(string.Join(", ", _keyValuePairs.Select(FormatKeyValue)));
        }

        private static string FormatKeyValue(KeyValuePair<string, object> pair)
        {
            var value = pair.Value ?? "NULL";
            if (string.IsNullOrEmpty(pair.Key))
            {
                return "{" + value + "}";
            }
            return pair.Key + " {" + value + "}";
        }

        private void Clear()
        {
            _buffer.Clear();
            _keyValuePairs.Clear();
        }

        private static string Format(Exception ex)
        {
            var buffer = new StringBuilder();

            buffer.AppendLine();
            buffer.AppendLine(FormatKeyValue(new KeyValuePair<string, object>("Exception Type", ex.GetType())));
            buffer.AppendLine(FormatKeyValue(new KeyValuePair<string, object>("Message", ex.Message)));
            buffer.AppendLine(FormatKeyValue(new KeyValuePair<string, object>("StackTrace", Environment.NewLine + ex.StackTrace)));

            if (ex.InnerException != null)
            {
                buffer.AppendLine(FormatKeyValue(new KeyValuePair<string, object>("InnerException", Format(ex.InnerException))));
            }

            return buffer.ToString();
        }

        #endregion
    }
}