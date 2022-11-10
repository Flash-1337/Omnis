using System;
using Omnis.Injection.Impl;

namespace Omnis.Injection;

public static class InjectorFactory
{
    public static IInject GetInjector(Injectors injector)
    {
        return injector switch
        {
            Injectors.LoadLibrary => new LoadLibraryInjector(),
            _ => throw new ArgumentException("Injector not implemented within the factory.")
        };
    }
}