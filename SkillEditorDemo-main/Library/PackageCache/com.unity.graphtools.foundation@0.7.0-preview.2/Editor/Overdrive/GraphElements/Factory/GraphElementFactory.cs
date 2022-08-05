using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityEditor.GraphToolsFoundation.Overdrive
{
    public static class GraphElementFactory
    {
        [CanBeNull]
        public static T CreateUI<T>(GraphView graphView, Store store, IGraphElementModel model) where T : class, IGraphElement
        {
            return CreateUI<T>(graphView, store, model, null);
        }

        public static T CreateUI<T>(GraphView graphView, Store store, IGraphElementModel model, string context) where T : class, IGraphElement
        {
            if (model == null)
            {
                Debug.LogError("GraphElementFactory could not create element because model is null.");
                return null;
            }

            var ext = ExtensionMethodCache<ElementBuilder>.GetExtensionMethod(
                model.GetType(),
                FilterMethods,
                KeySelector
            );

            T newElem = null;
            if (ext != null)
            {
                var nodeBuilder = new ElementBuilder { GraphView = graphView, Context = context };
                newElem = ext.Invoke(null, new object[] { nodeBuilder, store, model }) as T;
            }

            if (newElem == null)
            {
                Debug.LogError($"GraphElementFactory doesn't know how to create a UI for element of type: {model.GetType()}");
                return null;
            }

            return newElem;
        }

        internal static Type KeySelector(MethodInfo x)
        {
            return x.GetParameters()[2].ParameterType;
        }

        internal static bool FilterMethods(MethodInfo x)
        {
            if (x.ReturnType != typeof(IGraphElement))
                return false;

            var parameters = x.GetParameters();
            return parameters.Length == 3 && parameters[1].ParameterType == typeof(Store);
        }
    }
}