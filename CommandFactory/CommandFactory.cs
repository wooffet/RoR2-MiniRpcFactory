using MiniRpcFactory.Commands;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MiniRpcFactory.CommandFactory
{
    internal sealed class CommandFactory
    {
        private static Lazy<CommandFactory> _commandFactory { get; set; }
        private static CommandFactory _instance { get { return _commandFactory.Value; } }

        internal static CommandFactory CreateFactoryInstance()
        {
            if (!_commandFactory.IsValueCreated)
            {
                _commandFactory = new Lazy<CommandFactory>(() => new CommandFactory());
            }

            return _instance;
        }
        /*
         * Below is modified from this stack overflow answer: https://stackoverflow.com/a/39961349
         * I left the comments intact as they add context for someone who (like me!) who has not used the Expression library before
         * May change up how factory works as I think this negates the whole ICommand thing
         */

        // this delegate is just, so you don't have to pass an object array. _(params)_
        internal delegate Command<RequestType, ResponseType> ConstructorDelegate<RequestType, ResponseType>(params object[] args);
        internal ConstructorDelegate<RequestType, ResponseType> GenerateCommand<RequestType, ResponseType>(Type commandType, params Type[] parameters)
        {
            // Added this so users can just pass in their parameters
            var parameterTypes = parameters.Select(p => p.GetType()).ToArray();

            // Get the constructor info for these parameters
            var constructorInfo = commandType.GetConstructor(parameters);

            // define a object[] parameter
            var paramExpr = Expression.Parameter(typeof(object[]));

            // To feed the constructor with the right parameters, we need to generate an array 
            // of parameters that will be read from the initialize object array argument.
            var constructorParameters = parameters.Select((paramType, index) =>
                // convert the object[index] to the right constructor parameter type.
                Expression.Convert(
                    // read a value from the object[index]
                    Expression.ArrayAccess(
                        paramExpr,
                        Expression.Constant(index)),
                    paramType)).ToArray();

            // just call the constructor.
            var body = Expression.New(constructorInfo, constructorParameters);

            var constructor = Expression.Lambda<ConstructorDelegate<RequestType, ResponseType>>(body, paramExpr);
            return constructor.Compile();
        }
    }
}
