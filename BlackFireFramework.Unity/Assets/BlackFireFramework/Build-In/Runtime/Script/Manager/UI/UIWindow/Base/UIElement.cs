﻿//----------------------------------------------------
//Copyright © 2008-2018 Mr-Alan. All rights reserved.
//Mail: Mr.Alan.China@[outlook|gmail].com
//Website: www.0x69h.com
//----------------------------------------------------

namespace BlackFireFramework.Unity
{
    public class UIElement : UIBehaviour<IUIElementLogic>, IUIElementLogic
    {
        public override IUIElementLogic Logic
        {
            get
            {
                return this;
            }
        }

        public void BubblingEvent<T>(WindowHandleCallback<T> windowHandleCallback) where T : IUIEventHandler
        {
            var current = transform;
            UIWindow window = null;
            T i = default(T);
            while (null!=current)
            {
                i = current.GetComponent<T>();
                if (null!=i)
                {
                    windowHandleCallback.Invoke(i);
                    return;
                }

                window = current.GetComponent<UIWindow>();
                if (null!=window)
                {
                    if (window.Logic is T)
                    {
                        windowHandleCallback.Invoke((T)window.Logic);
                        return;
                    }
                }
                current = current.parent;
            }
        }
    }
}