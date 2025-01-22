using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using StardewValley.Network;
using System;
using StardewValley.GameData.Locations;

namespace AdvancedWeatherForecastMod.integrations;

[Flags]
public enum LightingTweenMode
{

    None = 0,
    Before = 1,
    After = 2,
    Both = 3

}

/// <summary>
/// This is the public API surface of Cloudy Skies.
/// </summary>
public interface ICloudySkiesApi
{
    /// <summary>
    /// Enumerate all the custom weather conditions we know about.
    /// </summary>
    IEnumerable<IWeatherData> GetAllCustomWeather();

    /// <summary>
    /// Try to get a custom weather condition by id.
    /// </summary>
    /// <param name="id">The Id of the weather condition to get.</param>
    /// <param name="data">The data, if it exists.</param>
    /// <returns>Whether or not the data exists.</returns>
    bool TryGetWeather(string id, [NotNullWhen(true)] out IWeatherData? data);
}

/// <summary>
/// The data resource for any given custom weather type. This is, for
/// the time being, read-only in the API. Modify the data resource if
/// you want to change the weather, please!
/// </summary>
public interface IWeatherData
{

    /// <summary>
    /// This weather condition's unique Id. This is the Id that you
    /// would check with game state queries, etc.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// A display name to show the player when this weather condition
    /// should be referenced by name. This is a tokenizable string.
    /// </summary>
    string DisplayName { get; set; }

    /// <summary>
    /// The name of the texture containing this weather's icon.
    /// </summary>
    string? IconTexture { get; set; }

    /// <summary>
    /// The location of this weather's icon within the texture. All
    /// weather icons are 12 by 8 pixels.
    /// </summary>
    Point IconSource { get; set; }

    /// <summary>
    /// The name of the texture containing this weather's TV animation.
    /// </summary>
    string? TVTexture { get; set; }

    /// <summary>
    /// The location of this weather's TV animation within the texture.
    /// Each frame of a TV animation is 13 by 13 pixels, and frames are
    /// arranged in a horizontal line.
    /// </summary>
    Point TVSource { get; set; }

    /// <summary>
    /// How many frames long this weather's TV animation is. Default is 4.
    /// </summary>
    int TVFrames { get; set; }

    /// <summary>
    /// How long should each frame of the TV animation be displayed?
    /// Default is 150.
    /// </summary>
    float TVSpeed { get; set; }

    /// <summary>
    /// A forecast string that will be displayed when the player checks
    /// tomorrow's forecast using a television. This is a
    /// tokenizable string. May be null.
    /// </summary>
    string? Forecast { get; set; }

    /// <summary>
    /// A dictionary of forecast strings, by context. These are all
    /// tokenizable strings. May be null.
    /// </summary>
    Dictionary<string, string>? ForecastByContext { get; }

    /// <summary>
    /// An optional string to display to the player when this weather
    /// condition is triggered using a custom weather totem. This is
    /// a tokenizable string.
    /// </summary>
    string? TotemMessage { get; set; }

    /// <summary>
    /// A sound to play immediately when using the totem. The base
    /// game's Rain Totem uses the <c>thunder</c> sound.
    /// </summary>
    string? TotemSound { get; set; }

    /// <summary>
    /// A sound to play 2 seconds after using a totem, as the animation
    /// ends. The base game's Rain Totem uses the <c>rainsound</c> sound.
    /// </summary>
    string? TotemAfterSound { get; set; }

    /// <summary>
    /// A color to flash the screen when using a totem. The base game's
    /// Rain Totem uses the color <c>slateblue</c>.
    /// </summary>
    Color? TotemScreenTint { get; set; }

    /// <summary>
    /// A texture to use for displaying particles when using a totem.
    /// The base game's Rain Totem uses the texture <c>LooseSprites\Cursors</c>
    /// </summary>
    string? TotemParticleTexture { get; set; }

    /// <summary>
    /// The source rectangle of the texture to use when displaying
    /// particles. Defaults to the entire texture. The base game's
    /// Rain Totem uses <c>648, 1045, 52, 33</c>.
    /// </summary>
    Rectangle? TotemParticleSource { get; set; }

    /// <summary>
    /// If this is set to a non-null value, this weather condition will
    /// override the playing music in the same way the base game's raining
    /// condition does, using the named audio cue.
    /// </summary>
    string? MusicOverride { get; set; }

    /// <summary>
    /// The frequency the <see cref="MusicOverride"/> should be played at when
    /// the player is in an outside location.
    /// This does not affect all sound cues. Default is 100.
    /// </summary>
    float MusicFrequencyOutside { get; set; }

    /// <summary>
    /// The frequency the <see cref="MusicOverride"/> should be played at when
    /// the player is in an inside location.
    /// This does not affect all sound cues. Default is 100.
    /// </summary>
    float MusicFrequencyInside { get; set; }

    /// <summary>
    /// An optional list of <see cref="LocationMusicData"/> entries. If
    /// this is set, these will be used when <see cref="StardewValley.GameLocation.GetLocationSpecificMusic"/>
    /// is called in order to override music selection with more nuance than
    /// <see cref="MusicOverride"/> offers. This will override the behavior
    /// of <see cref="MusicOverride"/> if there is a matching entry.
    /// </summary>
    List<LocationMusicData>? SoftMusicOverrides { get; set; }

    /// <summary>
    /// Controls the value of <see cref="LocationWeather.IsRaining"/>.
    /// </summary>
    bool IsRaining { get; set; }

    /// <summary>
    /// Controls the value of <see cref="LocationWeather.IsSnowing"/>.
    /// </summary>
    bool IsSnowing { get; set; }

    /// <summary>
    /// Controls the value of <see cref="LocationWeather.IsLightning"/>.
    /// </summary>
    bool IsLightning { get; set; }

    /// <summary>
    /// Controls the value of <see cref="LocationWeather.IsDebrisWeather"/>.
    /// </summary>
    bool IsDebrisWeather { get; set; }

    /// <summary>
    /// Controls the value of <see cref="LocationWeather.IsGreenRain"/>.
    /// </summary>
    bool IsGreenRain { get; set; }

    /// <summary>
    /// Whether or not crops and pet bowls should be watered at the start of
    /// the day when this weather is active. If this is not set, the behavior
    /// will default based on the value of <see cref="IsRaining"/>.
    /// </summary>
    bool? WaterCropsAndPets { get; set; }

    /// <summary>
    /// If this is set to true, this weather will cause maps to use night
    /// tiles and to have darkened windows, similar to how rain functions.
    /// </summary>
    bool UseNightTiles { get; set; }

    /// <summary>
    /// Whether or not critters should attempt to spawn in outdoor maps
    /// while this weather condition is active. Defaults to true.
    /// </summary>
    bool SpawnCritters { get; set; }

    /// <summary>
    /// Whether or not frogs should attempt to spawn. If this is left at
    /// <c>null</c>, the default logic will be used. <see cref="SpawnCritters"/>
    /// must be enabled for frogs to spawn.
    /// </summary>
    bool? SpawnFrogs { get; set; }

    /// <summary>
    /// Whether or not cloud shadow critters should attempt to spawn. If
    /// this is left at <c>null</c>, the default logic will be used.
    /// <see cref="SpawnCritters"/> must be enabled for cloud shadows to spawn.
    /// </summary>
    bool? SpawnClouds { get; set; }

    public IList<ICritterSpawnData> Critters { get; }

    /// <summary>
    /// A list of this weather type's screen tint data points.
    /// </summary>
    IList<IScreenTintData> Lighting { get; }

    /// <summary>
    /// A list of this weather type's effects.
    /// </summary>
    public IList<IEffectData> Effects { get; }

    /// <summary>
    /// A list of this weather type's layers.
    /// </summary>
    public IList<ILayerData> Layers { get; }
}

public interface ICritterSpawnData
{
    /// <summary>
    /// The unique Id for this critter spawn data entry. This need only
    /// be unique within the <see cref="IWeatherData"/> containing it.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// A game state query for determining if this critter should spawn.
    /// </summary>
    string? Condition { get; set; }

    /// <summary>
    /// If you set a group, only the first critter spawn rule in a group
    /// will be used at any given time.
    /// </summary>
    string? Group { get; set; }

    /// <summary>
    /// The type of critter to spawn.
    /// </summary>
    string Type { get; set; }

    /// <summary>
    /// How likely is this critter to spawn? This is multiplied by a value
    /// calculated based on the size of the current map, which will be a
    /// number between 0.15 and 0.5, inclusive. Setting this to 2.0, for
    /// example, would result in an effective chance between 0.3 and 1.0.
    /// </summary>
    float Chance { get; set; }
}

public interface IScreenTintData
{
    /// <summary>
    /// The unique Id for this set of screen tint information. This need only
    /// be unique within the <see cref="IWeatherData"/> containing it.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// The time of day this screen tint should apply at. By using multiple
    /// screen tint data entries with different time of day values, it is
    /// possible to smoothly blend between them.
    ///
    /// The default value, 600, means 6:00 AM. A value of 1830 means 6:30 PM / 18:30.
    ///
    /// Setting this to zero or a negative value will automatically adjust
    /// the value based on the current location's truly-dark time, which is
    /// typically 2000 though it varies based on the season.
    /// </summary>
    int TimeOfDay { get; set; }

    /// <summary>
    /// A game state query for determining if this set of screen tint data
    /// should be active or not. 
    /// </summary>
    string? Condition { get; set; }

    /// <summary>
    /// How should this screen tint data be blended with the data around it
    /// in the list?
    /// </summary>
    LightingTweenMode TweenMode { get; set; }

    /// <summary>
    /// The ambient color that should be used for lighting when this weather
    /// condition is active. In the base game, only rainy weather uses this,
    /// with the color <c>255, 200, 80</c>.
    /// </summary>
    Color? AmbientColor { get; set; }

    /// <summary>
    /// The opacity to apply the ambient color with when outdoors. This changes
    /// based on the time of day, tweening to <c>0.93</c> when it becomes dark
    /// outside. The initial value is <c>0</c> if it is not raining,
    /// or <c>0.3</c> if it is raining.
    ///
    /// Setting this will override the behavior as it becomes dark out, so
    /// you should use multiple lighting entries with different <see cref="TimeOfDay">
    /// values to implement your own shift, as appropriate.
    /// </summary>
    float? AmbientOutdoorOpacity { get; set; }

    /// <summary>
    /// This color, if set, is drawn to the screen in lighting mode during
    /// the Draw Lightmap phase of the world rendering. In the base game,
    /// only rainy weather uses this, with the color <c>orangered</c>.
    /// </summary>
    Color? LightingTint { get; set; }

    /// <summary>
    /// This opacity is pre-multiplied with <see cref="LightingTint"/>. You
    /// should use it if you want your tint to have an opacity, rather than
    /// directly using the alpha channel.
    ///
    /// In the base game, rainy weather uses <c>0.45</c> for this value.
    /// </summary>
    float LightingTintOpacity { get; set; }

    /// <summary>
    /// This color, if set, is drawn as a screen overlay after the lighting
    /// phase of world rendering. In the base game, only rainy weather uses
    /// this, with the color <c>blue</c>. Green Rain uses the color
    /// <c>0, 120, 150</c>.
    /// </summary>
    Color? PostLightingTint { get; set; }

    /// <summary>
    /// This opacity is pre-multiplied with <see cref="PostLightingTint"/>.
    /// You should use it if you want your tint to have an opacity, rather
    /// than directly using the alpha channel.
    ///
    /// In the base game, rainy weather uses <c>0.2</c> and green rain
    /// uses <c>0.22</c> for this value.
    /// </summary>
    float PostLightingTintOpacity { get; set; }
}

public interface IEffectData
{
    /// <summary>
    /// An identifier for this specific effect within its parent
    /// weather condition. This need only be unique within the weather
    /// condition itself, so you can feel free to use Ids like <c>cold</c>
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// The type of effect this data represents.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// How often should this effect update. A value of <c>60</c> is
    /// once per second. Defaults to <c>60</c>.
    /// </summary>
    uint Rate { get; set; }

    /// <summary>
    /// A condition that must evaluate to true for this effect to
    /// become active. If not set, this effect will always be active. This
    /// condition is only reevaluated upon location change, an event
    /// starting, or the hour changing.
    /// </summary>
    string? Condition { get; set; }

    /// <summary>
    /// If you set a group, only the first effect in a group will be
    /// active at any given time.
    /// </summary>
    string? Group { get; set; }

    /// <summary>
    /// The type(s) of maps that this effect should be active on. Defaults to Outdoors,
    /// unless you have a Condition checking "LOCATION_IS_INDOORS" or
    /// "LOCATION_IS_OUTDOORS" in order to maintain support for effects added
    /// before this property was added. You should remove those conditions in favor
    /// of setting this value, for performance reasons.
    /// </summary>
    public TargetMapType TargetMapType { get; set; }
}

[Flags]
public enum TargetMapType
{
    Outdoors = 1,
    Indoors = 2
}

public enum LayerDrawType
{
    Normal,
    Lighting
}

public interface ILayerData
{
    /// <summary>
    /// An identifier for this specific layer within its parent weather
    /// condition. This need only be unique within the weather
    /// condition itself, so you can feel free to use Ids like <c>rain</c>
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// The type of layer this data represents.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// A condition that must evaluate to true for this layer to be displayed.
    /// If not set, the layer will always be displayed. This condition is
    /// only reevaluated upon location change or the hour changing.
    /// </summary>
    public string? Condition { get; set; }

    /// <summary>
    /// If you set a group, only the first layer in a group will be
    /// displayed at any given time. This can be used to make layers that
    /// display in some situations, with fall-back layers in other situations.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// The type(s) of maps that this layer should render on. Defaults to Outdoors.
    /// </summary>
    public TargetMapType TargetMapType { get; set; }

    public LayerDrawType Mode { get; set; }
}