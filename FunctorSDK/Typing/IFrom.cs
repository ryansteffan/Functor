using System;
using System.Collections.Generic;
using System.Text;

namespace FunctorSDK.Typing;

public interface IFrom<TSelf, TFrom>
{
    public static abstract TSelf From(TFrom value);
}
