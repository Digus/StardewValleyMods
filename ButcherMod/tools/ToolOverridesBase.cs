﻿using System;
using System.Reflection;
using StardewValley;

namespace AnimalHusbandryMod.tools
{
    public class ToolOverridesBase
    {
        protected static void BaseToolDoFunction(Tool instance, GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            var baseMethod = typeof(Tool).GetMethod("DoFunction", BindingFlags.Public | BindingFlags.Instance);
            var functionPointer = baseMethod.MethodHandle.GetFunctionPointer();
            var function = (Action<GameLocation, int, int, int, StardewValley.Farmer>)Activator.CreateInstance(typeof(Action<GameLocation, int, int, int, StardewValley.Farmer>), instance, functionPointer);
            function(location, x, y, power, who);
        }
    }
}