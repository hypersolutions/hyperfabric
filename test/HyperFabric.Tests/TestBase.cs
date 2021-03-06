using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace HyperFabric.Tests
{
    public abstract class TestBase<TSubject>
    {
        private readonly Dictionary<Type, Mock> _mocks = new Dictionary<Type, Mock>();

        protected TestBase()
        {
            var ctor = typeof (TSubject).GetConstructors().First();

            foreach (var ctorParam in ctor.GetParameters())
            {
                var mockType = typeof(Mock<>);
                Type[] typeArgs = { ctorParam.ParameterType };
                var genericMockType = mockType.MakeGenericType(typeArgs);
                var mock = (Mock)Activator.CreateInstance(genericMockType);
                _mocks.Add(ctorParam.ParameterType, mock);
            }
            
            Subject = (TSubject)ctor.Invoke(_mocks.Values.Select(m => m.Object).ToArray());
        }

        protected TSubject Subject { get; }

        protected Mock<TInterface> MockFor<TInterface>() where TInterface : class
        {
            if (_mocks.ContainsKey(typeof(TInterface)))
                return (Mock<TInterface>)_mocks[typeof (TInterface)];

            throw new InvalidOperationException("Cannot find mock for type: " + typeof(TInterface).FullName);
        }
    }
}
