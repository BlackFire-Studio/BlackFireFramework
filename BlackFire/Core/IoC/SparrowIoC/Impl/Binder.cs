﻿/*
--------------------------------------------------
| Copyright © 2008 Mr-Alan. All rights reserved. |
| Website: www.0x69h.com                         |
| Mail: mr.alan.china@gmail.com                  |
| QQ: 835988221                                  |
--------------------------------------------------
*/

using System;

namespace BlackFire
{
    internal sealed class Binder : IBinder
    {
        private SparrowIoC m_SparrowIoC = null;

        public object CurrentInstance { get; set; }

        public Type CurrentType { get; set; }

        public Type CurrentTargetType { get; private set; }

        public object[] CurrentParameter { get; set; }

        public bool IsRegisterType { get; set; }

        public Binder(SparrowIoC sparrowIoC)
        {
            m_SparrowIoC = sparrowIoC;
        }

        public IBinder As(Type targetType)
        {
            if (IsRegisterType)
            {
                m_SparrowIoC.RegisterType(targetType, CurrentType, CurrentParameter);
                CurrentTargetType = targetType;
            }
            else
            {
                m_SparrowIoC.RegisterInstance(targetType, CurrentInstance);
            }
            return this;
        }

        public IBinder AsSelf()
        {
            if (IsRegisterType)
            {
                m_SparrowIoC.RegisterType(CurrentType, CurrentType, CurrentParameter);
                CurrentTargetType = CurrentType;
            }
            return this;
        }

        public IBinder AsSingleton()
        {
            if (IsRegisterType)
            {
                m_SparrowIoC.SetSingleInstance(CurrentTargetType);
            }
            else
            {
                m_SparrowIoC.SetSingleInstance(CurrentInstance);
            }
            return this;
        }
    }
}