﻿using System;

namespace ET
{
    public abstract class ConfigSingleton<T>: ProtoObject, ICategoryInit, ISingleton where T : ConfigSingleton<T>, new()
    {
        [StaticField]
        private static T instance;

        public static T Instance
        {
            get
            {
                return instance ??= ConfigComponent.Instance.LoadOneConfig(typeof (T)) as T;
            }
        }

        void ISingleton.Register()
        {
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof (T).Name}");
            }

            instance = (T)this;
        }

        void ISingleton.Destroy()
        {
            T t = instance;
            instance = null;
            t.Dispose();
        }

        bool ISingleton.IsDisposed()
        {
            throw new NotImplementedException();
        }

        public sealed override void AfterEndInit()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void AfterCategoryInit()
        {
        }

        public virtual void CategoryInit(ICategoryInit category)
        {
        }
    }
}