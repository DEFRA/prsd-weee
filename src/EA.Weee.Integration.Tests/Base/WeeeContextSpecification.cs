
namespace EA.Weee.Integration.Tests.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Specifications;

    namespace EA.Weee.Integration.Tests
    {
        [DebuggerStepThrough]
        [TestFixture]
        public abstract class WeeeContextSpecification
        {
            protected static Exception Exception;
            private static EventHandler _testCleanup;
            public static event EventHandler TestCleanup
            {
                add
                {
                    if (_testCleanup == null || _testCleanup.GetInvocationList().All(i => i.Method != value.GetMethodInfo()))
                    {
                        _testCleanup += value;
                    }
                }
                remove { _testCleanup -= value; }
            }

            public IEnumerator GetEnumerator()
            {
                return GetObservations().Concat(GetObservationsAsync()).GetEnumerator();
            }

            [OneTimeSetUp]
            public virtual void BaseSetUp()
            {
                InitializeContext();
                InvokeEstablish();
                if (Exception != null)
                    throw Exception;

                InvokeBecause();
            }

            void InitializeContext()
            {
                GetType();
            }

            [OneTimeTearDown]
            public virtual void BaseTearDown()
            {
                _testCleanup?.Invoke(this, EventArgs.Empty);
                _testCleanup = null;
                InvokeCleanup();
            }

            void InvokeEstablish()
            {
                var stack = new Stack<Type>();
                var type1 = GetType();

                do
                {
                    stack.Push(type1);
                    type1 = type1.BaseType;
                } while (type1 != typeof(WeeeContextSpecification));

                foreach (var type2 in stack)
                {
                    var num = 100;
                    var fields = type2.GetFields((BindingFlags)num);
                    FieldInfo fieldInfo1 = null;

                    foreach (var fieldInfo2 in fields)
                    {
                        if (fieldInfo2.FieldType.Name.Equals("Establish"))
                            fieldInfo1 = fieldInfo2;
                    }

                    Delegate establish = null;

                    if (fieldInfo1 != null)
                        establish = fieldInfo1.GetValue(this) as Delegate;

                    if (establish != null)
                        Exception = Catch.Exception(() => establish.DynamicInvoke(null));
                }
            }

            void InvokeBecause()
            {
                var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                FieldInfo fieldInfo1 = null;

                foreach (var fieldInfo2 in fields)
                {
                    if (fieldInfo2.FieldType.Name.Equals("Because"))
                        fieldInfo1 = fieldInfo2;
                }

                Delegate because = null;
                if (fieldInfo1 != null)
                    because = fieldInfo1.GetValue(this) as Delegate;

                if (because == null)
                    return;

                Exception = Catch.Exception(() => because.DynamicInvoke(null));
            }

            void InvokeCleanup()
            {
                try
                {
                    var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    FieldInfo fieldInfo1 = null;

                    foreach (var fieldInfo2 in fields)
                    {
                        if (fieldInfo2.FieldType.Name.Equals("Cleanup"))
                            fieldInfo1 = fieldInfo2;
                    }

                    Delegate @delegate = null;
                    if (fieldInfo1 != null)
                        @delegate = fieldInfo1.GetValue(this) as Delegate;

                    if (@delegate == null)
                        return;

                    @delegate.DynamicInvoke(null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    // only throw cleanup errors if there was no error in the main test
                    if (Exception == null)
                        throw;
                }
            }

            public delegate void Because();

            public delegate void Cleanup();

            public delegate void Establish();

            public delegate void It();

            public delegate Task ItAsync();

            protected IEnumerable<TestCaseData> GetObservations()
            {
                return GetObservationsBase("It");
            }

            protected IEnumerable<TestCaseData> GetObservationsAsync()
            {
                return GetObservationsBase("ItAsync");
            }

            public IEnumerable<TestCaseData> GetObservationsBase(string baseDelegateName)
            {
                var t = GetType();

                var categoryName = "Uncategorized";
                var description = string.Empty;

                var categoryAttributes = t.GetTypeInfo().GetCustomAttributes(typeof(CategoryAttribute), true);
                var subjectAttributes = t.GetTypeInfo().GetCustomAttributes(typeof(SubjectAttribute), true);

                if (categoryAttributes.Any())
                {
                    var categoryAttribute = (CategoryAttribute)categoryAttributes.First();
                    categoryName = categoryAttribute.Name;
                }

                if (subjectAttributes.Any())
                {
                    var subjectAttribute = (SubjectAttribute)subjectAttributes.First();
                    description = subjectAttribute.Subject;
                }

                var fieldInfos = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                             BindingFlags.FlattenHierarchy);
                var itFieldInfos = new List<FieldInfo>();

                foreach (var info in fieldInfos)
                    if (info.FieldType.Name.Equals(baseDelegateName))
                        itFieldInfos.Add(info);

                foreach (var it in itFieldInfos)
                {
                    var data = new TestCaseData(it.GetValue(this));
                    data.SetDescription(description);
                    data.SetName(it.Name.Replace("_", " "));
                    data.SetCategory(categoryName);
                    yield return data;
                }
            }

            [SpecificationSource("GetObservations")]
            public void Observations(It observation)
            {
                if (Exception != null)
                    throw Exception;

                observation();
            }

            [SpecificationSource("GetObservationsAsync")]
            public async Task ObservationsAsync(ItAsync observation)
            {
                if (Exception != null)
                    throw Exception;

                await observation();
            }
        }
    }

}
