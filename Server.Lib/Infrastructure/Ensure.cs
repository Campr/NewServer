using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Lib.Infrastructure
{
    public static class Ensure
    {
        public static class Argument
        {
            /// <summary>
            /// Ensures that the provided argument is not null.
            /// </summary>
            /// <param name="argument">The argument to check.</param>
            /// <param name="argumentName">The argument name, used to report failures.</param>
            /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
            public static void IsNotNull(object argument, string argumentName)
            {
                if (argument == null)
                    throw new ArgumentNullException(argumentName);
            }

            /// <summary>
            /// Ensures the provided argument is not null, whitespace, or empty.
            /// </summary>
            /// <param name="argument">The argument to check.</param>
            /// <param name="argumentName">The argument name, used to report failures.</param>
            /// <exception cref="ArgumentException">Thrown when the argument is whitespace or empty.</exception>
            /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
            public static void IsNotNullOrWhiteSpace(string argument, string argumentName)
            {
                Ensure.Argument.IsNotNull(argument, argumentName);

                if (string.IsNullOrWhiteSpace(argument))
                    throw new ArgumentException("Argument was empty or whitespace.", argumentName);
            }
        }

        public static class Dependency
        {
            /// <summary>
            /// Ensures that the provided dependency is not null.
            /// </summary>
            /// <param name="dependency">The dependency to check.</param>
            /// <param name="dependencyName">The dependency name, used to report failures.</param>
            /// <exception cref="NullReferenceException">Thrown when the dependency is null.</exception>
            public static void IsNotNull(object dependency, string dependencyName)
            {
                if (dependency == null)
                    throw new NullReferenceException($"Required dependency \"{dependencyName}\" is null.");
            }

            /// <summary>
            /// Ensures that none of the dependencies in the provided collection are null.
            /// </summary>
            /// <param name="dependencies">The collection of dependencies to check.</param>
            /// <param name="dependencyCollectionName">The name of the dependency collection, used to report failures.</param>
            /// <exception cref="NullReferenceException">Thrown when any of the dependencies is null.</exception>
            public static void IsNotNull(IEnumerable<object> dependencies, string dependencyCollectionName)
            {
                if (dependencies.Any(d => d == null))
                    throw new NullReferenceException($"One of the requires dependencies in \"{dependencyCollectionName}\" is null.");
            }
        }
    }
}