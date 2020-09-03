using System;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Tests.Fakes
{
    class FakeArgumentInfo : ArgumentInfo
    {
        public FakeArgumentInfo(Type type)
            : base(type, (a, b) => { }, new FakeArgumentAttribute()) { }

        public FakeArgumentInfo(Type type, ArgumentAttribute attribute)
            : base(type, (a, b) => { }, attribute) { }
    }
}