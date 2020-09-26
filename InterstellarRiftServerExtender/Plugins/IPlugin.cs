﻿using System;

namespace IRSE.Managers.Plugins
{
    public interface IPlugin
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        Guid Id
        { get; }
        string GetName
        { get; }
        string Version
        { get; }
        string Description
        { get; }
        string Author
        { get; }
        string API
        { get; }
        #endregion

        #region Methods
        void Init();
        void Shutdown();
        #endregion
    }
}
