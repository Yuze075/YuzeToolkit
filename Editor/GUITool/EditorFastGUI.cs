using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.Editor.GUITool
{
    public static class EditorFastGUI
    {
        public static void Test()
        {
            Horizontal(new Rect(), new IFunctionWrapper[]
            {
                _(EditorGUI.LabelField, ""),
                _(EditorGUI.Toggle, false).O(_ => { }),
                _(Horizontal, new[]
                {
                    _(EditorGUI.LabelField, ""),
                    _(EditorGUI.Toggle, false, _ => { }),
                }).O(vs => vs.O(out bool b).O(out int i, out int i1))
            });
        }

        public static VS Horizontal(Rect rect, IFunctionWrapper[] wrappers)
        {
            var weights = 0;
            var lengths = 0f;
            foreach (var wrapper in wrappers)
            {
                if (wrapper.Length < 0) weights += wrapper.Weight;
                else lengths += wrapper.Length;
            }

            if (lengths > rect.width)
            {
                
            }
            else
            {
                
            }
            
            return null;
        }

        #region FunctionWrapper

        public static ActionWrapper _(Action<Rect> action, int weight = 1, float length = -1f) =>
            new(action.Invoke, weight, length);

        public static FuncWrapper<TR> _<TR>(Func<Rect, TR> func, int weight = 1, float length = -1f) =>
            new(func.Invoke, weight, length);

        public static ActionWrapper _<TR>(Func<Rect, TR> func, Action<TR> @out, int weight = 1, float length = -1f) =>
            new(rect =>
            {
                var tr = func.Invoke(rect);
                @out?.Invoke(tr);
            }, weight, length);

        public static ActionWrapper _<T>(Action<Rect, T> action, T t, int weight = 1, float length = -1f) =>
            new(rect => action.Invoke(rect, t), weight, length);

        public static FuncWrapper<TR> _<T, TR>(Func<Rect, T, TR> func, T t, int weight = 1, float length = -1f) =>
            new(rect => func.Invoke(rect, t), weight, length);

        public static ActionWrapper _<T, TR>(Func<Rect, T, TR> func, T t, Action<TR> @out, int weight = 1,
            float length = -1f) => new(rect =>
        {
            var tr = func.Invoke(rect, t);
            @out?.Invoke(tr);
        }, weight, length);

        public static ActionWrapper _<T1, T2>(Action<Rect, T1, T2> action, T1 t1, T2 t2, int weight = 1,
            float length = -1f) => new(rect => action.Invoke(rect, t1, t2), weight, length);

        public static FuncWrapper<TR> _<T1, T2, TR>(Func<Rect, T1, T2, TR> func, T1 t1, T2 t2, int weight = 1,
            float length = -1f) => new(rect => func.Invoke(rect, t1, t2), weight, length);

        public static ActionWrapper _<T1, T2, TR>(Func<Rect, T1, T2, TR> func, T1 t1, T2 t2, Action<TR> @out,
            int weight = 1, float length = -1f) => new(rect =>
        {
            var tr = func.Invoke(rect, t1, t2);
            @out?.Invoke(tr);
        }, weight, length);

        public static ActionWrapper _<T1, T2, T3>(Action<Rect, T1, T2, T3> action, T1 t1, T2 t2, T3 t3, int weight = 1,
            float length = -1f) => new(rect => action.Invoke(rect, t1, t2, t3), weight, length);

        public static FuncWrapper<TR> _<T1, T2, T3, TR>(Func<Rect, T1, T2, T3, TR> func, T1 t1, T2 t2, T3 t3,
            int weight = 1, float length = -1f) => new(rect => func.Invoke(rect, t1, t2, t3), weight, length);

        public static ActionWrapper _<T1, T2, T3, TR>(Func<Rect, T1, T2, T3, TR> func, T1 t1, T2 t2, T3 t3,
            Action<TR> @out, int weight = 1, float length = -1f) => new(rect =>
        {
            var tr = func.Invoke(rect, t1, t2, t3);
            @out?.Invoke(tr);
        }, weight, length);

        public static ActionWrapper _<T1, T2, T3, T4>(Action<Rect, T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4,
            int weight = 1, float length = -1f) => new(rect => action.Invoke(rect, t1, t2, t3, t4), weight, length);

        public static FuncWrapper<TR> _<T1, T2, T3, T4, TR>(Func<Rect, T1, T2, T3, T4, TR> func, T1 t1, T2 t2, T3 t3,
            T4 t4, int weight = 1, float length = -1f) =>
            new(rect => func.Invoke(rect, t1, t2, t3, t4), weight, length);

        public static ActionWrapper _<T1, T2, T3, T4, TR>(Func<Rect, T1, T2, T3, T4, TR> func, T1 t1, T2 t2, T3 t3,
            T4 t4, Action<TR> @out, int weight = 1, float length = -1f) => new(rect =>
        {
            var tr = func.Invoke(rect, t1, t2, t3, t4);
            @out?.Invoke(tr);
        }, weight, length);


        public static ActionWrapper _<T1, T2, T3, T4, T5>(Action<Rect, T1, T2, T3, T4, T5> action, T1 t1, T2 t2,
            T3 t3, T4 t4, T5 t5, int weight = 1, float length = -1f) =>
            new(rect => action.Invoke(rect, t1, t2, t3, t4, t5), weight, length);

        public static FuncWrapper<TR> _<T1, T2, T3, T4, T5, TR>(Func<Rect, T1, T2, T3, T4, T5, TR> func, T1 t1,
            T2 t2, T3 t3, T4 t4, T5 t5, int weight = 1, float length = -1f) =>
            new(rect => func.Invoke(rect, t1, t2, t3, t4, t5), weight, length);

        public static ActionWrapper _<T1, T2, T3, T4, T5, TR>(Func<Rect, T1, T2, T3, T4, T5, TR> func, T1 t1,
            T2 t2, T3 t3, T4 t4, T5 t5, Action<TR> @out, int weight = 1, float length = -1f) => new(rect =>
        {
            var tr = func.Invoke(rect, t1, t2, t3, t4, t5);
            @out?.Invoke(tr);
        }, weight, length);


        public static ActionWrapper _<T1, T2, T3, T4, T5, T6>(Action<Rect, T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2,
            T3 t3, T4 t4, T5 t5, T6 t6, int weight = 1, float length = -1f) =>
            new(rect => action.Invoke(rect, t1, t2, t3, t4, t5, t6), weight, length);

        public static FuncWrapper<TR> _<T1, T2, T3, T4, T5, T6, TR>(Func<Rect, T1, T2, T3, T4, T5, T6, TR> func,
            T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, int weight = 1, float length = -1f) =>
            new(rect => func.Invoke(rect, t1, t2, t3, t4, t5, t6), weight, length);

        public static ActionWrapper _<T1, T2, T3, T4, T5, T6, TR>(Func<Rect, T1, T2, T3, T4, T5, T6, TR> func, T1 t1,
            T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Action<TR> @out, int weight = 1, float length = -1f) => new(rect =>
        {
            var tr = func.Invoke(rect, t1, t2, t3, t4, t5, t6);
            @out?.Invoke(tr);
        }, weight, length);

        #endregion

        #region Strcut

        public interface IFunctionWrapper
        {
            int Weight { get; }
            float Length { get; }
            IValueOut Invoke(Rect rect);
        }

        public class ActionWrapper : IFunctionWrapper
        {
            public ActionWrapper(Action<Rect> action, int weight, float length) =>
                (_action, Weight, Length) = (action, weight, length);

            private readonly Action<Rect> _action;

            public int Weight { get; }
            public float Length { get; }

            public IValueOut Invoke(Rect rect)
            {
                _action.Invoke(rect);
                return null;
            }
        }

        public class FuncWrapper<TR> : IFunctionWrapper
        {
            public FuncWrapper(Func<Rect, TR> func, int weight, float length) =>
                (_func, Weight, Length) = (func, weight, length);

            private readonly Func<Rect, TR> _func;
            public int Weight { get; }
            public float Length { get; }
            public IValueOut Invoke(Rect rect) => new ValueOut<TR>(_func.Invoke(rect));

            public ActionWrapper O(Action<TR> action)
                => new(rect =>
                {
                    var tr = Invoke(rect);
                    action?.Invoke(tr.Get<TR>());
                }, Weight, Length);
        }

        public interface IValueOut
        {
            T Get<T>();
        }

        public class ValueOut<T> : IValueOut
        {
            public ValueOut(T value) => _value = value;
            private readonly T _value;
            public TValue Get<TValue>() => _value is TValue tValue ? tValue : default;
        }

        public class VS
        {
            private readonly Stack<IValueOut> _valueOuts = new();

            public void Add(IValueOut valueOut)
            {
                if (valueOut != null)
                    _valueOuts.Push(valueOut);
            }

            public VS O<T>(out T t)
            {
                t = _valueOuts.Count == 0 ? default : _valueOuts.Pop().Get<T>();
                return this;
            }

            public VS O<T1, T2>(out T1 t1, out T2 t2)
            {
                O(out t1);
                O(out t2);
                return this;
            }

            public VS O<T1, T2, T3>(out T1 t1, out T2 t2, out T3 t3)
            {
                O(out t1);
                O(out t2);
                O(out t3);
                return this;
            }

            public VS O<T1, T2, T3, T4>(out T1 t1, out T2 t2, out T3 t3, out T4 t4)
            {
                O(out t1);
                O(out t2);
                O(out t3);
                O(out t4);
                return this;
            }

            public VS O<T1, T2, T3, T4, T5>(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5)
            {
                O(out t1);
                O(out t2);
                O(out t3);
                O(out t4);
                O(out t5);
                return this;
            }

            public VS O<T1, T2, T3, T4, T5, T6>(out T1 t1, out T2 t2, out T3 t3, out T4 t4, out T5 t5,
                out T6 t6)
            {
                O(out t1);
                O(out t2);
                O(out t3);
                O(out t4);
                O(out t5);
                O(out t6);
                return this;
            }
        }

        #endregion
    }
}