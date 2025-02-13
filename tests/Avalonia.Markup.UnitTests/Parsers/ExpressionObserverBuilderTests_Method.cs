﻿using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Avalonia.Markup.UnitTests.Parsers
{
    public class ExpressionObserverBuilderTests_Method
    {
        private class TestObject
        {
            public void MethodWithoutReturn() { }

            public int MethodWithReturn() => 0;

            public int MethodWithReturnAndParameters(int i) => i;

            public static void StaticMethod() { }

            public static void ManyParameters(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9) { }
            public static int ManyParametersWithReturnType(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8) => 1;
        }

        [Fact]
        public async Task Should_Get_Method()
        {
            var data = new TestObject();
            var observer = ExpressionObserverBuilder.Build(data, nameof(TestObject.MethodWithoutReturn));
            var result = await observer.Take(1);

            Assert.NotNull(result);

            GC.KeepAlive(data);
        }

        [Theory]
        [InlineData(nameof(TestObject.MethodWithoutReturn), typeof(Action))]
        [InlineData(nameof(TestObject.MethodWithReturn), typeof(Func<int>))]
        [InlineData(nameof(TestObject.MethodWithReturnAndParameters), typeof(Func<int, int>))]
        [InlineData(nameof(TestObject.StaticMethod), typeof(Action))]
        [InlineData(nameof(TestObject.ManyParameters), typeof(Action<int, int, int, int, int, int, int, int, int>))]
        [InlineData(nameof(TestObject.ManyParametersWithReturnType), typeof(Func<int, int, int, int, int, int, int, int, int>))]
        public async Task Should_Get_Method_WithCorrectDelegateType(string methodName, Type expectedType)
        {
            var data = new TestObject();
            var observer = ExpressionObserverBuilder.Build(data, methodName);
            var result = await observer.Take(1);

            Assert.IsType(expectedType, result);

            GC.KeepAlive(data);
        }

        [Fact]
        public async Task Can_Call_Method_Returned_From_Observer()
        {
            var data = new TestObject();
            var observer = ExpressionObserverBuilder.Build(data, nameof(TestObject.MethodWithReturnAndParameters));
            var result = await observer.Take(1);

            var callback = (Func<int, int>)result;

            Assert.Equal(1, callback(1));

            GC.KeepAlive(data);
        }
    }
}
