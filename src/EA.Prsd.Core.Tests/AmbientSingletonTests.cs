/* 
Copyright (c) 2011, NETFx
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list 
  of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other 
  materials provided with the distribution.

* Neither the name of Clarius Consulting nor the names of its contributors may be 
  used to endorse or promote products derived from this software without specific 
  prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
DAMAGE.
*/

namespace EA.Prsd.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class AmbientSingletonSpec
    {
        [Fact]
        public void WhenReusingIdentifier_ThenCanAccessSameValue()
        {
            var identifier = Guid.NewGuid();
            var singleton1 = new AmbientSingleton<string>("foo", identifier);
            var singleton2 = new AmbientSingleton<string>(identifier);

            Assert.Equal("foo", singleton1.Value);
            Assert.Equal("foo", singleton2.Value);
            Assert.Same(singleton1.Value, singleton2.Value);
        }

        [Fact]
        public void WhenSpecifyingGlobalDefault_ThenReturnsItFromSingleton()
        {
            var singleton = new AmbientSingleton<string>("foo");

            Assert.Equal("foo", singleton.Value);
        }

        [Fact]
        public void WhenNoGlobalDefaultSpecified_ThenReturnsDefaultValue()
        {
            var singleton = new AmbientSingleton<string>();

            Assert.Null(singleton.Value);
        }

        [Fact]
        public void WhenSpecifyingAmbientValue_ThenOverridesGlobalDefault()
        {
            var singleton = new AmbientSingleton<string>("foo") {Value = "bar"};

            Assert.Equal("bar", singleton.Value);
        }

        [Fact]
        public async Task WhenSpecifyingAmbientValue_ThenDoesNotOverrideOtherThreadsGlobalDefault()
        {
            // Arrange
            var singleton = new AmbientSingleton<string>("defaultValue");
            string threadSpecificValue = null;
            string defaultValue = null;

            // Act
            var threadSpecificTask = Task.Run(() =>
            {
                singleton.Value = "threadSpecificValue";
                threadSpecificValue = singleton.Value;
            });

            var defaultValueTask = Task.Run(() =>
            {
                defaultValue = singleton.Value;
            });

            await Task.WhenAll(threadSpecificTask, defaultValueTask);

            // Assert
            Assert.Equal("threadSpecificValue", threadSpecificValue);
            Assert.Equal("defaultValue", defaultValue);
        }

        [Fact]
        public void WhenUsingFactory_ThenCanCreateSingleton()
        {
            var s1 = AmbientSingleton.Create("foo");
            var s2 = AmbientSingleton.Create(() => "bar");

            Assert.Equal("foo", s1.Value);
            Assert.Equal("bar", s2.Value);
        }
    }
}