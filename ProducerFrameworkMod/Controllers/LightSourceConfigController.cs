﻿using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.Controllers
{
    public class LightSourceConfigController
    {
        public static void CreateLightSource(Object producer, Vector2 tileLocation, LightSourceConfig lightSourceConfig)
        {
            Color baseColor;
            //switch (lightSourceConfig.ColorType)
            //{
            //    case ColorType.ObjectColor when producer.heldObject.Value is ColoredObject coloredObject:
            //        baseColor = coloredObject.color.Value;
            //        break;
            //    case ColorType.ObjectDyeColor when TailoringMenu.GetDyeColor(producer.heldObject.Value) is Color color:
            //        baseColor = color;
            //        break;
            //    case ColorType.DefinedColor:
            //    default:
            //        baseColor = lightSourceConfig.Color * lightSourceConfig.ColorFactor;
            //        break;
            //}
            //producer.lightSource = new LightSource(
            //    lightSourceConfig.TextureIndex
            //    , new Vector2(tileLocation.X * 64.0f + 32.0f + lightSourceConfig.OffsetX, tileLocation.Y * 64f + lightSourceConfig.OffsetY)
            //    , lightSourceConfig.Radius
            //    , new Color(byte.MaxValue - baseColor.R, byte.MaxValue - baseColor.G, byte.MaxValue - baseColor.B, baseColor.A)
            //    , GenerateIdentifier(tileLocation)
            //    , LightSource.LightContext.None
            //    , 0L
            //);
        }

        public static int GenerateIdentifier(Vector2 tileLocation)
        {
            return (int)((double)tileLocation.X * 2000.0 + (double)tileLocation.Y);
        }
    }
}
