using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionMethodInvokeWays
{
    internal class MethodInvoke
    {
        /// <summary>
        /// Way7�G �ϥ� Reflection.Emit.DynamicMethod �ʺA���� IL �óz�L CreateDelegate �إ� Func �e�������k�C
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way7_UsingDynamicMethodCall(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;
            var type = targetObject.GetType();

            var fooMethod = type.GetMethod("Foo");

            // Ref�Ghttps://msdn.microsoft.com/zh-tw/library/system.reflection.emit.dynamicmethod(v=vs.110).aspx
            var fooDynamicMethod = new DynamicMethod("Foo_",
                typeof(string),
                new[] { type, typeof(int) },
                true);
            var fooIl = fooDynamicMethod.GetILGenerator();
            fooIl.DeclareLocal(typeof(string));
            fooIl.Emit(OpCodes.Ldarg_0);
            fooIl.Emit(OpCodes.Ldarg_1);
            fooIl.Emit(OpCodes.Call, fooMethod);
            fooIl.Emit(OpCodes.Ret);

            var fooFunc = (Func<int, string>)fooDynamicMethod.CreateDelegate(typeof(Func<int, string>), targetObject);
            var fooResult = fooFunc(7);

            var barMethod = type.GetMethod("Bar");
            var barDynamicMethod = new DynamicMethod("Bar_",
                typeof(string),
                new[] { type, typeof(string) },
                true);
            var barIl = barDynamicMethod.GetILGenerator();
            barIl.DeclareLocal(typeof(string));
            barIl.Emit(OpCodes.Ldarg_0);
            barIl.Emit(OpCodes.Ldarg_1);
            barIl.Emit(OpCodes.Call, barMethod);
            barIl.Emit(OpCodes.Ret);

            var barFunc = (Func<string, string>)barDynamicMethod.CreateDelegate(typeof(Func<string, string>), targetObject);
            var barResult = barFunc("Nancy");

            PrintResult(baseName, fooResult, barResult);
        }

        /// <summary>
        /// Way6�G �إ� expresion �öi���k�I�s�C
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way6_CreateExpressionCall(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;
            var thisObject = Expression.Constant(targetObject);

            var fooMethod = targetObject.GetType().GetMethod("Foo");
            var intValue = Expression.Parameter(typeof(int), "value");
            var fooCall = Expression.Call(thisObject, fooMethod, intValue);
            var fooLambda = Expression.Lambda<Func<int, string>>(fooCall, intValue);
            var fooFunc = fooLambda.Compile();
            var fooResult = fooFunc(6);

            var barMethod = targetObject.GetType().GetMethod("Bar");
            var strValue = Expression.Parameter(typeof(string), "value");
            var barCall = Expression.Call(thisObject, barMethod, strValue);
            var barLambda = Expression.Lambda<Func<string, string>>(barCall, strValue);
            var barFunc = barLambda.Compile();
            var barResult = barFunc("Metilda");

            PrintResult(baseName, fooResult, barResult);
        }

        /// <summary>
        /// Way5�G�ϥ� Delegate.CreateDelegate �إߩe���éI�s��k�C
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way5_CreateDelegateCall(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;

            var fooMethod = targetObject.GetType().GetMethod("Foo");
            var fooFunc = (Func<int, string>)Delegate.CreateDelegate(typeof(Func<int, string>), targetObject, fooMethod);
            var fooResult = fooFunc(5);

            var barMethod = targetObject.GetType().GetMethod("Bar");
            var barFunc = (Func<string, string>)Delegate.CreateDelegate(typeof(Func<string, string>), targetObject, barMethod);
            var barResult = barFunc("Gina");

            PrintResult(baseName, fooResult, barResult);
        }

        /// <summary>
        /// Way4�G�ϥ� dynamic ����r�C
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way4_Using_dynamic_Keyword(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;

            // ���I�G���h�j���O�ĪG�A���t�פ��t
            var dynamicTarget = (dynamic)targetObject;
            // .Foo ���|�� IntelliSense ���ܡC
            var fooResult = dynamicTarget.Foo(4) as string;
            // .Bar ���|�� IntelliSense ���ܡC
            var barResult = dynamicTarget.Bar("Love") as string;

            PrintResult(baseName, fooResult, barResult);
        }

        /// <summary>
        /// Way3�G�ϥί¤Ϯg(�ϬM)�� Methoid.Invoke �i���k�I�s
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way3_UsingMethodInvoke(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;

            var foo = targetObject.GetType().GetMethod("Foo");
            // GetTypeInfo() �O����s�� API�A�䴩�󥭥x
            // var foo2 = targetObject.GetType().GetTypeInfo().GetDeclaredMethod("Foo");
            var fooResult = foo.Invoke(targetObject, new object[] { 3 }) as string;

            var bar = targetObject.GetType().GetMethod("Bar");
            var barResult = bar.Invoke(targetObject, new object[] { "Happy" }) as string;

            PrintResult(baseName, fooResult, barResult);
        }

        /// <summary>
        /// Way2�G�إ� lambda �öi���k�I�s�C
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way2_CreateALambdaCall(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;

            Func<int, string> fooFunc = f => ((MyClass)targetObject).Foo(f);
            var fooResult = fooFunc(2);

            Func<string, string> barFunc = b => ((MyClass)targetObject).Bar(b);
            var barResult = barFunc("Sherry");

            PrintResult(baseName, fooResult, barResult);
        }

        /// <summary>
        /// Way1�G���󪽱��i���k�I�s�C
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        internal void Way1_DirectMethodCall(object targetObject)
        {
            var baseName = MethodBase.GetCurrentMethod().Name;

            var myClass = ((MyClass)targetObject);
            var fooResult = myClass.Foo(1);
            var barResult = myClass.Bar("Bruce");

            PrintResult(baseName, fooResult, barResult);
        }

        [Conditional("DEBUG")]
        private void PrintResult(string baseName, string foo, string bar)
        {
            Console.WriteLine($"{baseName}: {foo}, {bar}");
        }
    }
}